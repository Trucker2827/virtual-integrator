namespace VirtualIntegrator.Data.Entities;

public class Node
{
    public Guid Id { get; set; }
    public Guid SubnetId { get; set; }
    public Subnet Subnet { get; set; } = null!;
    public byte NodeId { get; set; }
    public required string Name { get; set; }
    public byte[]? NeuronId { get; set; }
    public string? ProgramId { get; set; }
    public List<NetworkVariable> NetworkVariables { get; } = [];
}
