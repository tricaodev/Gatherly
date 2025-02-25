using Gatherly.Domain.Primitives;
using Gatherly.Domain.Shared.Enums;

namespace Gatherly.Domain.Entities;

public sealed class Invitation : Entity
{
    internal Invitation(Guid id, Guid gatheringId, Guid memberId, InvitationStatus status, DateTime createdOnUtc)
        : base(id)
    {
        GatheringId = gatheringId;
        MemberId = memberId;
        Status = status;
        CreatedOnUtc = createdOnUtc;
    }
    public Guid GatheringId { get; private set; }
    public Guid MemberId { get; private set; }
    public InvitationStatus Status { get; internal set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime ModifiedOnUtc { get; internal set; }

    public void Expired()
    {
        Status = InvitationStatus.Expired;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void Accepted()
    {
        Status = InvitationStatus.Accepted;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}
