namespace VirtualIntegrator.Data.Entities;

public class Subnet
{
    public Guid Id { get; set; }
    public Guid DomainId { get; set; }
    public LonDomain Domain { get; set; } = null!;
    public byte SubnetId { get; set; }
    public required string Name { get; set; }
    public List<Node> Nodes { get; } = [];
}
