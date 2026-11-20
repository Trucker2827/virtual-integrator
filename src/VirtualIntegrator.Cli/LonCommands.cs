using VirtualIntegrator.Core.Lon;
using VirtualIntegrator.Lon.OpenLdv;

namespace VirtualIntegrator.Cli;

internal static class LonCommands
{
    public static async Task<int> ScanAsync(string device)
    {
        return await WithChannelAsync(device, async channel =>
        {
            Console.WriteLine($"Scanning on {device} ...");
            await foreach (var addr in channel.ScanAsync())
            {
                Console.WriteLine(addr);
            }
            return 0;
        });
    }

    public static async Task<int> ReadAsync(string device, string address, int nvIndex)
    {
        var addr = ParseAddress(address);
        return await WithChannelAsync(device, async channel =>
        {
            var bytes = await channel.ReadNvAsync(addr, nvIndex);
            Console.WriteLine(Convert.ToHexString(bytes.Span));
            return 0;
        });
    }

    public static async Task<int> WriteAsync(string device, string address, int nvIndex, string value)
    {
        var addr = ParseAddress(address);
        var bytes = Convert.FromHexString(value);
        return await WithChannelAsync(device, async channel =>
        {
            await channel.WriteNvAsync(addr, nvIndex, bytes);
            Console.WriteLine("OK");
            return 0;
        });
    }

    private static LonAddress ParseAddress(string s)
    {
        var parts = s.Split('/');
        if (parts.Length != 3)
        {
            throw new ArgumentException($"Address must be 'domain/subnet/node' (got '{s}').", nameof(s));
        }
        return new LonAddress(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]));
    }

    private static async Task<int> WithChannelAsync(string device, Func<ILonChannel, Task<int>> body)
    {
        if (!OperatingSystem.IsWindows())
        {
            Console.Error.WriteLine("OpenLDV is Windows-only. Use the IP-852 transport on macOS/Linux (pending).");
            return 2;
        }

        ILonChannel? channel = null;
        try
        {
            channel = OpenLdvChannel.Open(device);
            return await body(channel);
        }
        catch (OpenLdvException ex)
        {
            Console.Error.WriteLine($"OpenLDV error: {ex.Message}");
            return 3;
        }
        catch (NotImplementedException ex)
        {
            Console.Error.WriteLine($"not yet implemented: {ex.Message}");
            return 4;
        }
        catch (DllNotFoundException)
        {
            Console.Error.WriteLine("OpenLDV.dll not found. Install Echelon LNS/OpenLDV runtime on this host.");
            return 5;
        }
        finally
        {
            if (channel is not null)
            {
                await channel.DisposeAsync();
            }
        }
    }
}
