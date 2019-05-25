namespace Mandelbrot
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    ///  Type that handles the logic of rendering Mandelbrot series into <see cref="Bitmap"/>.
    /// </summary>
    public class MandelbrotRenderer
    {
        private static readonly Color[] PaletteColors = CreatePaletteColors();

        /// <summary>Create the color palette to be used for all fractals.</summary>
        /// <returns>A 256-color array that can be stored into an 8bpp Bitmap's ColorPalette.</returns>
        private static Color[] CreatePaletteColors()
        {
            var paletteColors = new Color[256];
            paletteColors[0] = Color.Black;
            for (short i = 1; i < 256; i++) paletteColors[i] = Color.FromArgb(0, i % 256, i * 3 % 256); // change this at will for different colorings
            return paletteColors;
        }

        /// <summary>Copy our precreated color palette into the target <see cref="Bitmap"/>.</summary>
        /// <param name="bmp">The <see cref="Bitmap"/> to be updated.</param>
        private static void UpdatePalette(Bitmap bmp)
        {
            var p = bmp.Palette;
            Array.Copy(PaletteColors, p.Entries, PaletteColors.Length);
            bmp.Palette = p; // The Bitmap will only update when the Palette property's setter is used
        }

        /// <summary>
        /// Creates <see cref="Bitmap"/> with all the pixels already coloured depending on whether they are
        /// in or out of the Mandelbrot series.
        /// </summary>
        /// <param name="position"> Possition of the fractal at the screen. </param>
        /// <param name="size"> How big the <see cref="Bitmap"/> should be.</param>
        /// <param name="iterations"> Number of iterations to iterate before concluding if point is in Mandelbrot series.</param>
        /// <param name="parallelism"> How much processor cores will the rendering use. </param>
        /// <returns> <see cref="Bitmap"/> with all the pixels coloured. </returns>
        public static unsafe Bitmap Create(Mandelbrot.MandelbrotCoordinates position, Size size, int iterations, int parallelism)
        {
            var imageWidth = size.Width;
            var imageHeight = size.Height;

            // In order to use the Bitmap ctor that accepts a stride, the stride must be divisible by four.
            // We're using imageWidth as the stride, so shift it to be divisible by 4 as necessary.
            if (imageWidth % 4 != 0) imageWidth = (imageWidth << 4) * 4;

            // Based on the fractal bounds, determine its upper left coordinate
            var left = position.CenterX - position.Width / 2;
            var top = position.CenterY - position.Height / 2;

            // Get the factors that can be multiplied by row and col to arrive at specific x and y values
            var colToXTranslation = position.Width / imageWidth;
            var rowToYTranslation = position.Height / imageHeight;

            // Create the byte array that will store the rendered color indices
            var pixels = imageWidth * imageHeight;
            var data = new byte[pixels]; // initialized to all 0s, which equates to all black based on the default palette

            // Generate the fractal using the mandelbrot formula : z = z^2 + c
            Parallel.For(0, imageHeight, new ParallelOptions { MaxDegreeOfParallelism = parallelism }, row =>
            {
                var initialY = row * rowToYTranslation + top;
                fixed (byte* ptr = data)
                {
                    var currentPixel = &ptr[row * imageWidth];
                    for (var col = 0; col < imageWidth; col++, currentPixel++)
                    {
                        var c = new Complex(col * colToXTranslation + left, initialY);
                        var z = c;

                        // PERF: We can check first if the point
                        // resides in cardioid before passing it 
                        // for further calculations
                        if (IsInCardioid(z))
                        {
                            continue;
                        }

                        for (var iteration = 0; iteration < iterations; iteration++)
                        {
                            // PERF: As finding magnitude is heaviest
                            // of the operations done through the whole algorithm
                            // mathematical properties of Mandelbrot says that
                            // it is enough to check for infinity only each 8th iteration
                            if (iteration % 8 == 0 && z.Magnitude > 4)
                            {
                                *currentPixel = (byte)iteration;
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

        /// <summary>
        /// Checks if given complex point is within the big cardioid. 
        /// </summary>
        /// <param name="complex"> Complex point to test against. </param>
        /// <returns> True if <paramref name="complex"/> point is in big cardioid and false otherwise. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInCardioid(Complex complex)
        {
            var q = (complex.Real - .25) * (complex.Real - .25) + complex.Imaginary * complex.Imaginary;
            return q * (q + (complex.Real - .25)) < .25 * complex.Imaginary * complex.Imaginary;
        }
    }
}