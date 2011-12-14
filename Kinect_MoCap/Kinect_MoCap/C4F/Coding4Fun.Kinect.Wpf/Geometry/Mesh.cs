using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace Coding4Fun.Kinect.Wpf.Geometry
{
    public struct Mesh
    {
        public Triangle[] Triangles;
        public Vector[] Points;

        public Mesh(Triangle[] triangles, Vector[] points)
        {
            Triangles = triangles;
            Points = points;
        }
    }
}
