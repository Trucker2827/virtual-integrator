namespace VirtualIntegrator.Data.Entities;

public class Binding
{
    public Guid Id { get; set; }
    public Guid SourceNvId { get; set; }
    public NetworkVariable SourceNv { get; set; } = null!;
    public Guid TargetNvId { get; set; }
    public NetworkVariable TargetNv { get; set; } = null!;
    public BindingService Service { get; set; }
    public byte Retry { get; set; }
    public int RepeatTimerMs { get; set; }
    public int TransmitTimerMs { get; set; }
}

public enum BindingService
{
    Acknowledged,
    Repeated,
    Unacknowledged,
}
