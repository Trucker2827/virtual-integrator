namespace VirtualIntegrator.Core.Lon.Protocol;

// LonTalk application-layer message code ranges (CEA-709.1 §8).
// Full-byte codes for Network Management (0x60–0x7F) and Network Diagnostic
// (0x50–0x5F); NV traffic uses the high bit (0x80–0xFF) with the NV selector
// encoded in the low 14 bits across the code + following byte.
public static class MessageCode
{
    // Network Management — individual commands verified against izot/lon-stack-ex
    // and the LonTalk Protocol Specification v3.0 (public mirror).
    public const byte NmQueryId = 0x61;
    public const byte NmRespondToQuery = 0x62;
    public const byte NmUpdateDomain = 0x63;
    public const byte NmLeaveDomain = 0x64;
    public const byte NmUpdateKey = 0x65;
    public const byte NmUpdateAddress = 0x66;
    public const byte NmQueryAddress = 0x67;
    public const byte NmQueryNvConfig = 0x68;
    public const byte NmUpdateGroupAddr = 0x69;
    public const byte NmQueryDomain = 0x6A;
    public const byte NmUpdateNvConfig = 0x6B;
    public const byte NmSetNodeMode = 0x6C;
    public const byte NmReadMemory = 0x6D;
    public const byte NmWriteMemory = 0x6E;
    public const byte NmChecksumRecalc = 0x6F;
    public const byte NmWink = 0x70;
    public const byte NmMemoryRefresh = 0x71;
    public const byte NmQuerySnvt = 0x72;
    public const byte NmNvFetch = 0x73;
    public const byte NmDeviceEscape = 0x7D;
    public const byte NmServicePinMsg = 0x7F;

    // Network Diagnostic
    public const byte NdQueryStatus = 0x51;
    public const byte NdProxy = 0x52;
    public const byte NdClearStatus = 0x53;
    public const byte NdQueryXcvrStatus = 0x54;

    // Response bit — ORed with the original request code
    public const byte ResponseBit = 0x20;

    public static bool IsNvMessage(byte code) => (code & 0x80) != 0;
    public static bool IsNetworkManagement(byte code) => code is >= 0x60 and <= 0x7F;
    public static bool IsNetworkDiagnostic(byte code) => code is >= 0x50 and <= 0x5F;
}
