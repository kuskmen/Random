namespace Mandelbrot
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

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
            RenderMandelbrot(int.Parse(iterationsTb.Text));
        }

        /// <summary>
        /// On-click handler for generate button. Triggers rendering of the Mandelbrot
        /// set using current configuration settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenderMandelbrot(object sender, EventArgs e)
        {
            RenderMandelbrot(int.Parse(iterationsTb.Text));
        }

        private void RenderMandelbrot(int iterations)
        {
            // For diagnostic reasons, time how long the rendering takes
            var stopWatch = Stopwatch.StartNew();

            // Render the fractal
            var bmp = MandelbrotRenderer.Create(_mandelbrotCoordinates, mandelbrotPb.Size, iterations, int.Parse(parallelismTb.Text));
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
            _mandelbrotCoordinates.CenterX += (e.X - mandelbrotPb.Width / 2.0) / mandelbrotPb.Width * _mandelbrotCoordinates.Width;
            _mandelbrotCoordinates.CenterY += (e.Y - mandelbrotPb.Height / 2.0) / mandelbrotPb.Height * _mandelbrotCoordinates.Height;

            _mandelbrotCoordinates.Width *= factor;
            _mandelbrotCoordinates.Height *= factor;

            // Update the image
            RenderMandelbrot(int.Parse(iterationsTb.Text));
        }

        public struct MandelbrotCoordinates
        {
            public static MandelbrotCoordinates Default =>
                new MandelbrotCoordinates
                {
                    Width = 4.5,
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
