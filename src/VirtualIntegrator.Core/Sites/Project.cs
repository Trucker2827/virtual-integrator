namespace VirtualIntegrator.Core.Sites;

public sealed record Project(string Name, string RootPath, IReadOnlyList<Subsystem> Subsystems);

public sealed record Subsystem(
    string Name,
    string Path,
    IReadOnlyList<GraphicPage> Pages,
    IReadOnlyList<DeviceInstance> Devices,
    IReadOnlyList<Subsystem> Children);

public sealed record GraphicPage(
    string Name,
    string Path,
    GraphicKind Kind,
    IReadOnlyList<CarvedImage> EmbeddedImages,
    IReadOnlyList<string> Labels,
    IReadOnlyList<string> ObjectClasses);

public sealed record DeviceInstance(
    string Name,
    string Path,
    IReadOnlyList<CarvedImage> EmbeddedImages,
    IReadOnlyList<string> Labels);

public sealed record CarvedImage(ImageKind Kind, int StreamOffset, ReadOnlyMemory<byte> Bytes);

public enum GraphicKind { Vbd, Sub, Fpt }
public enum ImageKind { Png, Jpeg, Gif, Bmp }
