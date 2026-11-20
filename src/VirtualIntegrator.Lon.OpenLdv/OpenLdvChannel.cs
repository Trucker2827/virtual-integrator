using System.Runtime.Versioning;
using VirtualIntegrator.Core.Lon;

namespace VirtualIntegrator.Lon.OpenLdv;

[SupportedOSPlatform("windows")]
public sealed class OpenLdvChannel : ILonChannel
{
    private IntPtr _handle;
    private bool _disposed;

    public static OpenLdvChannel Open(string deviceName)
    {
        OpenLdvException.ThrowIfError(
            NativeMethods.Open(deviceName, out var handle),
            $"ldv_open({deviceName}) failed");
        return new OpenLdvChannel { _handle = handle };
    }

    public IAsyncEnumerable<LonAddress> ScanAsync(CancellationToken ct = default) =>
        throw new NotImplementedException("Scan requires NM query-id broadcast — pending LonTalk layer.");

    public ValueTask<ReadOnlyMemory<byte>> ReadNvAsync(LonAddress address, int nvIndex, CancellationToken ct = default) =>
        throw new NotImplementedException("NV fetch requires LonTalk APDU layer.");

    public ValueTask WriteNvAsync(LonAddress address, int nvIndex, ReadOnlyMemory<byte> value, CancellationToken ct = default) =>
        throw new NotImplementedException("NV update requires LonTalk APDU layer.");

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        if (_handle != IntPtr.Zero)
        {
            NativeMethods.Close(_handle);
            _handle = IntPtr.Zero;
        }
        await ValueTask.CompletedTask;
    }
}
