using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Drawing
{
    /// <summary>
    /// Mandelbrot class extends Form, used to render the Mandelbrot set,
    /// with user controls allowing selection of the region to plot,
    /// resolution, maximum iteration count etc.
    /// </summary>
    public partial class Mandelbrot : Form
    {
        public Mandelbrot()
        {
            InitializeComponent();
        }

        private ScreenPixelManage _myPixelManager;                   // Used for conversions between maths and pixel coordinates.
        private ComplexPoint _zoomCoord1 = new ComplexPoint(-1, 1);  // First point (lower-left) of zoom rectangle.
        private ComplexPoint _zoomCoord2 = new ComplexPoint(-2, 1);  // Second point (upper-right) of zoom rectangle.
        private double _yMin = -2.0;                                 // Default minimum Y for the set to render.
        private double _yMax = 0.0;                                  // Default maximum Y for the set to render.
        private double _xMin = -2.0;                                 // Default minimum X for the set to render.
        private double _xMax = 1.0;                                  // Default maximum X for the set to render.
        private int _zoomScale = 7;                                  // Default amount to zoom in by.

        private Graphics _g;                                         // Graphics object: all graphical rendering is done on this object.
        private LockedBitmap _myBitmap;                              // Bitmap used to draw images.
        private double _xValue;                                      // Save x coordinate on screen click.
        private double _yValue;                                      // Save y coordinate on screen click.
        private int _undoNum = 0;                                    // Undo count, used when undoing user opertions in the form controls.
        private string _userName;                                    // User name.
        private ColourTable _colourTable;                            // Colour table.
        private readonly IDictionary<string, string> _cache = new Dictionary<string, string>();

        /// <summary>
        /// Load the main form for this application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Get current user name. Used to manage their favourites (file storage),
            // and also undo-history storage.
            _userName = Environment.UserName;

            // Create graphics object for Mandelbrot rendering.
            _myBitmap = new LockedBitmap(ClientRectangle.Width,
                                  ClientRectangle.Height,
                                  System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            _g = Graphics.FromImage(_myBitmap.Bitmap);
            // Set the background of the form to white.
            _g.Clear(Color.White);

            // Hide controls that are not relevant until the first rendering has completed.
            zoomCheckbox.Hide();
            undoButton.Hide();

            // Initialise the user's favourites storage.
            Directory.CreateDirectory(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\");
            Directory.CreateDirectory(@"C:\Users\" + _userName + "\\mandelbrot_config\\Undo\\");
            Directory.CreateDirectory(@"C:\Users\" + _userName + "\\mandelbrot_config\\Images\\");
            Array.ForEach(Directory.GetFiles(@"c:\Users\" + _userName + "\\mandelbrot_config\\Undo\\"), File.Delete);
            var dinfo = new DirectoryInfo(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\");
            var files = dinfo.GetFiles("*.txt");
            foreach (var file in files)
            {
                var name = file.Name.Substring(0, file.Name.LastIndexOf(".txt", StringComparison.OrdinalIgnoreCase));
                if (name.Equals(""))
                {
                    File.Delete(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\.txt");
                }
                else
                {
                    favouritesComboBox.Items.Add(name);
                }
            }

            // Initialize undo.
            using (var writer = new StreamWriter(@"C:\Users\" + _userName + "\\mandelbrot_config\\Undo\\undo" + (_undoNum -= 1) + ".txt"))
            {
                writer.Write($"{iterationCountTextBox.Text}\r\n{yMinCheckBox.Text}\r\n{yMaxCheckBox.Text}\r\n{xMinCheckBox.Text}\r\n{xMaxCheckBox.Text}");
            }
        }

        /// <summary>
        /// On-click handler for generate button. Triggers rendering of the Mandelbrot
        /// set using current configuration settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenderMandelbrot(object sender, EventArgs e)
        {
            _cache.TryGetValue(Keys.Iterations, out var previousIterations);
            _cache.AddOrUpdate(Keys.Iterations, iterationCountTextBox.Text);

            var iterations = Convert.ToInt32(_cache[Keys.Iterations]);
            // If colourTable is not yet created or kMax has changed, create colourTable.
            if (_colourTable == null || iterations != Convert.ToInt32(previousIterations))
            {
                _colourTable = new ColourTable(iterations);
            }

            RenderImage(iterations);
        }

        private bool IsFormInvalid()
        {
            return iterationCountTextBox.Text.Equals("") ||
                   yMinCheckBox.Text.Equals("") ||
                   yMaxCheckBox.Text.Equals("") ||
                   xMinCheckBox.Text.Equals("") ||
                   xMaxCheckBox.Text.Equals("");
        }

        private void RenderImage(int iterations)
        {
            try
            {
                statusLabel.Text = "Status: Rendering";
                if (IsFormInvalid())
                {
                    // Choose default parameters and warn the user if the settings are all empty.
                    iterationCountTextBox.Text = "85";
                    yMinCheckBox.Text = "-1";
                    yMaxCheckBox.Text = "1";
                    xMinCheckBox.Text = "-2";
                    xMaxCheckBox.Text = "1";
                    MessageBox.Show("Invalid fields detected. Using default values.");
                    statusLabel.Text = "Status: Error";
                    return;
                }

                // Show zoom and undo controls.
                zoomCheckbox.Show();
                undoButton.Show();
                _undoNum++;

                // Get the x, y range (mathematical coordinates) to plot.
                _yMin = Convert.ToDouble(yMinCheckBox.Text);
                _yMax = Convert.ToDouble(yMaxCheckBox.Text);
                _xMin = Convert.ToDouble(xMinCheckBox.Text);
                _xMax = Convert.ToDouble(xMaxCheckBox.Text);

                // Zoom scale.
                _zoomScale = Convert.ToInt16(zoomTextBox.Text);

                // Clear any existing graphics content.
                _g.Clear(Color.White);

                // Initialise working variables.
                const int kLast = -1;
                var colorLast = Color.Blue;

                // Get screen boundary (lower left & upper right). This is
                // used when calculating the pixel scaling factors.
                var screenBottomLeft = new ComplexPoint(_xMin, _yMin);
                var screenTopRight = new ComplexPoint(_xMax, _yMax);

                // Create pixel manager. This sets up the scaling factors used when
                // converting from mathemathical to screen (pixel units) using the
                _myPixelManager = new ScreenPixelManage(_g, screenBottomLeft, screenTopRight);

                // The pixel step size defines the increment in screen pixels for each point
                // at which the Mandelbrot calcualtion will be done.
                // This increment is converted to mathematical coordinates.
                var xyStep = _myPixelManager.GetDeltaMathsCoord(new ComplexPoint(1, 1));

                // Start stopwatch - used to measure performance improvements
                // (from improving the efficiency of the maths implementation).
                var sw = new Stopwatch();
                sw.Start();

                // Main loop, nested over Imaginary (outer) and Real (inner) values.
                var yPix = _myBitmap.Height - 1;
                for (var y = _yMin; y < _yMax; y += xyStep.Imaginary)
                {
                    var xPix = 0;
                    for (var x = _xMin; x < _xMax; x += xyStep.Real)
                    {
                        // Create complex point C = x + i*y.
                        var c = new ComplexPoint(x, y);

                        // Initialise complex value Zk.
                        var zk = new ComplexPoint(0, 0);

                        // Do the main Mandelbrot calculation. Iterate until the equation
                        // converges or the maximum number of iterations is reached.
                        // TODO: Parallelization
                        var k = 0;
                        double modulusSquared;
                        do
                        {
                            k++;
                            zk = ComplexPoint.Square(zk);
                            zk = ComplexPoint.Add(zk, c);
                            modulusSquared = ComplexPoint.ModulusSquared(zk);
                        } while (modulusSquared <= 4.0 && k < iterations);

                        if (k < iterations)
                        {
                            // Max number of iterations was not reached. This means that the
                            // equation converged. Now assign a colour to the current pixel that
                            // depends on the number of iterations, k, that were done.

                            Color color;
                            if (k == kLast)
                            {
                                // If the iteration count is the same as the last count, re-use the
                                // last pen. This avoids re-calculating colour factors which is
                                // computationally intensive. We benefit from this often because
                                // adjacent pixels are often the same colour, especially in large parts
                                // of the Mandelbrot set that are away from the areas of detail.
                                color = colorLast;
                            }
                            else
                            {
                                // Calculate coluor scaling, from k. We don't use complicated/fancy colour
                                // lookup tables. Instead, the following simple conversion works well:
                                //
                                // hue = (k/kMax)**0.25 where the constant 0.25 can be changed if wanted.
                                // This formula stretches colours allowing more to be assigned at higher values
                                // of k, which brings out detail in the Mandelbrot images.

                                // The following is a full colour calculation, replaced now with colour table.
                                // Uncomment and disable the colour table if wanted. The colour table works
                                // well but supports fewer colours than full calculation of hue and colour
                                // using double-precision arithmetic.
                                //double colourIndex = ((double)k) / kMax;
                                //double hue = Math.Pow(colourIndex, 0.25);

                                // Colour table lookup.
                                // Convert the hue value to a useable colour and assign to current pen.
                                // The saturation and lightness are hard-coded at 0.9 and 0.6 respectively,
                                // which work well.
                                color = _colourTable.GetColour(k);
                                colorLast = color;
                            }

                            if (xPix < _myBitmap.Width && yPix >= 0)
                            {
                                _myBitmap.SetPixel(xPix, yPix, color);
                            }
                        }

                        xPix++;
                    }
                    yPix--;
                }
                // Finished rendering. Stop the stopwatch and show the elapsed time.
                sw.Stop();
                Refresh();
                stopwatchLabel.Text = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
                statusLabel.Text = "Status: Render complete";

                // Save current settings to undo file.
                using (var writer =
                    new StreamWriter(@"C:\Users\" + _userName + "\\mandelbrot_config\\Undo\\undo" + _undoNum + ".txt"))
                {
                    writer.Write(iterationCountTextBox.Text + Environment.NewLine + yMinCheckBox.Text +
                                 Environment.NewLine + yMaxCheckBox.Text + Environment.NewLine + xMinCheckBox.Text +
                                 Environment.NewLine + xMaxCheckBox.Text);
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show("Exception Trapped: " + e2.Message, "Error");
                statusLabel.Text = "Status: Error";
            }
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

        /// <summary>
        /// On-click handler for main form. Defines the points (lower-left and upper-right)
        /// of a zoom rectangle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseClickOnForm(object sender, MouseEventArgs e)
        {
            if (!zoomCheckbox.Checked) return;
            var x = Convert.ToDouble(e.X);
            _xValue = x;
            var y = Convert.ToDouble(e.Y);
            _yValue = y;

            try
            {
                _zoomScale = Convert.ToInt16(zoomTextBox.Text);
            }
            catch (Exception c)
            {
                MessageBox.Show("Error: " + c.Message, "Error");
            }
            // Zoom scale has to be above 0, or their is no point in zooming.
            if (_zoomScale < 1)
            {
                MessageBox.Show("Zoom scale must be above 0");
                _zoomScale = 7;
                zoomTextBox.Text = "7";
                return;
            }

            var pixelCoord = new ComplexPoint((int)(_xValue - 1005 / _zoomScale / 4), (int)(_yValue - 691 / _zoomScale / 4));//
            _zoomCoord1 = _myPixelManager.GetAbsoluteMathsCoord(pixelCoord);
        }

        /// <summary>
        /// Mouse-up handler for main form. The coordinates of the rectangle are
        /// saved so the new drawing can be rendered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseUpOnForm(object sender, MouseEventArgs e)
        {
            if (!zoomCheckbox.Checked) return;

            var pixelCoord = new ComplexPoint((int)(_xValue + 1005 / _zoomScale / 4), (int)(_yValue + 691 / _zoomScale / 4));//
            _zoomCoord2 = _myPixelManager.GetAbsoluteMathsCoord(pixelCoord);

            // Swap to ensure that zoomCoord1 stores the lower-left
            // coordinate for the zoom region, and zoomCoord2 stores the
            // upper right coordinate.
            if (_zoomCoord2.Real < _zoomCoord1.Real)
            {
                var temp = _zoomCoord1.Real;
                _zoomCoord1.Real = _zoomCoord2.Real;
                _zoomCoord2.Real = temp;
            }
            if (_zoomCoord2.Imaginary < _zoomCoord1.Imaginary)
            {
                var temp = _zoomCoord1.Imaginary;
                _zoomCoord1.Imaginary = _zoomCoord2.Imaginary;
                _zoomCoord2.Imaginary = temp;
            }
            yMinCheckBox.Text = Convert.ToString(_zoomCoord1.Imaginary);
            yMaxCheckBox.Text = Convert.ToString(_zoomCoord2.Imaginary);
            xMinCheckBox.Text = Convert.ToString(_zoomCoord1.Real);
            xMaxCheckBox.Text = Convert.ToString(_zoomCoord2.Real);

            RenderImage(Convert.ToInt32(iterationCountTextBox.Text));
        }

        /// <summary>
        /// This will prompt for a favourite name, and then save the current
        /// settings to a text file so they can be used again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToFavourites_Click(object sender, EventArgs e)
        {
            var promptValue = PromptForNewFavourite.ShowDialog("Name", "New Favourite");

            var writer = new StreamWriter(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\" + promptValue + ".txt");
            writer.Write(iterationCountTextBox.Text + Environment.NewLine + yMinCheckBox.Text + Environment.NewLine + yMaxCheckBox.Text + Environment.NewLine + xMinCheckBox.Text + Environment.NewLine + xMaxCheckBox.Text);
            writer.Close();
            writer.Dispose();

            favouritesComboBox.Items.Add(promptValue);
        }

        /// <summary>
        /// This reads the selected text file, and sets the xMin xMax, yMin yMax text
        /// boxes to the coordinates in the text file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFavourites_Click(object sender, EventArgs e)
        {
            var fileContent = File.ReadAllText(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\" + favouritesComboBox.SelectedItem + ".txt");
            var array = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);

            iterationCountTextBox.Text = array[1];
            yMinCheckBox.Text = array[2];
            yMaxCheckBox.Text = array[3];
            xMinCheckBox.Text = array[4];
            xMaxCheckBox.Text = array[5];
        }

        /// <summary>
        /// When the dropdown list is opened, it will check for empty favourite names
        /// and delete them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FavouritesComboBox_DropDown(object sender, EventArgs e)
        {
            var dinfo = new DirectoryInfo(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\");
            var files = dinfo.GetFiles("*.txt");
            foreach (var file in files)
            {
                var name = file.Name.Substring(0, file.Name.LastIndexOf(".txt", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(name)) continue;
                File.Delete(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\.txt");
                favouritesComboBox.Items.Remove(name);
            }
        }

        /// <summary>
        /// When the undo button is clicked, the last settings are read from
        /// the last undo text file, and the text boxes are set to these settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Undo_Click(object sender, EventArgs e)
        {
            try
            {
                var fileContent = File.ReadAllText(@"C:\Users\" + _userName + "\\mandelbrot_config\\Undo\\undo" + (_undoNum -= 1) + ".txt");
                var array1 = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);

                iterationCountTextBox.Text = array1[0];
                yMinCheckBox.Text = array1[1];
                yMaxCheckBox.Text = array1[2];
                xMinCheckBox.Text = array1[3];
                xMaxCheckBox.Text = array1[4];
            }
            catch (Exception e3)
            {
                MessageBox.Show("Unable to Undo: " + e3.Message, "Error");
            }
        }

        /// <summary>
        /// Class used for colour lookup table.
        /// </summary>
        private class ColourTable
        {
            private readonly Color[] _colourTable;

            /// <summary>
            /// Constructor. Creates lookup table.
            /// </summary>
            /// <param name="kMax"></param>
            public ColourTable(int kMax)
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
        }

        // Mandelbrot_Paint handler to draw the image.
        private void Mandelbrot_Paint(object sender, PaintEventArgs e)
        {
            var graphicsObj = e.Graphics;
            graphicsObj.DrawImage(_myBitmap.Bitmap, 0, 0, _myBitmap.Width, _myBitmap.Height);
            graphicsObj.Dispose();
        }

        // Button used to save bitmap at desired location. File type is defaulted as Portable Network Graphics.
        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            _myBitmap.Bitmap.Save(@"C:\Users\" + _userName + "\\mandelbrot_config\\Images\\" + saveImageTextBox.Text + ".png");
        }
    }
}
