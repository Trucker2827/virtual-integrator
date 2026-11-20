using System.CommandLine;
using VirtualIntegrator.Cli;

var deviceOption = new Option<string>(
    aliases: ["--device", "-d"],
    description: "OpenLDV device name (e.g. LON1, LON2). Windows only.",
    getDefaultValue: () => "LON1");

var nvIndexArg = new Argument<int>("nv-index", "Network-variable index on the target node.");
var addressArg = new Argument<string>("address", "LonTalk address as 'domain/subnet/node' (e.g. 1/2/3).");
var valueArg = new Argument<string>("value", "Hex bytes to write (e.g. 'A0B1C2').");

var scan = new Command("scan", "Discover devices on the LON channel.") { deviceOption };
scan.SetHandler(LonCommands.ScanAsync, deviceOption);

var read = new Command("read", "Read a network variable.") { deviceOption, addressArg, nvIndexArg };
read.SetHandler(LonCommands.ReadAsync, deviceOption, addressArg, nvIndexArg);

var write = new Command("write", "Write a network variable.") { deviceOption, addressArg, nvIndexArg, valueArg };
write.SetHandler(LonCommands.WriteAsync, deviceOption, addressArg, nvIndexArg, valueArg);

var lon = new Command("lon", "LonTalk operations") { scan, read, write };

var root = new RootCommand("Virtual Integrator CLI") { lon };
return await root.InvokeAsync(args);
