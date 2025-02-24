namespace Gatherly.Domain.Entities;

public class Attendee
{
    public Attendee(Guid gatheringId, Guid memberId)
    {
        Id = Guid.NewGuid();
        GatheringId = gatheringId;
        MemberId = memberId;
        CreatedOnUtc = DateTime.UtcNow;
    }
    public Guid Id { get; set; }
    public Guid GatheringId { get; private set; }
    public Guid MemberId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
}
