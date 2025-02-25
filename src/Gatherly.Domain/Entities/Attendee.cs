using Gatherly.Domain.Primitives;

namespace Gatherly.Domain.Entities;

public sealed class Attendee : Entity
{
    public Attendee(Guid id, Guid gatheringId, Guid memberId)
        :base(id)
    {
        GatheringId = gatheringId;
        MemberId = memberId;
        CreatedOnUtc = DateTime.UtcNow;
    }
    public Guid GatheringId { get; private set; }
    public Guid MemberId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
}
