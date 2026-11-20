using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace VirtualIntegrator.Lon.OpenLdv;

[SupportedOSPlatform("windows")]
internal static partial class NativeMethods
{
    private const string OpenLdvDll = "OpenLDV.dll";

    [LibraryImport(OpenLdvDll, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "ldv_open")]
    internal static partial LdvErr Open(string name, out IntPtr handle);

    [LibraryImport(OpenLdvDll, EntryPoint = "ldv_close")]
    internal static partial LdvErr Close(IntPtr handle);

    [LibraryImport(OpenLdvDll, EntryPoint = "ldv_write")]
    internal static partial LdvErr Write(IntPtr handle, ReadOnlySpan<byte> data, int length);

    [LibraryImport(OpenLdvDll, EntryPoint = "ldv_read")]
    internal static partial LdvErr Read(IntPtr handle, Span<byte> data, int length, out int readLength);
}

public enum LdvErr
{
    Ok = 0,
    NotFound = 1,
    AlreadyOpen = 2,
    NotOpen = 3,
    DeviceErr = 4,
    InvalidBufLen = 5,
    NoMsgAvail = 6,
    NoBuffAvail = 7,
    Capability = 8,
    CapabilityInUse = 9,
    InitializationFailure = 10,
    InternalError = 127,
}
