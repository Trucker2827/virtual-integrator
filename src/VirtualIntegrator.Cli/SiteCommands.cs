using VirtualIntegrator.Core.Sites;

namespace VirtualIntegrator.Cli;

internal static class SiteCommands
{
    public static int Open(string path)
    {
        Project project;
        try
        {
            project = SiteLoader.Load(path);
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 2;
        }

        Console.WriteLine($"Project: {project.Name}");
        Console.WriteLine($"Root:    {project.RootPath}");
        Console.WriteLine($"Subsystems: {project.Subsystems.Count}");
        foreach (var s in project.Subsystems)
        {
            PrintSubsystem(s, 1);
        }
        return 0;
    }

    private static void PrintSubsystem(Subsystem s, int depth)
    {
        var indent = new string(' ', depth * 2);
        var imageCount = s.Pages.Sum(p => p.EmbeddedImages.Count) + s.Devices.Sum(d => d.EmbeddedImages.Count);
        Console.WriteLine($"{indent}- {s.Name}  ({s.Pages.Count} pages, {s.Devices.Count} devices, {s.Children.Count} children, {imageCount} images)");
        foreach (var child in s.Children)
        {
            PrintSubsystem(child, depth + 1);
        }
    }
}
