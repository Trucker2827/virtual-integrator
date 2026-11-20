namespace VirtualIntegrator.Data.Entities;

public class NetworkVariable
{
    public Guid Id { get; set; }
    public Guid NodeId { get; set; }
    public Node Node { get; set; } = null!;
    public int Index { get; set; }
    public required string Name { get; set; }
    public string? SnvtType { get; set; }
    public NetworkVariableDirection Direction { get; set; }
}

public enum NetworkVariableDirection
{
    Input,
    Output,
}
