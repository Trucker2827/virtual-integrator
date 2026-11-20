namespace VirtualIntegrator.Core.Lon.Protocol;

// A single application-layer LonTalk message ready to hand to a transport
// (OpenLDV, CEA-852, etc.). The transport is responsible for wrapping this in
// its own addressing / session envelope.
public sealed record LonMessage(
    LonDestination Destination,
    ServiceType Service,
    byte Code,
    ReadOnlyMemory<byte> Payload,
    bool Authenticated = false,
    bool Priority = false)
{
    public static LonMessage Wink(LonAddress target) => new(
        new LonDestination.SubnetNode(target),
        ServiceType.Repeated,
        MessageCode.NmWink,
        ReadOnlyMemory<byte>.Empty);

    public static LonMessage NvFetch(LonAddress target, int nvIndex)
    {
        if (nvIndex is < 0 or > 0x3FFF)
        {
            throw new ArgumentOutOfRangeException(nameof(nvIndex), "NV index must fit in 14 bits.");
        }
        var payload = new byte[] { (byte)(nvIndex >> 8), (byte)(nvIndex & 0xFF) };
        return new LonMessage(
            new LonDestination.SubnetNode(target),
            ServiceType.Request,
            MessageCode.NmNvFetch,
            payload);
    }

    public static LonMessage NvUpdate(LonAddress target, int nvIndex, ReadOnlyMemory<byte> value)
    {
        if (nvIndex is < 0 or > 0x3FFF)
        {
            throw new ArgumentOutOfRangeException(nameof(nvIndex), "NV index must fit in 14 bits.");
        }
        // NV messages pack the 14-bit selector into (code, next byte): top 6
        // bits in the low 6 bits of the code byte (OR'd with 0x80 direction
        // bit). This is the simplified "polled" update form used with explicit
        // addressing; implicit (bound) NV traffic uses a different selector
        // path and is out of scope for this helper.
        var code = (byte)(0x80 | ((nvIndex >> 8) & 0x3F));
        var payload = new byte[1 + value.Length];
        payload[0] = (byte)(nvIndex & 0xFF);
        value.Span.CopyTo(payload.AsSpan(1));
        return new LonMessage(
            new LonDestination.SubnetNode(target),
            ServiceType.Acknowledged,
            code,
            payload);
    }
}
