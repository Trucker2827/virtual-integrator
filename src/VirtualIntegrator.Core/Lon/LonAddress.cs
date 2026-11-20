namespace VirtualIntegrator.Core.Lon;

public readonly record struct LonAddress(byte Domain, byte Subnet, byte Node)
{
    public override string ToString() => $"{Domain}/{Subnet}/{Node}";
}
