using Microsoft.Research.Kinect.Nui;
namespace Coding4Fun.Kinect.Wpf.Geometry
{
    public struct Triangle
    {
        public Vector V1;
        public Vector V2;
        public Vector V3;

        public Triangle(Vector v1, Vector v2, Vector v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }

        public double ContainsInCircumcircle(Vector point)
        {
            var ax = V1.X - point.X;
            var ay = V1.Y - point.Y;
            var bx = V2.X - point.X;
            var by = V2.Y - point.Y;
            var cx = V3.X - point.X;
            var cy = V3.Y - point.Y;

            var detAb = ax*by - bx*ay;
            var detBc = bx*cy - cx*by;
            var detCa = cx*ay - ax*cy;

            var aSquared = ax*ax + ay*ay;
            var bSquared = bx*bx + by*by;
            var cSquared = cx*cx + cy*cy;

            return aSquared*detBc + bSquared*detCa + cSquared*detAb;
        }

        public bool SharesVertexWith(Triangle triangle)
        {
            if (V1.X == triangle.V1.X && V1.Y == triangle.V1.Y) return true;
            if (V1.X == triangle.V2.X && V1.Y == triangle.V2.Y) return true;
            if (V1.X == triangle.V3.X && V1.Y == triangle.V3.Y) return true;

            if (V2.X == triangle.V1.X && V2.Y == triangle.V1.Y) return true;
            if (V2.X == triangle.V2.X && V2.Y == triangle.V2.Y) return true;
            if (V2.X == triangle.V3.X && V2.Y == triangle.V3.Y) return true;

            if (V3.X == triangle.V1.X && V3.Y == triangle.V1.Y) return true;
            if (V3.X == triangle.V2.X && V3.Y == triangle.V2.Y) return true;
            if (V3.X == triangle.V3.X && V3.Y == triangle.V3.Y) return true;

            return false;
        }
    }
}