using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Samples.Kinect.WpfViewers;
using Coding4Fun.Kinect.Wpf;

namespace Kinect_MoCap
{
    public partial class MainWindow : Window
    {
        //Instantiate the Kinect runtime. Required to initialize the device.
        //IMPORTANT NOTE: You can pass the device ID here, in case more than one Kinect device is connected.
        Runtime runtime = new Runtime();

        public MainWindow()
        {
            InitializeComponent();

            //Runtime initialization is handled when the window is opened. When the window
            //is closed, the runtime MUST be unitialized.
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);

            //Handle the content obtained from the video camera, once received.
            runtime.VideoFrameReady += new EventHandler<Microsoft.Research.Kinect.Nui.ImageFrameReadyEventArgs>(runtime_VideoFrameReady);

            runtime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(runtime_SkeletonFrameReady);
        }

        // Keep track of the frame rate using the total frame, last frames, and time
        int totalFrames = 0;
        int lastFrames = 0;
        DateTime lastTime = DateTime.MaxValue;

        // Depth data stored as a byte array.  Color Indexes are used
        const int RED_IDX = 2;
        const int GREEN_IDX = 1;
        const int BLUE_IDX = 0;
        byte[] depthFrame32 = new byte[320 * 240 * 4];

        // Dictionary to store corresponding colors for the joints
        Dictionary<JointID, Brush> jointColors =
            new Dictionary<JointID, Brush>()
            {
                // FILL IN WITH ALL JOINTS AND COLORS
                {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
                {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
                {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
                {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0, 0))},
                {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79, 84, 33))},
                {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84, 33, 42))},
                {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
                {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215, 86, 0))},
                {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33, 79, 84))},
                {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33, 33, 84))},
                {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
                {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37, 69, 243))},
                {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
                {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69, 33, 84))},
                {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
                {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
                {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
                {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222, 76))},
                {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
                {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
            };

        


        //This method is used to position the ellipses on the canvas
        //according to correct movements of the tracked joints.

        //IMPORTANT NOTE: Code for vector scaling was imported from the Coding4Fun Kinect Toolkit
        //available here: http://c4fkinect.codeplex.com/
        //I only used this part to avoid adding an extra reference.
        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            Microsoft.Research.Kinect.Nui.Vector vector = new Microsoft.Research.Kinect.Nui.Vector();
            vector.X = ScaleVector(640, joint.Position.X);
            vector.Y = ScaleVector(480, -joint.Position.Y);
            vector.Z = joint.Position.Z;

            Joint updatedJoint = new Joint();
            updatedJoint.ID = joint.ID;
            updatedJoint.TrackingState = JointTrackingState.Tracked;
            updatedJoint.Position = vector;

            Canvas.SetLeft(ellipse, updatedJoint.Position.X);
            Canvas.SetTop(ellipse, updatedJoint.Position.Y);
        }

        private float ScaleVector(int length, float position)
        {
            float value = (((((float)length) / 1f) / 2f) * position) + (length / 2);
            if (value > length)
            {
                return (float)length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            runtime.Uninitialize();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                //Since only a color video stream is needed, RuntimeOptions.UseColor is used.
                runtime.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseColor | RuntimeOptions.UseSkeletalTracking);
            }
            catch(InvalidOperationException)
            {
                // Display Error
                System.Windows.MessageBox.Show("Cannot Initialize Kinect.  Please Make Sure Kinect is Kinected");
                return;

            }

            try
            {
                //You can adjust the resolution here.
                runtime.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                runtime.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            }
            catch (InvalidOperationException)
            {
                // Display Error
                
                return;
            }
        }

        void runtime_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
        }

        void runtime_VideoFrameReady(object sender, Microsoft.Research.Kinect.Nui.ImageFrameReadyEventArgs e)
        {
            videoImage.Source = e.ImageFrame.ToBitmapSource();
        }

        void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            int iSkeleton = 0;
            Brush[] brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

            skeleton.Children.Clear();
           

        }

        private Point getPointPosition(Joint joint)
        {
            float depthX, depthY;
            runtime.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = depthX * 320;
            depthY = depthY * 240;
            int colorX, colorY;
            ImageViewArea iv = new ImageViewArea();
            runtime.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

            // map back to skeleton.Width & skeleton.Height
            return new Point((int)(skeleton.Width * colorX / 640.0), (int)(skeleton.Height * colorY / 480));
        }

        Polyline getBoneSegment(Microsoft.Research.Kinect.Nui.JointsCollection joints, Brush brush, params JointID[] ids)
        {
            PointCollection points = new PointCollection(ids.Length);
            for (int i = 0; i < ids.Length; ++i)
            {
                points.Add(getPointPosition(joints[ids[i]]));
            }

            Polyline polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }
    }
}
