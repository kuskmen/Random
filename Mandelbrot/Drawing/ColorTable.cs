using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Drawing
{
    /// <summary>
    /// Class used for colour lookup table.
    /// </summary>
    public class ColorTable
    {
        private readonly Color[] _colourTable;

        public ColorTable(int kMax)
        {
            _colourTable = new Color[kMax];

            Parallel.For(0, kMax, step =>
            {
                var colourIndex = (double)step / kMax;
                var hue = Math.Pow(colourIndex, 0.30);
                _colourTable[step] = ColorFromHsla(hue, 0.9, 0.6);
            });
        }

        /// <summary>
        /// Retrieve the colour from iteration count k.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Color GetColour(int k)
        {
            return _colourTable[k];
        }

        /// <summary>
        /// Convert HSL colour value to Color object.
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="l">Lightness</param>
        /// <returns>Color object</returns>
        private static Color ColorFromHsla(double h, double s, double l)
        {
            var r = l;
            var g = l;
            var b = l;

            // Standard HSL to RGB conversion. This is described in
            // detail at:
            // http://www.niwa.nu/2013/05/math-behind-colorspace-conversions-rgb-hsl/
            var v = l <= 0.5 ? l * (1.0 + s) : l + s - l * s;

            if (v > 0)
            {
                var m = l + l - v;
                var sv = (v - m) / v;
                h *= 6.0;
                var sextant = (int)h;
                var fract = h - sextant;
                var vsf = v * sv * fract;
                var mid1 = m + vsf;
                var mid2 = v - vsf;

                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;

                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;

                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;

                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;

                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;

                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            // Create Color object from RGB values.
            var color = Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
            return color;
        }
    }
}
