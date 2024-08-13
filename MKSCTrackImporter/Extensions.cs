using AdvancedLib.Types;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKSCTrackImporter
{
    public static class TileExtensions
    {
        public static Bitmap ToImage(this Tile tile, Palette palette)
        {
            Bitmap bmp = new Bitmap(8, 8);

            Rectangle rect = new Rectangle(0, 0, 8, 8);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Color newColor = palette[tile.indicies[x, y]].ToColor();
                    int position = (y * bmpData.Stride) + (x * bytesPerPixel);
                    rgbValues[position] = newColor.B; // Blue
                    rgbValues[position + 1] = newColor.G; // Green
                    rgbValues[position + 2] = newColor.R; // Red
                    if (bytesPerPixel == 4) // Handle alpha if the pixel format includes it
                    {
                        rgbValues[position + 3] = newColor.A;
                    }
                }
            }

            // Copy modified bytes back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
    }
    public static class ColorExtensions
    {
        public static Color ToColor(this BgrColor color)
        {
            return Color.FromArgb(color.r * 8, color.g * 8, color.b * 8);
        }
    }
}
