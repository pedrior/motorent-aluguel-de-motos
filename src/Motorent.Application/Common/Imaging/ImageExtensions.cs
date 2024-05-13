namespace Motorent.Application.Common.Imaging;

internal static class ImageExtensions
{
    private static readonly List<byte[]> ImageHeaders =
    [
        [0x42, 0x4D], // BMP
        [0x89, 0x50, 0x4E, 0x47, 0xD, 0xA, 0x1A, 0xA] // PNG
    ];

    public static bool IsImage(this Stream stream)
    {
        var isImage = false;
        foreach (var header in ImageHeaders)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var slice = new byte[header.Length];
            var read = stream.Read(slice, 0, header.Length);

            isImage = read == header.Length && header.SequenceEqual(slice);
            if (isImage)
            {
                break;
            }
        }

        stream.Seek(0, SeekOrigin.Begin);
        return isImage;
    }
}