namespace VirtualIntegrator.Core.Sites;

// Loads a Circon site tree from disk. The on-disk layout for a Visual
// Integrator site is:
//
//   <customer>/<building>/vi/
//       _default.vbd                 landing page
//       <subsystem dir>/             one folder per subsystem (floor, camera group, etc.)
//           _default.sub             subsystem container
//           *.app                    device instances
//           <nested subsystem dir>/  optional
//       *.vbd                        extra top-level pages
//       *.sub                        (rare) top-level subsystems
public static class SiteLoader
{
    public static Project Load(string vInnerPath)
    {
        var root = Path.GetFullPath(vInnerPath);
        if (!Directory.Exists(root))
        {
            throw new DirectoryNotFoundException($"Site directory not found: {root}");
        }
        var name = InferProjectName(root);
        var subsystems = LoadSubsystems(root);
        return new Project(name, root, subsystems);
    }

    private static string InferProjectName(string path)
    {
        var dir = new DirectoryInfo(path);
        if (dir.Name.Equals("vi", StringComparison.OrdinalIgnoreCase) && dir.Parent is not null)
        {
            return dir.Parent.Name;
        }
        return dir.Name;
    }

    private static IReadOnlyList<Subsystem> LoadSubsystems(string dirPath)
    {
        var list = new List<Subsystem>();
        foreach (var child in Directory.EnumerateDirectories(dirPath).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            list.Add(LoadSubsystem(child));
        }
        return list;
    }

    private static Subsystem LoadSubsystem(string dirPath)
    {
        var name = new DirectoryInfo(dirPath).Name;
        var pages = new List<GraphicPage>();
        var devices = new List<DeviceInstance>();

        foreach (var file in Directory.EnumerateFiles(dirPath).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            var ext = Path.GetExtension(file).ToLowerInvariant();
            switch (ext)
            {
                case ".vbd":
                case ".sub":
                case ".fpt":
                    pages.Add(LoadPage(file, ext));
                    break;
                case ".app":
                    devices.Add(LoadDevice(file));
                    break;
            }
        }

        var children = LoadSubsystems(dirPath);
        return new Subsystem(name, dirPath, pages, devices, children);
    }

    private static GraphicPage LoadPage(string path, string ext)
    {
        var kind = ext switch
        {
            ".vbd" => GraphicKind.Vbd,
            ".sub" => GraphicKind.Sub,
            ".fpt" => GraphicKind.Fpt,
            _ => GraphicKind.Vbd,
        };
        var name = Path.GetFileNameWithoutExtension(path);
        try
        {
            var (images, labels, classes) = OleGraphicReader.Read(path);
            return new GraphicPage(name, path, kind, images, labels, classes);
        }
        catch (Exception)
        {
            return new GraphicPage(name, path, kind, Array.Empty<CarvedImage>(), Array.Empty<string>(), Array.Empty<string>());
        }
    }

    private static DeviceInstance LoadDevice(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        try
        {
            var (images, labels, _) = OleGraphicReader.Read(path);
            return new DeviceInstance(name, path, images, labels);
        }
        catch (Exception)
        {
            return new DeviceInstance(name, path, Array.Empty<CarvedImage>(), Array.Empty<string>());
        }
    }
}
