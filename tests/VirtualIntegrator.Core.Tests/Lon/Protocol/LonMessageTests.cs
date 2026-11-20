using VirtualIntegrator.Core.Lon;
using VirtualIntegrator.Core.Lon.Protocol;
using Xunit;

namespace VirtualIntegrator.Core.Tests.Lon.Protocol;

public class LonMessageTests
{
    [Fact]
    public void Wink_targets_subnet_node_with_repeated_service()
    {
        var msg = LonMessage.Wink(new LonAddress(1, 2, 3));

        Assert.Equal(MessageCode.NmWink, msg.Code);
        Assert.Equal(ServiceType.Repeated, msg.Service);
        Assert.True(msg.Payload.IsEmpty);
        var dest = Assert.IsType<LonDestination.SubnetNode>(msg.Destination);
        Assert.Equal(new LonAddress(1, 2, 3), dest.ToAddress());
    }

    [Fact]
    public void NvFetch_packs_14_bit_index_in_payload()
    {
        var msg = LonMessage.NvFetch(new LonAddress(1, 2, 3), 0x1234);

        Assert.Equal(MessageCode.NmNvFetch, msg.Code);
        Assert.Equal(ServiceType.Request, msg.Service);
        Assert.Equal(new byte[] { 0x12, 0x34 }, msg.Payload.ToArray());
    }

    [Fact]
    public void NvFetch_rejects_out_of_range_index()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LonMessage.NvFetch(new LonAddress(1, 2, 3), 0x4000));
    }

    [Fact]
    public void NvUpdate_encodes_direction_bit_and_selector()
    {
        var msg = LonMessage.NvUpdate(new LonAddress(1, 2, 3), 0x0005, new byte[] { 0xAA, 0xBB });

        Assert.Equal(0x80, msg.Code & 0x80);
        Assert.Equal(ServiceType.Acknowledged, msg.Service);
        Assert.Equal(new byte[] { 0x05, 0xAA, 0xBB }, msg.Payload.ToArray());
    }

    [Fact]
    public void MessageCode_classifiers()
    {
        Assert.True(MessageCode.IsNetworkManagement(MessageCode.NmWink));
        Assert.True(MessageCode.IsNetworkDiagnostic(MessageCode.NdQueryStatus));
        Assert.True(MessageCode.IsNvMessage(0x80));
        Assert.False(MessageCode.IsNvMessage(MessageCode.NmWink));
    }

    [Fact]
    public void NeuronId_destination_requires_6_bytes()
    {
        Assert.Throws<ArgumentException>(() =>
            LonDestination.NeuronId.FromId(1, 2, new byte[] { 1, 2, 3 }));
        var d = LonDestination.NeuronId.FromId(1, 2, new byte[] { 1, 2, 3, 4, 5, 6 });
        Assert.Equal(6, d.Id.Length);
    }
}
