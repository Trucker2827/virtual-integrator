using OpenMcdf;

namespace VirtualIntegrator.Core.Sites;

// Reads the `Graphics` stream from a Circon .vbd / .sub / .app / .fpt OLE
// compound document and surfaces the primitives we have characterized so far:
// embedded images (PNG/JPEG/GIF/BMP magic carve), ASCII labels, and the MFC
// object-class names present. Full vector geometry decoding is deferred.
public static class OleGraphicReader
{
    public static (IReadOnlyList<CarvedImage> images, IReadOnlyList<string> labels, IReadOnlyList<string> classes) Read(string path)
    {
        using var cf = new CompoundFile(path);
        byte[] graphics;
        try
        {
            var stream = cf.RootStorage.GetStream("Graphics");
            graphics = stream.GetData();
        }
        catch (CFItemNotFound)
        {
            return (Array.Empty<CarvedImage>(), Array.Empty<string>(), Array.Empty<string>());
        }

        var images = CarveImages(graphics);
        var (labels, classes) = ScanAscii(graphics);
        return (images, labels, classes);
    }

    private static IReadOnlyList<CarvedImage> CarveImages(byte[] buf)
    {
        var list = new List<CarvedImage>();
        for (int i = 0; i < buf.Length - 8; i++)
        {
            // PNG
            if (buf[i] == 0x89 && buf[i + 1] == 0x50 && buf[i + 2] == 0x4E && buf[i + 3] == 0x47)
            {
                var end = IndexOf(buf, PngTail, i + 8);
                if (end > 0)
                {
                    var len = end + PngTail.Length - i;
                    list.Add(new CarvedImage(ImageKind.Png, i, buf.AsMemory(i, len)));
                    i += len - 1;
                }
            }
            // JPEG
            else if (buf[i] == 0xFF && buf[i + 1] == 0xD8 && buf[i + 2] == 0xFF)
            {
                var end = IndexOf(buf, JpegTail, i + 3);
                if (end > 0)
                {
                    var len = end + JpegTail.Length - i;
                    list.Add(new CarvedImage(ImageKind.Jpeg, i, buf.AsMemory(i, len)));
                    i += len - 1;
                }
            }
            // GIF
            else if (buf[i] == 0x47 && buf[i + 1] == 0x49 && buf[i + 2] == 0x46 && buf[i + 3] == 0x38)
            {
                var end = IndexOf(buf, GifTail, i + 6);
                if (end > 0)
                {
                    var len = end + GifTail.Length - i;
                    list.Add(new CarvedImage(ImageKind.Gif, i, buf.AsMemory(i, len)));
                    i += len - 1;
                }
            }
        }
        return list;
    }

    private static (IReadOnlyList<string> labels, IReadOnlyList<string> classes) ScanAscii(byte[] buf)
    {
        var labels = new List<string>();
        var classes = new HashSet<string>(StringComparer.Ordinal);
        int start = -1;
        for (int i = 0; i <= buf.Length; i++)
        {
            byte b = i < buf.Length ? buf[i] : (byte)0;
            bool printable = b >= 0x20 && b <= 0x7E;
            if (printable)
            {
                if (start < 0) start = i;
            }
            else
            {
                if (start >= 0)
                {
                    int len = i - start;
                    if (len >= 4 && len <= 64)
                    {
                        var s = System.Text.Encoding.ASCII.GetString(buf, start, len);
                        if (IsClassLike(s)) classes.Add(s);
                        else labels.Add(s);
                    }
                    start = -1;
                }
            }
        }
        return (labels, classes.OrderBy(s => s, StringComparer.Ordinal).ToArray());
    }

    private static bool IsClassLike(string s)
    {
        if (s.Length < 4 || s[0] != 'C') return false;
        if (!char.IsUpper(s[1])) return false;
        for (int i = 2; i < s.Length; i++)
        {
            char c = s[i];
            if (!(char.IsLetterOrDigit(c))) return false;
        }
        return true;
    }

    private static int IndexOf(byte[] haystack, byte[] needle, int start)
    {
        for (int i = start; i <= haystack.Length - needle.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j] != needle[j]) { match = false; break; }
            }
            if (match) return i;
        }
        return -1;
    }

    private static readonly byte[] PngTail = { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
    private static readonly byte[] JpegTail = { 0xFF, 0xD9 };
    private static readonly byte[] GifTail = { 0x00, 0x3B };
}
