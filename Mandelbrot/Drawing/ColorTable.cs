using System.Drawing;
using System.Threading.Tasks;

namespace Drawing
{
    /// <summary>
    /// Class used for colour lookup table.
    /// </summary>
    public class ColorTable
    {
        private readonly Color[] _colorPalette;

        public ColorTable()
        {
            _colorPalette = new Color[256];
            _colorPalette[0] = Color.Black;

            Parallel.For(1, _colorPalette.Length, step =>
            {
                _colorPalette[step] = Color.FromArgb(0, step * 3 % 255, step * 3 % 255);
            });
        }

        /// <summary>
        /// Retrieve the colour from iteration count k.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Color GetColour(int k) => _colorPalette[k];
    }
}
