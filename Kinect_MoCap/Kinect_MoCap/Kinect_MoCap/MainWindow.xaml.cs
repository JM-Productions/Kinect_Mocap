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
            };

        void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonSet = e.SkeletonFrame;

            SkeletonData data = (from s in skeletonSet.Skeletons
                                 where s.TrackingState == SkeletonTrackingState.Tracked
                                 select s).FirstOrDefault();

            SetEllipsePosition(head, data.Joints[JointID.Head]);
            SetEllipsePosition(leftHand, data.Joints[JointID.HandLeft]);
            SetEllipsePosition(rightHand, data.Joints[JointID.HandRight]);
        }


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
            //Since only a color video stream is needed, RuntimeOptions.UseColor is used.
            runtime.Initialize(Microsoft.Research.Kinect.Nui.RuntimeOptions.UseColor | RuntimeOptions.UseSkeletalTracking);

            //You can adjust the resolution here.
            runtime.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
        }

        void runtime_VideoFrameReady(object sender, Microsoft.Research.Kinect.Nui.ImageFrameReadyEventArgs e)
        {
            PlanarImage image = e.ImageFrame.Image;

            BitmapSource source = BitmapSource.Create(image.Width, image.Height, 96, 96,
                PixelFormats.Bgr32, null, image.Bits, image.Width * image.BytesPerPixel);
            videoImage.Source = source;
        }
    }
}
