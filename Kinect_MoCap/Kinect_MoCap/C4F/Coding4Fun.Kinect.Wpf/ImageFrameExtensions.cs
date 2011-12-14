// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Coding4Fun.Kinect.Common;
using Coding4Fun.Kinect.Wpf.Geometry;
using Microsoft.Research.Kinect.Nui;
using Point = System.Windows.Point;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

namespace Coding4Fun.Kinect.Wpf
{
    public static class ImageFrameExtensions
    {
        //private static Single ToCentimetres(this Single s)
        //{
        //    return s * 100;
        //}

        //public static List<Vector> ToPointCloud(this ImageFrame image, Runtime nui)
        //{
        //    return image.ToPointCloud(nui, 8000);
        //}

        //public static List<Vector> ToPointCloud(this ImageFrame image, Runtime nui, int maximumDepth)
        //{
        //    if (image == null)
        //        throw new ArgumentNullException("image");

        //    var width = image.Image.Width;
        //    var height = image.Image.Height;
        //    var greyIndex = 0;

        //    var points = new List<Vector>();

        //    for (var y = 0; y < height; y++)
        //    {
        //        for (var x = 0; x < width; x++)
        //        {
        //            short depth;
        //            switch (image.Type)
        //            {
        //                case ImageType.DepthAndPlayerIndex:
        //                    depth = (short)((image.Image.Bits[greyIndex] >> 3) | (image.Image.Bits[greyIndex + 1] << 5));
        //                    if (depth <= maximumDepth)
        //                    {
        //                        points.Add(nui.SkeletonEngine.DepthImageToSkeleton(((float)x / image.Image.Width), ((float)y / image.Image.Height), (short)(depth << 3)));
        //                    }
        //                    break;
        //                case ImageType.Depth: // depth comes back mirrored
        //                    depth = (short)((image.Image.Bits[greyIndex] | image.Image.Bits[greyIndex + 1] << 8));
        //                    if (depth <= maximumDepth)
        //                    {
        //                        points.Add(nui.SkeletonEngine.DepthImageToSkeleton(((float)(width - x - 1) / image.Image.Width), ((float)y / image.Image.Height), (short)(depth << 3)));
        //                    }
        //                    break;
        //            }

        //            greyIndex += 2;
        //        }
        //    }

        //    return points.Where(p => p.X != 0 || p.Y != 0 || p.Z != 0).ToList();
        //}


        //public static void Save(this List<Vector> points, string filename)
        //{
        //    var ply = new StringBuilder();
        //    var plyPoints = new StringBuilder();

        //    ply.AppendLine("ply");
        //    ply.AppendLine("format ascii 1.0");
        //    ply.AppendLine(String.Format("element vertex {0}", points.Count));
        //    ply.AppendLine("property float x\r\n" + "property float y\r\n" + "property float z\r\n" + "end_header");

        //    foreach (var point in points)
        //    {
        //        ply.AppendLine(String.Format("{0} {1} {2}", point.X.ToCentimetres(), point.Y.ToCentimetres(), point.Z.ToCentimetres()));
        //    }

        //    using (var filestream = new StreamWriter(filename))
        //    {
        //        filestream.Write(ply);
        //    }
        //}

        //public static Mesh ToMesh(this ImageFrame image, Runtime nui)
        //{
        //    return image.ToMesh(nui, 8000);
        //}

        //public static Mesh ToMesh(this ImageFrame image, Runtime nui, int maximumDepth)
        //{
        //    var points = image.ToPointCloud(nui, maximumDepth);

        //    var triangles = Triangulate(ref points);

        //    return new Mesh(triangles.ToArray(), points.ToArray());
        //}

        //public static void Save(this Mesh mesh, string filename)
        //{
        //    var ply = new StringBuilder();
        //    var plyTriangles = new StringBuilder();
        //    var plyPoints = new StringBuilder();

        //    Dictionary<Vector, int> pindex = new Dictionary<Vector, int>();
        //    var count = 0;

        //    foreach (var point in mesh.Points)
        //    {
        //        plyPoints.AppendLine(String.Format("{0} {1} {2}", point.X.ToCentimetres(), point.Y.ToCentimetres(), point.Z.ToCentimetres()));
        //        pindex.Add(point, count);
        //        count++;
        //    }

        //    var rowCounter = 0;
        //    foreach (var t in mesh.Triangles)
        //    {
        //        plyTriangles.AppendLine(String.Format("3 {0} {1} {2}", pindex[t.V1], pindex[t.V2], pindex[t.V3]));
        //        rowCounter++;
        //    }


        //    ply.AppendLine("ply");
        //    ply.AppendLine("format ascii 1.0");
        //    ply.AppendLine(String.Format("element vertex {0}", mesh.Points.Count()));
        //    ply.AppendLine("property float x");
        //    ply.AppendLine("property float y");
        //    ply.AppendLine("property float z");
        //    ply.AppendLine(String.Format("element face {0}", rowCounter));
        //    ply.AppendLine("property list uchar int vertex_index");
        //    ply.AppendLine("end_header");

        //    ply.Append(plyPoints);
        //    ply.Append(plyTriangles.ToString());


        //    using (var filestream = new StreamWriter(filename))
        //    {
        //        filestream.Write(ply);
        //    }
        //}

        ///// <summary>
        ///// Using the Delaunay algorithm to try and build triangles
        ///// http://en.wikipedia.org/wiki/Delaunay_triangulation
        ///// </summary>
        ///// <param name="points"></param>
        ///// <returns></returns>
        //private static List<Triangle> Triangulate(ref List<Vector> points)
        //{
        //    if (points.Count < 3)
        //        throw new ArgumentException("Triangulation requires more than three verticies");

        //    var triangles = new List<Triangle>();
        //    var encompassingTriangle = EncompassingTriangle(points);
        //    triangles.Add(encompassingTriangle);

        //    foreach (var p in points)
        //    {
        //        var edges = new List<Edge>();
        //        for (var j = triangles.Count - 1; j >= 0; j--)
        //        {
        //            var t = triangles[j];
        //            if (t.ContainsInCircumcircle(p) > 0)
        //            {
        //                edges.Add(new Edge(t.V1, t.V2));
        //                edges.Add(new Edge(t.V2, t.V3));
        //                edges.Add(new Edge(t.V3, t.V1));
        //                triangles.RemoveAt(j);
        //            }
        //        }

        //        for (var j = edges.Count - 2; j >= 0; j--)
        //        {
        //            for (var k = edges.Count - 1; k >= j + 1; k--)
        //            {
        //                if (edges[j] == edges[k])
        //                {
        //                    edges.RemoveAt(k);
        //                    edges.RemoveAt(j);
        //                    k--;
        //                    continue;
        //                }
        //            }
        //        }

        //        triangles.AddRange(edges.Select(t => new Triangle(t.StartPoint, t.EndPoint, p)));
        //    }

        //    for (var i = triangles.Count - 1; i >= 0; i--)
        //    {
        //        if (triangles[i].SharesVertexWith(encompassingTriangle))
        //            triangles.RemoveAt(i);
        //    }

        //    return triangles;

        //}

        //private static Triangle EncompassingTriangle(IList<Vector> triangulationPoints)
        //{
        //    double M = triangulationPoints[0].X;

        //    for (var i = 1; i < triangulationPoints.Count; i++)
        //    {
        //        double xAbs = Math.Abs(triangulationPoints[i].X);
        //        double yAbs = Math.Abs(triangulationPoints[i].Y);
        //        if (xAbs > M) M = xAbs;
        //        if (yAbs > M) M = yAbs;
        //    }

        //    var sp1 = new Vector { X = (float)(10 * M), Y = 0, Z = 0 };
        //    var sp2 = new Vector { X = 0, Y = (float)(10 * M), Z = 0 };
        //    var sp3 = new Vector { X = (float)(-10 * M), Y = (float)(-10 * M), Z = 0 };

        //    return new Triangle(sp1, sp2, sp3);
        //}


        public static short[][] ToDepthArray2D(this ImageFrame image)
        {
            return ImageFrameCommonExtensions.ToDepthArray2D(image);
        }

        public static short[] ToDepthArray(this ImageFrame image)
        {
            return ImageFrameCommonExtensions.ToDepthArray(image);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
        public static short GetDistance(this ImageFrame image, int x, int y)
        {
            return ImageFrameCommonExtensions.GetDistance(image, x, y);
        }

        public static Point GetMidpoint(this short[] depthData, int width, int height, int startX, int startY, int endX, int endY, int minimumDistance)
        {
            double x;
            double y;
            depthData.GetMidpoint(width, height, startX, startY, endX, endY, minimumDistance, out x, out y);

            return new Point(x, y);
        }

        public static BitmapSource ToBitmapSource(this short[] depthData, int width, int height, int minimumDistance, Color highlightColor)
        {
            if (depthData != null)
            {
                var colorFrame = new byte[height * width * 4];

                for (int colorIndex = 0, depthIndex = 0; colorIndex < colorFrame.Length; colorIndex += 4, depthIndex++)
                {
                    var intensity = ImageFrameCommonExtensions.CalculateIntensityFromDepth(depthData[depthIndex]);

                    colorFrame[colorIndex + ImageFrameCommonExtensions.RedIndex] = intensity;
                    colorFrame[colorIndex + ImageFrameCommonExtensions.GreenIndex] = intensity;
                    colorFrame[colorIndex + ImageFrameCommonExtensions.BlueIndex] = intensity;

                    if (depthData[depthIndex] <= minimumDistance && depthData[depthIndex] > 0)
                    {
                        var color = Color.Multiply(highlightColor, intensity / 255f);

                        colorFrame[colorIndex + ImageFrameCommonExtensions.RedIndex] = color.R;
                        colorFrame[colorIndex + ImageFrameCommonExtensions.GreenIndex] = color.G;
                        colorFrame[colorIndex + ImageFrameCommonExtensions.BlueIndex] = color.B;
                    }
                }

                return colorFrame.ToBitmapSource(width, height);
            }

            return null;
        }

        public static BitmapSource ToBitmapSource(this ImageFrame image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            switch (image.Type)
            {
                case ImageType.Color:
                    {
                        return image.Image.Bits.ToBitmapSource(image.Image.Width, image.Image.Height);
                    }
                case ImageType.Depth:
                    {
                        return ImageFrameCommonExtensions.ConvertDepthFrameDataToBitmapData(image.Image.Bits, image.Image.Width, image.Image.Height).ToBitmapSource(image.Image.Width, image.Image.Height);
                    }
                case ImageType.DepthAndPlayerIndex:
                    {
                        return
                            ImageFrameCommonExtensions.ConvertDepthFrameDataWithSkeletonToBitmapData(image.Image.Bits, image.Image.Width, image.Image.Height).ToBitmapSource(image.Image.Width, image.Image.Height);
                    }
                default:
                    return null;
            }
        }
    }
}
