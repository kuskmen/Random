using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Drawing
{
    public class MandelbrotRenderer
    {
        /// <summary>The 256 color palette to use for all fractals.</summary>
        private static readonly Color[] _paletteColors = CreatePaletteColors();

        /// <summary>Create the color palette to be used for all fractals.</summary>
        /// <returns>A 256-color array that can be stored into an 8bpp Bitmap's ColorPalette.</returns>
        private static Color[] CreatePaletteColors()
        {
            var paletteColors = new Color[256];
            paletteColors[0] = Color.Black;
            for (short i = 1; i < 256; i++) paletteColors[i] = Color.FromArgb(0, i * 5 % 256, i * 23 % 256); // change this at will for different colorings
            return paletteColors;
        }

        /// <summary>Copy our precreated color palette into the target Bitmap.</summary>
        /// <param name="bmp">The Bitmap to be updated.</param>
        private static void UpdatePalette(Bitmap bmp)
        {
            var p = bmp.Palette;
            Array.Copy(_paletteColors, p.Entries, _paletteColors.Length);
            bmp.Palette = p; // The Bitmap will only update when the Palette property's setter is used
        }

        public static unsafe Bitmap Create(Mandelbrot.MandelbrotCoordinates position, int imageWidth, int imageHeight, CancellationToken cancellationToken, int iterations)
        {
            // In order to use the Bitmap ctor that accepts a stride, the stride must be divisible by four.
            // We're using imageWidth as the stride, so shift it to be divisible by 4 as necessary.
            if (imageWidth % 4 != 0) imageWidth = (imageWidth / 4) * 4;

            // Based on the fractal bounds, determine its upper left coordinate
            var left = position.CenterX - (position.Width / 2);
            var top = position.CenterY - (position.Height / 2);

            // Get the factors that can be multiplied by row and col to arrive at specific x and y values
            var colToXTranslation = position.Width / imageWidth;
            var rowToYTranslation = position.Height / imageHeight;

            // Create the byte array that will store the rendered color indices
            var pixels = imageWidth * imageHeight;
            var data = new byte[pixels]; // initialized to all 0s, which equates to all black based on the default palette

            // Generate the fractal using the mandelbrot formula : z = z^2 + c

            var options = new ParallelOptions { CancellationToken = cancellationToken };
            Parallel.For(0, imageHeight, options, row =>
            {
                var initialY = row * rowToYTranslation + top;
                fixed (byte* ptr = data)
                {
                    var currentPixel = &ptr[row * imageWidth];
                    for (var col = 0; col < imageWidth; col++, currentPixel++)
                    {
                        var c = new Complex(col * colToXTranslation + left, initialY);
                        var z = c;
                        for (var iteration = 0; iteration < iterations; iteration++)
                        {
                            if (z.Magnitude > 4)
                            {
                                *currentPixel = (byte) iteration;
                                break;
                            }

                            z = z * z + c;
                        }
                    }
                }
            });

            // Produce a Bitmap from the byte array of color indices and return it
            fixed (byte* ptr = data)
            {
                using (var tempBitmap = new Bitmap(imageWidth, imageHeight, imageWidth, PixelFormat.Format8bppIndexed, (IntPtr)ptr))
                {
                    var bitmap = tempBitmap.Clone(new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), PixelFormat.Format8bppIndexed);
                    UpdatePalette(bitmap);
                    return bitmap;
                }
            }
        }
    }
}