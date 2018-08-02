using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
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
        private Graphics _g;                                         // Graphics object: all graphical rendering is done on this object.
        private LockedBitmap _myBitmap;                              // Bitmap used to draw images.
        private readonly Stack<UndoInfo> _undoHistory = new Stack<UndoInfo>();
        private readonly IDictionary<string, object> _cache = new Dictionary<string, object>();

        /// <summary>
        /// Load the main form for this application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Create graphics object for Mandelbrot rendering.
            _myBitmap = new LockedBitmap(ClientRectangle.Width,
                                  ClientRectangle.Height,
                                  System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            _g = Graphics.FromImage(_myBitmap.Bitmap);
            // Set the background of the form to white.
            _g.Clear(Color.White);

            // Hide controls that are not relevant until the first rendering has completed.
            undoButton.Hide();
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
            if (!_cache.TryGetValue(Keys.ColorTable, out var colorTable) || iterations != Convert.ToInt32(previousIterations))
            {
                colorTable = new ColorTable();
                _cache.AddOrUpdate(Keys.ColorTable, colorTable);
            }

            RenderImage(iterations);
        }

        private void RenderImage(int iterations)
        {
            undoButton.Show();

            // Get the x, y range (mathematical coordinates) to plot.
            var yMin = Convert.ToDouble(yMinCheckBox.Text);
            var yMax = Convert.ToDouble(yMaxCheckBox.Text);
            var xMin = Convert.ToDouble(xMinCheckBox.Text);
            var xMax = Convert.ToDouble(xMaxCheckBox.Text);

            // Clear any existing graphics content.
            _g.Clear(Color.White);

            // Create pixel manager. This sets up the scaling factors used when
            // converting from mathemathical to screen (pixel units) using the
            _myPixelManager = new ScreenPixelManage(_g, new Complex(xMin, yMin), new Complex(xMax, yMax));

            // The pixel step size defines the increment in screen pixels for each point
            // at which the Mandelbrot calcualtion will be done.
            // This increment is converted to mathematical coordinates.
            var xyStep = _myPixelManager.GetDeltaMathsCoord(new Complex(1, 1));

            var sw = Stopwatch.StartNew();

            // Main loop, nested over Imaginary (outer) and Real (inner) values.
            var yPix = _myBitmap.Height - 1;
            for (var y = yMin; y < yMax; y += xyStep.Imaginary)
            {
                var xPix = 0;
                for (var x = xMin; x < xMax; x += xyStep.Real)
                {

                    var c = new Complex(x, y);
                    var z = c;

                    var k = 0;
                    for (var iteration = 0; iteration < iterations; iteration++)
                    {
                        if (z.Magnitude > 4.0)
                        {
                            k = iteration;
                            break;
                        }

                        z = z * z + c;
                    }

                    if (k < iterations)
                    {
                        if (xPix < _myBitmap.Width && yPix >= 0)
                        {
                            var color = ((ColorTable)_cache[Keys.ColorTable]).GetColour(k);
                            _myBitmap.SetPixel(xPix, yPix, color);
                        }
                    }

                    xPix++;
                }

                yPix--;
            }

            sw.Stop();
            Refresh();
            stopwatchLabel.Text = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            statusLabel.Text = "Status: Render complete";

            _undoHistory.Push(new UndoInfo
            {
                IterationCount = iterationCountTextBox.Text,
                Ymin = yMinCheckBox.Text,
                Ymax = yMaxCheckBox.Text,
                Xmin = xMinCheckBox.Text,
                Xmax = xMaxCheckBox.Text
            });
        }

        private void RenderImageParallel(int iterations)
        {
            undoButton.Show();

            // Get the x, y range (mathematical coordinates) to plot.
            var yMin = Convert.ToDouble(yMinCheckBox.Text);
            var yMax = Convert.ToDouble(yMaxCheckBox.Text);
            var xMin = Convert.ToDouble(xMinCheckBox.Text);
            var xMax = Convert.ToDouble(xMaxCheckBox.Text);

            // Clear any existing graphics content.
            _g.Clear(Color.White);

            // Create pixel manager. This sets up the scaling factors used when
            // converting from mathemathical to screen (pixel units) using the
            _myPixelManager = new ScreenPixelManage(_g, new Complex(xMin, yMin), new Complex(xMax, yMax));

            // The pixel step size defines the increment in screen pixels for each point
            // at which the Mandelbrot calcualtion will be done.
            // This increment is converted to mathematical coordinates.
            var xyStep = _myPixelManager.GetDeltaMathsCoord(new Complex(1, 1));

            var sw = Stopwatch.StartNew();

            // Main loop, nested over Imaginary (outer) and Real (inner) values.
            var yPix = _myBitmap.Height - 1;

            Parallel.ForEach(Iterate(yMin, yMax, xyStep.Imaginary), y =>
             {
                 var xPix = 0;
                 for (var x = xMin; x < xMax; x += xyStep.Real)
                 {
                     // Create complex point C = x + i*y.
                     var c = new Complex(x, y);

                     // Initialise complex value Zk.
                     var z = c;

                     var k = 0;
                     do
                     {
                         k++;
                         z = z * z + c;
                     } while (z.Magnitude <= 4.0 && k < iterations);

                     if (k < iterations)
                     {
                         if (xPix < _myBitmap.Width && yPix >= 0)
                         {
                             var color = ((ColorTable)_cache[Keys.ColorTable]).GetColour(k);
                             _myBitmap.SetPixel(xPix, yPix, color);
                         }
                     }

                     xPix++;
                 }

                 yPix--;
             });

            sw.Stop();
            Refresh();
            stopwatchLabel.Text = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            statusLabel.Text = "Status: Render complete";

            _undoHistory.Push(new UndoInfo
            {
                IterationCount = iterationCountTextBox.Text,
                Ymin = yMinCheckBox.Text,
                Ymax = yMaxCheckBox.Text,
                Xmin = xMinCheckBox.Text,
                Xmax = xMaxCheckBox.Text
            });

            IEnumerable<double> Iterate(double fromInclusive, double toExclusive, double step)
            {
                for (var x = fromInclusive; x < toExclusive; x += step) yield return x;
            }
        }

        /// <summary>
        /// Mouse-up handler for main form. The coordinates of the rectangle are
        /// saved so the new drawing can be rendered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseUpOnForm(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(zoomTextBox.Text)) return;

            var zoomScale = 0;
            try
            {
                zoomScale = Convert.ToInt16(zoomTextBox.Text);
            }
            catch (Exception c)
            {
                MessageBox.Show("Error: " + c.Message, "Error");
            }

            // Zoom scale has to be above 0, or their is no point in zooming.
            if (zoomScale < 1)
            {
                MessageBox.Show("Zoom scale must be above 0");
                zoomTextBox.Text = "1";
                return;
            }

            var x = Convert.ToDouble(e.X);
            var y = Convert.ToDouble(e.Y);

            var pixelCoord = new Complex((int)(x - 1005d / zoomScale / 4), (int)(y - 691d / zoomScale / 4));
            var zoomCoord1 = _myPixelManager.GetAbsoluteMathsCoord(pixelCoord);
            var pixelCoord2 = new Complex((int)(x + 1005d / zoomScale / 4), (int)(y + 691d / zoomScale / 4));
            var zoomCoord2 = _myPixelManager.GetAbsoluteMathsCoord(pixelCoord2);

            // Swap to ensure that zoomCoord1 stores the lower-left
            // coordinate for the zoom region, and zoomCoord2 stores the
            // upper right coordinate.
            if (zoomCoord2.Real < zoomCoord1.Real)
            {
                var temp = zoomCoord1.Real;
               // zoomCoord1.Real = zoomCoord2.Real;
               // zoomCoord2.Real = temp;
            }
            if (zoomCoord2.Imaginary < zoomCoord1.Imaginary)
            {
                var temp = zoomCoord1.Imaginary;
            //    zoomCoord1.Imaginary = zoomCoord2.Imaginary;
            //    zoomCoord2.Imaginary = temp;
            }

            yMinCheckBox.Text = zoomCoord1.Imaginary.ToString(CultureInfo.InvariantCulture);
            yMaxCheckBox.Text = zoomCoord2.Imaginary.ToString(CultureInfo.InvariantCulture);
            xMinCheckBox.Text = zoomCoord1.Real.ToString(CultureInfo.InvariantCulture);
            xMaxCheckBox.Text = zoomCoord2.Real.ToString(CultureInfo.InvariantCulture);

            RenderImage(Convert.ToInt32(iterationCountTextBox.Text));
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
                _undoHistory.Pop();
                var undoInfo = _undoHistory.Pop();

                iterationCountTextBox.Text = undoInfo.IterationCount;
                yMinCheckBox.Text = undoInfo.Ymin;
                yMaxCheckBox.Text = undoInfo.Ymax;
                xMinCheckBox.Text = undoInfo.Xmin;
                xMaxCheckBox.Text = undoInfo.Xmax;

                RenderImage(Convert.ToInt32(iterationCountTextBox.Text));
            }
            catch (Exception e3)
            {
                MessageBox.Show("Unable to Undo: " + e3.Message, "Error");
            }
        }

        // Mandelbrot_Paint handler to draw the image.
        private void Mandelbrot_Paint(object sender, PaintEventArgs e)
        {
            var graphicsObj = e.Graphics;
            graphicsObj.DrawImage(_myBitmap.Bitmap, 0, 0, _myBitmap.Width, _myBitmap.Height);
            graphicsObj.Dispose();
        }
    }
}
