using System.Drawing;

namespace Drawing {

    /// <summary>
    /// ScreenPixelManage class. Handles the conversions between mathematical
    /// units and physical screen coordinates, i.e. pixel coordinates. The
    /// underlying mathematical coordinates are independent of screen resolution
    /// and size, whereas pixel coordinates are applicable to run-time screen
    /// dimensions.
    /// </summary>
    public class ScreenPixelManage {
        private readonly double _convConstX1;
        private readonly double _convConstX2;
        private readonly double _convConstY1;
        private readonly double _convConstY2;

        public ScreenPixelManage(Graphics graphics, ComplexPoint screenBottomLeftCorner, ComplexPoint screenTopRightCorner) {
            
            // Transform from mathematical to pixel coordinates.
            //
            // The following are long-handed calulations, now replaced with more efficient calculations
            // using convConst** values.
            //       this.xPixel = (int) ((graphics.VisibleClipBounds.Size.Width) / (screenTopRightCorner.x - screenBottomLeftCorner.x) * (cmplxPoint.x - screenBottomLeftCorner.x));
            //       this.yPixel = (int) (graphics.VisibleClipBounds.Size.Height - graphics.VisibleClipBounds.Size.Height / (screenTopRightCorner.y - screenBottomLeftCorner.y) * (cmplxPoint.y - screenBottomLeftCorner.y));

            _convConstX1 = graphics.VisibleClipBounds.Size.Width / (screenTopRightCorner.Real - screenBottomLeftCorner.Real);
            _convConstX2 = _convConstX1 * screenBottomLeftCorner.Real;

            _convConstY1 = graphics.VisibleClipBounds.Size.Height * (1.0 + screenBottomLeftCorner.Imaginary / (screenTopRightCorner.Imaginary - screenBottomLeftCorner.Imaginary));
            _convConstY2 = graphics.VisibleClipBounds.Size.Height / (screenTopRightCorner.Imaginary - screenBottomLeftCorner.Imaginary);
        }

        /// <summary>
        /// Converts a pixel-coordinate increment (small change in X, Y
        /// screen coordiante) to the corresponding increment in mathematical
        /// coordinates. This is used, for example, when drawing the Mandlebrot
        /// set with chosen X, Y pixel steps, for which the cooresponding
        /// mathematical steps need to be known.
        /// 
        /// This is done using the transformation functions that convert
        /// from maths coordinates to pixel coordinates. If these are 
        /// respectively:
        /// 
        /// Fx() and Fy() for the x and y domains, then to convert in the
        /// opposite direction, from pixels to maths coordinates we need
        /// to use:
        /// 
        /// dFx()/dx and dFy()/dy (which are the derivates of each function),
        /// then multiply each by the corresponding pixel increments in
        /// either x or y.
        /// 
        /// This implementation uses pre-calculated constant scale factors
        /// for an efficient implementation.
        /// 
        /// </summary>
        /// <param name="pixelCoord">Screen coordinate</param>
        /// <returns></returns>
        public ComplexPoint GetDeltaMathsCoord(ComplexPoint pixelCoord) {
            var result = new ComplexPoint(
                   pixelCoord.Real / _convConstX1,
                   pixelCoord.Imaginary / _convConstY2);
            return result;
        }

        /// <summary>
        /// Get absolute maths coordinate from pixel coordinate. This is effectively
        /// an inverse calcuate: given a pixel screen coordinate it returns the
        /// corresponding mathematical point.
        /// </summary>
        /// <param name="pixelCoord">Screen coordinate</param>
        /// <returns>Mathematical point corresponding to pixelCoord</returns>
        public ComplexPoint GetAbsoluteMathsCoord(ComplexPoint pixelCoord) {
            var result = new ComplexPoint(
                   (_convConstX2 + pixelCoord.Real) / _convConstX1,
                   (_convConstY1 - pixelCoord.Imaginary) / _convConstY2);
            return result;
        }
    }
}
