namespace Drawing
{
    public struct ComplexPoint
    {
        public double Real { get; set; }
        public double Imaginary { get; set; }

        public ComplexPoint(double real, double imaginary)
        {
            this.Real = real;
            this.Imaginary = imaginary;
        }

        public static double ModulusSquared(ComplexPoint point) => point.Real * point.Real + point.Imaginary * point.Imaginary;

        public static ComplexPoint Square(ComplexPoint point)
            => new ComplexPoint(0, 0)
            {
                Real = point.Real * point.Real - point.Imaginary * point.Imaginary,
                Imaginary = 2 * point.Real * point.Imaginary
            };

        public static ComplexPoint Add(ComplexPoint right, ComplexPoint other)
        {
            right.Real += other.Real;
            right.Imaginary += other.Imaginary;
            return right;
        }
    }
}
