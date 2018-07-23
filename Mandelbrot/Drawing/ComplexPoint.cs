namespace Drawing {
    /// <summary>
    /// ComplexPoint class is used encapsulate a single complex point
    /// Z = x + i*y where x and y are the real and imaginary parts respectively.
    /// A number of complex arithmetic utility methods are provided.
    /// </summary>
    public struct ComplexPoint {
        public double X;
        public double Y;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">real part of complex point</param>
        /// <param name="y">imaginary part of complex point</param>
        public ComplexPoint(double x, double y) {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Calculate modulus squared |Z|**2 = X*x + y*y.
        /// </summary>
        /// <returns>Modulus squared</returns>
        public double DoMoulusSq() {
            return X * X + Y * Y;
        }

        /// <summary>
        /// Calculate complex square plus complex constant. The result
        /// is another complex number.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Z**2 + arg</returns>
        public ComplexPoint DoCmplxSqPlusConst(ComplexPoint arg) {
            var result = new ComplexPoint(0, 0)
            {
                X = X * X - Y * Y,
                Y = 2 * X * Y
            };
            result.X += arg.X;
            result.Y += arg.Y;
            return result;
        }
    }
}
