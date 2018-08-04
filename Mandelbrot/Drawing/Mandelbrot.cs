using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

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

        private readonly Stack<UndoInfo> _undoHistory = new Stack<UndoInfo>();
        private readonly MandelbrotCoordinates _mandelbrotCoordinates = MandelbrotCoordinates.Default;

        /// <summary>
        /// Load the main form for this application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
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
            RenderMandelbrot(int.Parse(iterationCountTextBox.Text));
        }

        private void RenderMandelbrot(int iterations)
        {
            // Get the current size of the picture box
            var renderSize = mandelbrotPb.Size;

            // For diagnostic reasons, time how long the rendering takes
            var stopWatch = Stopwatch.StartNew();

            // Render the fractal
            var bmp = MandelbrotRenderer.Create(_mandelbrotCoordinates, renderSize.Width, renderSize.Height, CancellationToken.None, iterations);
            if (bmp != null)
            {
                stopWatch.Stop();

                // Update the fractal image asynchronously on the UI thread
                Image old = null;
                BeginInvoke((MethodInvoker)delegate
                {
                    old = mandelbrotPb.Image;
                    mandelbrotPb.Image = bmp;
                });
                old?.Dispose();
            }

            stopwatchLabel.Text = stopWatch.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
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

                RenderMandelbrot(Convert.ToInt32(iterationCountTextBox.Text));
            }
            catch (Exception e3)
            {
                MessageBox.Show("Unable to Undo: " + e3.Message, "Error");
            }
        }

        private void Mandelbrot_MouseClick(object sender, MouseEventArgs args)
        {
            if ((args.Button & MouseButtons.Left) != 0)
            {
                // TODO: Zoom
            }
            // TODO: Undo
        }

        public struct MandelbrotCoordinates
        {
            public static MandelbrotCoordinates Default =>
                new MandelbrotCoordinates
                {
                    Width = 2.9,
                    Height = 2.27,
                    CenterX = -.75,
                    CenterY = .006
                };
            public double Width, Height;
            public double CenterX, CenterY;
        }
    }
}
