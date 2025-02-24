using Gatherly.Domain.Shared.Enums;

namespace Gatherly.Domain.Entities;

public class Gathering
{
    private readonly List<Invitation> _invitations = new List<Invitation>();
    private readonly List<Attendee> _attendees = new List<Attendee>();

    private Gathering()
    {

    }
    private Gathering(
        Guid id,
        Member creator,
        GatheringType type,
        DateTime scheduledAtUtc,
        string name,
        string? location)
    {
        Id = id;
        Creator = creator;
        Type = type;
        ScheduledAtUtc = scheduledAtUtc;
        Name = name;
        Location = location;
    }
    public Guid Id { get; private set; }
    public Member Creator {  get; private set; } = new Member();
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

        switch (gathering.Type)
        {
            case GatheringType.WithFixedNumberOfAttendees:
                if (maximumNumberOfAttendees is null)
                {
                    throw new Exception(
                        $"{nameof(MaximumNumberOfAttendees)} can't be null.");
                }

                gathering.MaximumNumberOfAttendees = maximumNumberOfAttendees;
                break;
            case GatheringType.WithExpirationForInvitations:
                if (invitationsValidBeforeInHours is null)
                {
                    throw new Exception(
                        $"{nameof(invitationsValidBeforeInHours)} can't be null.");
                }

                gathering.InvitationsExpireAtUtc =
                    gathering.ScheduledAtUtc.AddHours(-invitationsValidBeforeInHours.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(GatheringType));
        }

        return gathering;
    }

    public Invitation SendInvitation(Guid invitationId, Guid gatheringId, Guid memberId, InvitationStatus status, DateTime utcNow)
    {
        if (Creator.Id == memberId)
        {
            throw new Exception("Can't send invitation to the gathering creator.");
        }

        if (ScheduledAtUtc < utcNow)
        {
            throw new Exception("Can't send invitation for gathering in the past.");
        }

        var invitation = new Invitation(invitationId, gatheringId, memberId, status, utcNow);

        _invitations.Add(invitation);

        return invitation;
    }

    public Attendee? AcceptInvitation(Invitation invitation)
    {
        // Check if expired
        var expired = (Type == GatheringType.WithFixedNumberOfAttendees &&
                       NumberOfAttendees < MaximumNumberOfAttendees) ||
                      (Type == GatheringType.WithExpirationForInvitations &&
                       InvitationsExpireAtUtc < DateTime.UtcNow);

        if (expired)
        {
            invitation.Expired();
            return null;
        }

        invitation.Accepted();

        // Create attendee
        var attendee = new Attendee(invitation.GatheringId, invitation.MemberId);

        _attendees.Add(attendee);
        NumberOfAttendees++;

        return attendee;
    }
}
