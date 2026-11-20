namespace VirtualIntegrator.Core.Lon;

public interface ILonChannel : IAsyncDisposable
{
    IAsyncEnumerable<LonAddress> ScanAsync(CancellationToken ct = default);
    ValueTask<ReadOnlyMemory<byte>> ReadNvAsync(LonAddress address, int nvIndex, CancellationToken ct = default);
    ValueTask WriteNvAsync(LonAddress address, int nvIndex, ReadOnlyMemory<byte> value, CancellationToken ct = default);
}
