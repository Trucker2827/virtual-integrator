using OpenMcdf;
using VirtualIntegrator.Core.Sites;
using Xunit;

namespace VirtualIntegrator.Core.Tests.Sites;

public class OleGraphicReaderTests
{
    [Fact]
    public void Reads_embedded_PNG_and_class_names_from_synthetic_vbd()
    {
        var temp = Path.GetTempFileName();
        try
        {
            using (var cf = new CompoundFile())
            {
                var graphics = cf.RootStorage.AddStream("Graphics");
                graphics.SetData(BuildGraphicsStream());
                cf.Save(temp);
            }

            var (images, labels, classes) = OleGraphicReader.Read(temp);

            Assert.Single(images);
            Assert.Equal(ImageKind.Png, images[0].Kind);
            Assert.Contains("CHyperFrameOb", classes);
            Assert.Contains("FirstFloor", labels);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void Missing_graphics_stream_returns_empty_collections()
    {
        var temp = Path.GetTempFileName();
        try
        {
            using (var cf = new CompoundFile())
            {
                cf.RootStorage.AddStream("Other").SetData(new byte[] { 1, 2, 3 });
                cf.Save(temp);
            }

            var (images, labels, classes) = OleGraphicReader.Read(temp);
            Assert.Empty(images);
            Assert.Empty(labels);
            Assert.Empty(classes);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    private static byte[] BuildGraphicsStream()
    {
        using var ms = new MemoryStream();
        // ASCII label that should land in labels[]
        var label = System.Text.Encoding.ASCII.GetBytes("FirstFloor");
        ms.Write(label);
        ms.WriteByte(0);
        // Class name that should land in classes[]
        var cls = System.Text.Encoding.ASCII.GetBytes("CHyperFrameOb");
        ms.Write(cls);
        ms.WriteByte(0);
        // Minimal valid PNG: signature + IHDR + IDAT + IEND (1x1 transparent)
        byte[] png = {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
            0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
            0x89,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x44, 0x41, 0x54,
            0x78, 0x9C, 0x62, 0x00, 0x01, 0x00, 0x00, 0x05,
            0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4,
            0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44,
            0xAE, 0x42, 0x60, 0x82,
        };
        ms.Write(png);
        return ms.ToArray();
    }
}
