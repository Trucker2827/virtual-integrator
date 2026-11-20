namespace VirtualIntegrator.Core.Lon.Protocol;

// LonTalk service type (CEA-709.1 §7, ISO/IEC 14908-1). Wire values follow the
// standard 2-bit encoding used in the NPDU header.
public enum ServiceType : byte
{
    Acknowledged = 0,
    Repeated = 1,
    Unacknowledged = 2,
    Request = 3,
}
