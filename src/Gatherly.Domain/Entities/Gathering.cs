namespace Gatherly.Domain.Entities;

public class Gathering
{
    public int Id { get; set; }
    public int Creator {  get; set; }
    public int Type { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}
