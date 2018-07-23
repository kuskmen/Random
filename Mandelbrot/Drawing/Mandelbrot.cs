using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Drawing {
    /// <summary>
    /// Mandelbrot class extends Form, used to render the Mandelbrot set,
    /// with user controls allowing selection of the region to plot,
    /// resolution, maximum iteration count etc.
    /// </summary>
    public partial class Mandelbrot : Form {

        /// <summary>
        /// Constructor.
        /// </summary>
        public Mandelbrot() {
            InitializeComponent();
        }

        private ScreenPixelManage _myPixelManager;                   // Used for conversions between maths and pixel coordinates.
        private ComplexPoint _zoomCoord1 = new ComplexPoint(-1, 1);  // First point (lower-left) of zoom rectangle.
        private ComplexPoint _zoomCoord2 = new ComplexPoint(-2, 1);  // Second point (upper-right) of zoom rectangle.
        private double _yMin = -2.0;                                 // Default minimum Y for the set to render.
        private double _yMax = 0.0;                                  // Default maximum Y for the set to render.
        private double _xMin = -2.0;                                 // Default minimum X for the set to render.
        private double _xMax = 1.0;                                  // Default maximum X for the set to render.
        private int _kMax = 50;                                      // Default maximum number of iterations for Mandelbrot calculation.
        private int _numColours = 85;                                // Default number of colours to use in colour table.
        private int _zoomScale = 7;                                  // Default amount to zoom in by.

        private Graphics _g;                                         // Graphics object: all graphical rendering is done on this object.
        private Bitmap _myBitmap;                                    // Bitmap used to draw images.
        private double _xValue;                                      // Save x coordinate on screen click.
        private double _yValue;                                      // Save y coordinate on screen click.
        private int _undoNum = 0;                                    // Undo count, used when undoing user opertions in the form controls.
        private string _userName;                                    // User name.
        private ColourTable _colourTable = null;                     // Colour table.

        /// <summary>
        /// Load the main form for this application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e) {
            // Get current user name. Used to manage their favourites (file storage),
            // and also undo-history storage.
            _userName = Environment.UserName;

            // Create graphics object for Mandelbrot rendering.
            _myBitmap = new Bitmap(ClientRectangle.Width,
                                  ClientRectangle.Height,
                                  System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            _g = Graphics.FromImage(_myBitmap);
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
            foreach (var file in files) {
                var name = file.Name.Substring(0, file.Name.LastIndexOf(".txt", StringComparison.OrdinalIgnoreCase));
                if (name.Equals("")) {
                    File.Delete(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\.txt");
                } else {
                    favouritesComboBox.Items.Add(name);
                }
            }

            // Initialize undo.
            var writer = new StreamWriter(@"C:\Users\" + _userName + "\\mandelbrot_config\\Undo\\undo" + (_undoNum -= 1) + ".txt");
            writer.Write(pixelStepTextBox.Text +
                Environment.NewLine +
                iterationCountTextBox.Text +
                Environment.NewLine +
                yMinCheckBox.Text +
                Environment.NewLine +
                yMaxCheckBox.Text +
                Environment.NewLine +
                xMinCheckBox.Text +
                Environment.NewLine +
                xMaxCheckBox.Text);
            writer.Close();
            writer.Dispose();
        }

        /// <summary>
        /// On-click handler for generate button. Triggers rendering of the Mandelbrot
        /// set using current configuration settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generate_Click(object sender, EventArgs e) {
            RenderImage();
        }

        private void RenderImage() {
            try {
                statusLabel.Text = "Status: Rendering";
                if (Convert.ToBoolean(pixelStepTextBox.Text.Equals("")) ||
                    Convert.ToBoolean(pixelStepTextBox.Text.Equals("0")) ||
                    Convert.ToBoolean(iterationCountTextBox.Text.Equals("")) ||
                    Convert.ToBoolean(yMinCheckBox.Text.Equals("")) ||
                    Convert.ToBoolean(yMaxCheckBox.Text.Equals("")) ||
                    Convert.ToBoolean(xMinCheckBox.Text.Equals("")) ||
                    Convert.ToBoolean(xMaxCheckBox.Text.Equals(""))) {
                    // Choose default parameters and warn the user if the settings are all empty.
                    pixelStepTextBox.Text = "1";
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

                // Mandelbrot iteration count.
                _kMax = Convert.ToInt32(iterationCountTextBox.Text);
                _numColours = _kMax;

                // If colourTable is not yet created or kMax has changed, create colourTable.
                if (_colourTable == null || _kMax != _colourTable.KMax || _numColours != _colourTable.NColour) {
                    _colourTable = new ColourTable(_numColours, _kMax);
                }

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
                var kLast = -1;
                var colorLast = Color.Red;

                // Get screen boundary (lower left & upper right). This is
                // used when calculating the pixel scaling factors.
                var screenBottomLeft = new ComplexPoint(_xMin, _yMin);
                var screenTopRight = new ComplexPoint(_xMax, _yMax);

                // Create pixel manager. This sets up the scaling factors used when
                // converting from mathemathical to screen (pixel units) using the
                _myPixelManager = new ScreenPixelManage(_g, screenBottomLeft, screenTopRight);

                // The pixel step size defines the increment in screen pixels for each point
                // at which the Mandelbrot calcualtion will be done. e.g. a Step of 5 means
                // that the calcualtion will be done a 5-pixel increments. The X & Y increments
                // are the same.
                //
                // This increment is converted to mathematical coordinates.
                int xyPixelStep = Convert.ToInt16(pixelStepTextBox.Text);
                var pixelStep = new ComplexPoint(xyPixelStep, xyPixelStep);
                var xyStep = _myPixelManager.GetDeltaMathsCoord(pixelStep);

                // Start stopwatch - used to measure performance improvements
                // (from improving the efficiency of the maths implementation).
                var sw = new Stopwatch();
                sw.Start();

                // Main loop, nested over Y (outer) and X (inner) values.
                var yPix = _myBitmap.Height - 1;
                for (var y = _yMin; y < _yMax; y += xyStep.Y) {
                    var xPix = 0;
                    for (var x = _xMin; x < _xMax; x += xyStep.X) {
                        // Create complex point C = x + i*y.
                        var c = new ComplexPoint(x, y);

                        // Initialise complex value Zk.
                        var zk = new ComplexPoint(0, 0);

                        // Do the main Mandelbrot calculation. Iterate until the equation
                        // converges or the maximum number of iterations is reached.
                        var k = 0;
                        // TODO: Parallelization
                        double modulusSquared;
                        do {
                            zk = zk.DoCmplxSqPlusConst(c);
                            modulusSquared = zk.DoMoulusSq();
                            k++;
                        } while (modulusSquared <= 4.0 && k < _kMax);

                        if (k < _kMax) {
                            // Max number of iterations was not reached. This means that the
                            // equation converged. Now assign a colour to the current pixel that
                            // depends on the number of iterations, k, that were done.

                            Color color;
                            if (k == kLast) {
                                // If the iteration count is the same as the last count, re-use the
                                // last pen. This avoids re-calculating colour factors which is
                                // computationally intensive. We benefit from this often because
                                // adjacent pixels are often the same colour, especially in large parts
                                // of the Mandelbrot set that are away from the areas of detail.
                                color = colorLast;
                            } else {
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

                            // Draw single pixel
                            if (xyPixelStep == 1) {
                                // Pixel step is 1, set a single pixel.
                                if (xPix < _myBitmap.Width && yPix >= 0) {
                                    _myBitmap.SetPixel(xPix, yPix, color);
                                }
                            } else {
                                // Pixel step is > 1, set a square of pixels.
                                Parallel.For(0, xyPixelStep,
                                    new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}, step =>
                                    {
                                        for (var pY = 0; pY < xyPixelStep; pY++)
                                        {
                                            if (xPix + step < _myBitmap.Width && yPix - pY >= 0)
                                            {
                                                _myBitmap.SetPixel(xPix + step, yPix - pY, color);
                                            }
                                        }
                                    });
                            }
                        }
                        xPix += xyPixelStep;
                    }
                    yPix -= xyPixelStep;
                }
                // Finished rendering. Stop the stopwatch and show the elapsed time.
                sw.Stop();
                Refresh();
                stopwatchLabel.Text = Convert.ToString(sw.Elapsed.TotalSeconds);
                statusLabel.Text = "Status: Render complete";

                // Save current settings to undo file.
                var writer = new StreamWriter(@"C:\Users\" + _userName + "\\mandelbrot_config\\Undo\\undo" + _undoNum + ".txt");
                writer.Write(pixelStepTextBox.Text + Environment.NewLine + iterationCountTextBox.Text + Environment.NewLine + yMinCheckBox.Text + Environment.NewLine + yMaxCheckBox.Text + Environment.NewLine + xMinCheckBox.Text + Environment.NewLine + xMaxCheckBox.Text);
                writer.Close();
                writer.Dispose();
            } catch (Exception e2) {
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
        private static Color ColorFromHsla(double h, double s, double l) {
            var r = l;
            var g = l;
            var b = l;

            // Standard HSL to RGB conversion. This is described in
            // detail at:
            // http://www.niwa.nu/2013/05/math-behind-colorspace-conversions-rgb-hsl/
            var v = l <= 0.5 ? l * (1.0 + s) : l + s - l * s;

            if (v > 0) {
                var m = l + l - v;
                var sv = (v - m) / v;
                h *= 6.0;
                var sextant = (int)h;
                var fract = h - sextant;
                var vsf = v * sv * fract;
                var mid1 = m + vsf;
                var mid2 = v - vsf;

                switch (sextant) {
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
        private void MouseClickOnForm(object sender, MouseEventArgs e) {
            if (!zoomCheckbox.Checked) return;
            var x = Convert.ToDouble(e.X);
            _xValue = x;
            var y = Convert.ToDouble(e.Y);
            _yValue = y;

            try {
                _zoomScale = Convert.ToInt16(zoomTextBox.Text);
            } catch(Exception c) {
                MessageBox.Show("Error: " + c.Message, "Error");
            }
            // Zoom scale has to be above 0, or their is no point in zooming.
            if (_zoomScale < 1) {
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
        private void MouseUpOnForm(object sender, MouseEventArgs e) {
            if (!zoomCheckbox.Checked) return;

            var pixelCoord = new ComplexPoint((int)(_xValue + 1005 / _zoomScale / 4), (int)(_yValue + 691 / _zoomScale / 4));//
            _zoomCoord2 = _myPixelManager.GetAbsoluteMathsCoord(pixelCoord);

            // Swap to ensure that zoomCoord1 stores the lower-left
            // coordinate for the zoom region, and zoomCoord2 stores the
            // upper right coordinate.
            if (_zoomCoord2.X < _zoomCoord1.X) {
                var temp = _zoomCoord1.X;
                _zoomCoord1.X = _zoomCoord2.X;
                _zoomCoord2.X = temp;
            }
            if (_zoomCoord2.Y < _zoomCoord1.Y) {
                var temp = _zoomCoord1.Y;
                _zoomCoord1.Y = _zoomCoord2.Y;
                _zoomCoord2.Y = temp;
            }
            yMinCheckBox.Text = Convert.ToString(_zoomCoord1.Y);
            yMaxCheckBox.Text = Convert.ToString(_zoomCoord2.Y);
            xMinCheckBox.Text = Convert.ToString(_zoomCoord1.X);
            xMaxCheckBox.Text = Convert.ToString(_zoomCoord2.X);
            RenderImage();
        }

        /// <summary>
        /// This will prompt for a favourite name, and then save the current
        /// settings to a text file so they can be used again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToFavourites_Click(object sender, EventArgs e) {
            var promptValue = PromptForNewFavourite.ShowDialog("Name", "New Favourite");
            
            var writer = new StreamWriter(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\" + promptValue + ".txt");
            writer.Write(pixelStepTextBox.Text + Environment.NewLine + iterationCountTextBox.Text + Environment.NewLine + yMinCheckBox.Text + Environment.NewLine + yMaxCheckBox.Text + Environment.NewLine + xMinCheckBox.Text + Environment.NewLine + xMaxCheckBox.Text);
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
        private void OpenFavourites_Click(object sender, EventArgs e) {
            var fileContent = File.ReadAllText(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\" + favouritesComboBox.SelectedItem + ".txt");
            var array = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);

            pixelStepTextBox.Text = array[0];
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
        private void FavouritesComboBox_DropDown(object sender, EventArgs e) {
            var dinfo = new DirectoryInfo(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\");
            var files = dinfo.GetFiles("*.txt");
            foreach (var file in files) {
                var name = file.Name.Substring(0, file.Name.LastIndexOf(".txt", StringComparison.OrdinalIgnoreCase));
                if (name.Equals("")) {
                    File.Delete(@"C:\Users\" + _userName + "\\mandelbrot_config\\Fav\\.txt");
                    favouritesComboBox.Items.Remove(name);
                }
            }
        }

        /// <summary>
        /// When the undo button is clicked, the last settings are read from
        /// the last undo text file, and the text boxes are set to these settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Undo_Click(object sender, EventArgs e) {
            try {
                var fileContent = File.ReadAllText(@"C:\Users\" + _userName + "\\mandelbrot_config\\Undo\\undo" + (_undoNum -= 1) + ".txt");
                var array1 = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);

                pixelStepTextBox.Text = array1[0];
                iterationCountTextBox.Text = array1[1];
                yMinCheckBox.Text = array1[2];
                yMaxCheckBox.Text = array1[3];
                xMinCheckBox.Text = array1[4];
                xMaxCheckBox.Text = array1[5];
            } catch (Exception e3) {
                MessageBox.Show("Unable to Undo: " + e3.Message, "Error");
            }
        }

        /// <summary>
        /// Class used for colour lookup table.
        /// </summary>
        private class ColourTable {
            public readonly int KMax;
            public readonly int NColour;
            private readonly Color[] _colourTable;

            /// <summary>
            /// Constructor. Creates lookup table.
            /// </summary>
            /// <param name="n"></param>
            /// <param name="kMax"></param>
            public ColourTable(int n, int kMax) {
                NColour = n;
                this.KMax = kMax;
                _colourTable = new Color[NColour];

                Parallel.For(0, NColour, step =>
                {
                    var colourIndex = (double)step / NColour;
                    var hue = Math.Pow(colourIndex, 0.30);
                    _colourTable[step] = ColorFromHsla(hue, 0.9, 0.6);
                });
            }

            /// <summary>
            /// Retrieve the colour from iteration count k.
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            public Color GetColour(int k) {
                return _colourTable[k];
            } 
        }

        // Mandelbrot_Paint handler to draw the image.
        private void Mandelbrot_Paint(object sender, PaintEventArgs e) {
            var graphicsObj = e.Graphics;
            graphicsObj.DrawImage(_myBitmap, 0, 0, _myBitmap.Width, _myBitmap.Height);
            graphicsObj.Dispose();
        }

        // Button used to save bitmap at desired location. File type is defaulted as Portable Network Graphics.
        private void SaveImageButton_Click(object sender, EventArgs e) {
            _myBitmap.Save(@"C:\Users\" + _userName + "\\mandelbrot_config\\Images\\" + saveImageTextBox.Text + ".png");
        }
    }
}
