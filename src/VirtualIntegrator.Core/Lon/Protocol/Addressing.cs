namespace VirtualIntegrator.Core.Lon.Protocol;

// Destination-address variants from CEA-709.1 §7 (NPDU addressing formats).
//
//   Broadcast  = fmt 0 — all nodes in the domain (or domain-wide if subnet=0)
//   Group      = fmt 1 — a multicast group address
//   SubnetNode = fmt 2a — unicast to a specific subnet/node
//   NeuronId   = fmt 2b — unicast by 48-bit hardware ID
public abstract record LonDestination
{
    public sealed record Broadcast(byte Domain, byte Subnet) : LonDestination;
    public sealed record Group(byte Domain, byte GroupId) : LonDestination;
    public sealed record SubnetNode(byte Domain, byte Subnet, byte Node) : LonDestination
    {
        public SubnetNode(LonAddress addr) : this(addr.Domain, addr.Subnet, addr.Node) { }
        public LonAddress ToAddress() => new(Domain, Subnet, Node);
    }
    public sealed record NeuronId(byte Domain, byte Subnet, byte[] Id) : LonDestination
    {
        public static NeuronId FromId(byte domain, byte subnet, ReadOnlySpan<byte> id)
        {
            if (id.Length != 6)
            {
                throw new ArgumentException("Neuron ID must be 6 bytes.", nameof(id));
            }
            return new NeuronId(domain, subnet, id.ToArray());
        }
    }
}
