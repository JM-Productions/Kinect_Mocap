using Microsoft.Research.Kinect.Nui;

namespace Coding4Fun.Kinect.Wpf.Geometry
{
    public struct Edge
    {
        public readonly Vector EndPoint;
        public readonly Vector StartPoint;

        public Edge(Vector startPoint, Vector endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public override int GetHashCode()
        {
            return StartPoint.GetHashCode() ^ EndPoint.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this == (Edge) obj;
        }

        public static bool operator ==(Edge left, Edge right)
        {
            if (((object)(left) == ((object) right)))
            {
                return true;
            }

            if (((object) left) == null || ((object) right) == null)
            {
                return false;
            }

            return (((left.StartPoint.X == right.StartPoint.X &&  left.StartPoint.Y == right.StartPoint.Y && left.StartPoint.Z == right.StartPoint.Z)
                    && (left.EndPoint.X == right.EndPoint.X &&  left.EndPoint.Y == right.EndPoint.Y && left.EndPoint.Z == right.EndPoint.Z)) 
                ||
                    ((left.StartPoint.X == right.EndPoint.X && left.StartPoint.Y == right.EndPoint.Y && left.StartPoint.Z == right.EndPoint.Z)
                    && (left.EndPoint.X == right.StartPoint.X &&  left.EndPoint.Y == right.StartPoint.Y && left.EndPoint.Z == right.StartPoint.Z)));
        }

        public static bool operator !=(Edge left, Edge right)
        {
            return !(left == right);
        }
    }
}