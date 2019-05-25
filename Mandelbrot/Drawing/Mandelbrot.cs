namespace Mandelbrot
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    /// <inheritdoc />
    /// <summary>
    /// Type responsible for the back end of the <see cref="T:System.Windows.Forms.Form" />. 
    /// </summary>
    public partial class Mandelbrot : Form
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes new instance of <see cref="T:Mandelbrot.Mandelbrot" /> type.
        /// </summary>
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
        /// <param name="sender"> Sender of the event. </param>
        /// <param name="e"> Event arguments. </param>
        private void RenderMandelbrot(object sender, EventArgs e)
        {
            RenderMandelbrot(int.Parse(iterationsTb.Text));
        }

        /// <summary>
        ///  Renders mandelbrot into the <see cref="mandelbrotPb"/>
        /// </summary>
        /// <param name="iterations"></param>
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

        /// <summary>
        ///  Event handler for mouse click on the mandelbrot picture.
        ///  Left mouse click zooms, while right click zooms out.
        /// </summary>
        /// <param name="sender"> Sender of the event. </param>
        /// <param name="args"> Event arguments used to determine which mouse button was clicked. </param>
        private void Mandelbrot_MouseClick(object sender, MouseEventArgs args)
        {
            if((args.Button & MouseButtons.Left) != 0) {
                Zoom(.5, args);
                return;
            }

            Zoom(2d, args);
        }

        /// <summary>
        ///  Zooms Mandelbrot picture by given factor.
        /// </summary>
        /// <param name="factor"> Factor that the mandelbrot will be zoomed with. </param>
        /// <param name="e"> Event arguments used for determing where the new zoomed mandelbrot will be. </param>
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

        /// <summary>
        ///  Type responsible for mandelbrot coordinates at the screen.
        /// </summary>
        public struct MandelbrotCoordinates
        {
            /// <summary>
            ///  Default positioning of the mandelbrot fractal.
            /// </summary>
            public static MandelbrotCoordinates Default =>
                new MandelbrotCoordinates
                {
                    Width = 4.5,
                    Height = 2.27,
                    CenterX = -.75,
                    CenterY = .006
                };
            
#pragma warning disable 1591
            public double Width { get; set; }

            public double Height { get; set; }

            public double CenterX { get; set; }

            public double CenterY { get; set; }
#pragma warning restore 1591
        }
    }
}
