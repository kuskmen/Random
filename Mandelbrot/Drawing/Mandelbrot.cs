namespace Drawing
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;

   public partial class Mandelbrot : Form
    {
        public Mandelbrot()
        {
            InitializeComponent();
        }

        private MandelbrotCoordinates _mandelbrotCoordinates = MandelbrotCoordinates.Default;

        /// <summary>
        /// Load the main form for this application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            RenderMandelbrot(int.Parse(iterationCountTextBox.Text));
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

        private void Mandelbrot_MouseClick(object sender, MouseEventArgs args)
        {
            if((args.Button & MouseButtons.Left) != 0) {
                Zoom(.5, args);
                return;
            }

            Zoom(2d, args);
        }

        private void Zoom(double factor, MouseEventArgs e)
        {
            // Center the image on the selected location
            _mandelbrotCoordinates.CenterX += ((e.X - (mandelbrotPb.Width / 2.0)) / mandelbrotPb.Width) * _mandelbrotCoordinates.Width;
            _mandelbrotCoordinates.CenterY += ((e.Y - (mandelbrotPb.Height / 2.0)) / mandelbrotPb.Height) * _mandelbrotCoordinates.Height;

            _mandelbrotCoordinates.Width *= factor;
            _mandelbrotCoordinates.Height *= factor;

            // Update the image
            RenderMandelbrot(int.Parse(iterationCountTextBox.Text));
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

            public double Width { get; set; }
            public double Height { get; set; }
            public double CenterX { get; set; }
            public double CenterY { get; set; }
        }
    }
}
