using VirtualIntegrator.Core.Lon.Protocol;

namespace VirtualIntegrator.Lon.OpenLdv;

// OpenLDV / Neuron Interface Unit (NIU) framing. Every write to the LDV
// handle is a NIU packet laid out as:
//
//   [ niCmd ][ niLen ][ msgOut header ][ data... ]
//
// where niCmd is niCOMM (0x12) for outbound application messages. The
// msgOut header encodes service type, authentication, tag, domain index,
// addressing format, and the destination addressing bytes. Exact bit layout
// is specified in the OpenLDV Programmer's Guide §4 and in `ni_msg.h` from
// the public izot/lon-stack-ex source — this encoder is intentionally
// incomplete and will be filled in with a tested bit-exact implementation
// in a follow-up commit. Keeping it stubbed prevents us from shipping
// plausible-but-wrong packets to real hardware.
internal static class MsgOutBuffer
{
    public const byte NiCommCode = 0x12;

    public static byte[] Encode(LonMessage message)
    {
        _ = message;
        throw new NotImplementedException(
            "NIU msgOut bit-layout pending cross-check against OpenLDV Programmer's Guide §4 + izot ni_msg.h.");
    }
}
