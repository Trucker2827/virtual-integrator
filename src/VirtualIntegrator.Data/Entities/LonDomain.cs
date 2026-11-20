namespace VirtualIntegrator.Data.Entities;

public class LonDomain
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public byte DomainId { get; set; }
    public List<Subnet> Subnets { get; } = [];
}
