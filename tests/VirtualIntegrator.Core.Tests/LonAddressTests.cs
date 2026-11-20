using VirtualIntegrator.Core.Lon;
using Xunit;

namespace VirtualIntegrator.Core.Tests;

public class LonAddressTests
{
    [Fact]
    public void ToString_formats_as_domain_subnet_node()
    {
        var addr = new LonAddress(1, 2, 3);
        Assert.Equal("1/2/3", addr.ToString());
    }
}
