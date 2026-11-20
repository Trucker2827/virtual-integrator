using System.CommandLine;

var root = new RootCommand("Virtual Integrator CLI");

var lon = new Command("lon", "LonTalk operations");
lon.AddCommand(new Command("scan", "Discover devices on the LON channel"));
lon.AddCommand(new Command("read", "Read a network variable"));
lon.AddCommand(new Command("write", "Write a network variable"));
root.AddCommand(lon);

return await root.InvokeAsync(args);
