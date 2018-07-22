using System.Windows.Forms;
using System.Drawing;

namespace Drawing {
    internal class Canvas : Panel {
        private static readonly Canvas MyCanvas = new Canvas();

        public Canvas() {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            MyCanvas.BackColor = System.Drawing.Color.Transparent;

        }

        public void Draw(int x, int y, MouseEventArgs e, Graphics gr) {
            var rect = new Rectangle(x, y, (int)(e.X - x), (int)(e.Y - y));
            
        }
    }
}
