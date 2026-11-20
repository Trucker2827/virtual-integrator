using System.Runtime.Versioning;
using VirtualIntegrator.Core.Lon;
using VirtualIntegrator.Core.Lon.Protocol;

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
        throw new NotImplementedException("Scan: iterate NM QueryId broadcast with response collection — pending MsgOutBuffer.Encode.");

    public ValueTask<ReadOnlyMemory<byte>> ReadNvAsync(LonAddress address, int nvIndex, CancellationToken ct = default)
    {
        var msg = LonMessage.NvFetch(address, nvIndex);
        return SendAndAwaitResponseAsync(msg, ct);
    }

    public ValueTask WriteNvAsync(LonAddress address, int nvIndex, ReadOnlyMemory<byte> value, CancellationToken ct = default)
    {
        var msg = LonMessage.NvUpdate(address, nvIndex, value);
        return SendAsync(msg, ct);
    }

    private ValueTask SendAsync(LonMessage message, CancellationToken ct)
    {
        _ = ct;
        var buffer = MsgOutBuffer.Encode(message);
        OpenLdvException.ThrowIfError(
            NativeMethods.Write(_handle, buffer, buffer.Length),
            "ldv_write failed");
        return ValueTask.CompletedTask;
    }

    private ValueTask<ReadOnlyMemory<byte>> SendAndAwaitResponseAsync(LonMessage message, CancellationToken ct) =>
        throw new NotImplementedException(
            "Request/response correlation requires MsgInBuffer parser + transaction-ID tracking — pending.");

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
