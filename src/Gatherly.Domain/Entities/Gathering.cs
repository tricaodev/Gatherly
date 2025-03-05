using Gatherly.Domain.Errors;
using Gatherly.Domain.Exceptions;
using Gatherly.Domain.Primitives;
using Gatherly.Domain.Shared;
using Gatherly.Domain.Shared.Enums;

namespace Gatherly.Domain.Entities;

public sealed class Gathering : Entity
{
    private readonly List<Invitation> _invitations = new List<Invitation>();
    private readonly List<Attendee> _attendees = new List<Attendee>();

    private Gathering() : base(Guid.Empty)
    {
    }
    private Gathering(
        Guid id,
        Member creator,
        GatheringType type,
        DateTime scheduledAtUtc,
        string name,
        string? location) : base(id)
    {
        Creator = creator;
        Type = type;
        ScheduledAtUtc = scheduledAtUtc;
        Name = name;
        Location = location;
    }
    public Member Creator { get; private set; } = new Member();
    public GatheringType Type { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime ScheduledAtUtc { get; private set; }
    public string? Location { get; private set; }
    public int? MaximumNumberOfAttendees { get; private set; }
    public DateTime InvitationsExpireAtUtc { get; private set; }
    public int NumberOfAttendees { get; private set; }
    public IReadOnlyCollection<Attendee> Attendees => _attendees;
    public IReadOnlyCollection<Invitation> Invitations => _invitations;

    public static Gathering Create(Guid guid, Member member, GatheringType type, DateTime scheduledAtUtc, string name, string? location,
        int? maximumNumberOfAttendees, int? invitationsValidBeforeInHours)
    {
        Gathering gathering = new Gathering(Guid.NewGuid(), member, type, scheduledAtUtc, name, location);

        gathering.CaculateGatheringTypeDetail(maximumNumberOfAttendees, invitationsValidBeforeInHours);

        return gathering;
    }

    private void CaculateGatheringTypeDetail(int? maximumNumberOfAttendees, int? invitationsValidBeforeInHours)
    {
        switch (Type)
        {
            case GatheringType.WithFixedNumberOfAttendees:
                if (maximumNumberOfAttendees is null)
                {
                    throw new GatheringMaximumNumberOfAttendeesIsNullDomainException(
                        $"{nameof(MaximumNumberOfAttendees)} can't be null.");
                }

                MaximumNumberOfAttendees = maximumNumberOfAttendees;
                break;
            case GatheringType.WithExpirationForInvitations:
                if (invitationsValidBeforeInHours is null)
                {
                    throw new GatheringInvitationsValidBeforeInHoursIsNullDomainException(
                        $"{nameof(invitationsValidBeforeInHours)} can't be null.");
                }

                InvitationsExpireAtUtc =
                    ScheduledAtUtc.AddHours(-invitationsValidBeforeInHours.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(GatheringType));
        }
    }

    public Result<Invitation> SendInvitation(Guid invitationId, Guid gatheringId, Guid memberId, InvitationStatus status, DateTime utcNow)
    {
        if (Creator.Id == memberId)
        {
            return Result.Failure<Invitation>(DomainError.Gathering.InvitingCreator);
        }

        if (ScheduledAtUtc < utcNow)
        {
            return Result.Failure<Invitation>(DomainError.Gathering.AlreadyPassed);
        }

        var invitation = new Invitation(invitationId, gatheringId, memberId, status, utcNow);

        _invitations.Add(invitation);

        return invitation;
    }

    public Result<Attendee> AcceptInvitation(Invitation invitation)
    {
        // Check if expired
        var expired = (Type == GatheringType.WithFixedNumberOfAttendees &&
                       NumberOfAttendees < MaximumNumberOfAttendees) ||
                      (Type == GatheringType.WithExpirationForInvitations &&
                       InvitationsExpireAtUtc < DateTime.UtcNow);

        if (expired)
        {
            invitation.Expired();
            return Result.Failure<Attendee>(DomainError.Gathering.Expired);
        }

        invitation.Accepted();

        // Create attendee
        var attendee = new Attendee(Guid.NewGuid(), invitation.GatheringId, invitation.MemberId);

        _attendees.Add(attendee);
        NumberOfAttendees++;

        return attendee;
    }
}
