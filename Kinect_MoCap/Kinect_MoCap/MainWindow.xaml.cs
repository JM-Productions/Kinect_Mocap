/* =============================================================================
 * Copyright © 2011 ESRI
 * 
 * All rights reserved under the copyright laws of the United States and
 * applicable international laws, treaties, and conventions.
 * 
 * You may freely redistribute and use this sample code, with or without
 * modification, provided you include the original copyright notice and use restrictions. 
 * 
 * Disclaimer: THE SAMPLE CODE IS PROVIDED "AS IS" AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL ESRI OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) SUSTAINED BY YOU OR A THIRD PARTY, HOWEVER CAUSED
 * AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
 * TORT ARISING IN ANY WAY OUT OF THE USE OF THIS SAMPLE CODE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * For additional information, contact:
 * Environmental Systems Research Institute, Inc.
 * Attn: Contracts and Legal Services Department
 * 380 New York Street Redlands, California, 92373
 * USA
 * email: contracts@esri.com
 * =============================================================================*/

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Research.Kinect.Nui;

namespace Kinect_MoCap
{
    public partial class MainWindow : UserControl
    {
        private DispatcherTimer _timerInclination = null;
        private DispatcherTimer _timerUI = null;
        private DateTime _animateKinectMenu = DateTime.Now;
        private DateTime _lastFrame = DateTime.Now;
        private int _frameCount = 0;
        private const int BLUE_IDX = 0;
        private const int GREEN_IDX = 1;
        private const int RED_IDX = 2;
        private const int ALPHA_IDX = 3;
        private Runtime _runtime = null;
        //
        // CONSTRUCTOR
        //
        public MainWindow()
        {
            InitializeComponent();

            // Loaded
            this.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);

            // Create Arms
            this.LeftArm = new Limb();
            this.RightArm = new Limb();

            // User Interface Timer
            this._timerUI = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(200d)
            };
            this._timerUI.Tick += new EventHandler(this.Timer_Tick);
            this._timerUI.Start();

            // Inclination Timer
            this._timerInclination = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(1000d)
            };
            this._timerInclination.Tick += new EventHandler(this.Timer_Tick);

            // Listen to Button Clicks
            this.ButtonInclinationPlus.Click += new RoutedEventHandler(this.Button_Click);
            this.ButtonInclinationMinus.Click += new RoutedEventHandler(this.Button_Click);

            //
            this.RadioButtonVideo.Checked += new RoutedEventHandler(this.RadioButton_Checked);
            this.RadioButtonDepth.Checked += new RoutedEventHandler(this.RadioButton_Checked);
            this.RadioButtonBlend.Checked += new RoutedEventHandler(this.RadioButton_Checked);

            // Assign Window to DataContext
            this.DataContext = this;
        }
        //
        // PROPERTIES
        //
        public static readonly DependencyProperty LeftArmProperty = DependencyProperty.Register(
            "LeftArm",
            typeof(Limb),
            typeof(MainWindow),
            new PropertyMetadata(null)
        );
        public static readonly DependencyProperty RightArmProperty = DependencyProperty.Register(
            "RightArm",
            typeof(Limb),
            typeof(MainWindow),
            new PropertyMetadata(null)
        );
        public static readonly DependencyProperty InclinationProperty = DependencyProperty.Register(
            "Inclination",
            typeof(int),
            typeof(MainWindow),
            new PropertyMetadata(0, new PropertyChangedCallback(MainWindow.OnPropertyChanged))
        );
        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(
            "DisplayMode",
            typeof(DisplayMode),
            typeof(MainWindow),
            new PropertyMetadata(DisplayMode.Video, new PropertyChangedCallback(MainWindow.OnPropertyChanged))
        );
        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate",
            typeof(double),
            typeof(MainWindow),
            new PropertyMetadata(0d)
        );
        public Limb LeftArm
        {
            get { return ((Limb)base.GetValue(MainWindow.LeftArmProperty)); }
            set { base.SetValue(MainWindow.LeftArmProperty, value); }
        }
        public Limb RightArm
        {
            get { return ((Limb)base.GetValue(MainWindow.RightArmProperty)); }
            set { base.SetValue(MainWindow.RightArmProperty, value); }
        }
        public int Inclination
        {
            get { return ((int)base.GetValue(MainWindow.InclinationProperty)); }
            set { base.SetValue(MainWindow.InclinationProperty, value); }
        }
        public DisplayMode DisplayMode
        {
            get { return ((DisplayMode)base.GetValue(MainWindow.DisplayModeProperty)); }
            set { base.SetValue(MainWindow.DisplayModeProperty, value); }
        }
        public double FrameRate
        {
            get { return ((double)base.GetValue(MainWindow.FrameRateProperty)); }
            set { base.SetValue(MainWindow.FrameRateProperty, value); }
        }
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainWindow kc = d as MainWindow;
            kc.PropertyChanged(e);
        }
        private void PropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == MainWindow.InclinationProperty)
            {
                this._timerInclination.Stop();
                this._timerInclination.Start();
            }
            else if (e.Property == MainWindow.DisplayModeProperty)
            {
                switch (this.DisplayMode)
                {
                    case DisplayMode.Video:
                        this.ImageVideo.Visibility = Visibility.Visible;
                        this.ImageDepth.Visibility = Visibility.Collapsed;
                        this.ImageVideo.Opacity = 1d;
                        this.ImageDepth.Opacity = 0d;
                        break;
                    case DisplayMode.Depth:
                        this.ImageVideo.Visibility = Visibility.Collapsed;
                        this.ImageDepth.Visibility = Visibility.Visible;
                        this.ImageVideo.Opacity = 0d;
                        this.ImageDepth.Opacity = 1d;
                        break;
                    case DisplayMode.Blend:
                        this.ImageVideo.Visibility = Visibility.Visible;
                        this.ImageDepth.Visibility = Visibility.Visible;
                        this.ImageVideo.Opacity = 1d;
                        this.ImageDepth.Opacity = 0.5d;
                        break;
                }
            }
        }
        //
        // PUBLIC METHODS
        //
        public void UnitializeKinect()
        {
            if (this._runtime != null)
            {
                this._runtime.Uninitialize();
            }
        }
        //
        // PRIVATE METHODS
        //
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!this.IsLoaded) { return; }
            if (this.Visibility != Visibility.Visible) { return; }
            if (this.Opacity == 0d) { return; }

            if (sender == this._timerUI)
            {
                // Inclination Plus
                bool plus = this.Inclination != Microsoft.Research.Kinect.Nui.Camera.ElevationMaximum;
                if (this.ButtonInclinationPlus.IsEnabled != plus)
                {
                    this.ButtonInclinationPlus.IsEnabled = plus;
                }

                // Inclination Minus
                bool minus = this.Inclination != Microsoft.Research.Kinect.Nui.Camera.ElevationMinimum;
                if (this.ButtonInclinationMinus.IsEnabled != minus)
                {
                    this.ButtonInclinationMinus.IsEnabled = minus;
                }
            }
            else if (sender == this._timerInclination)
            {
                // Adjust Sensor Tilt (if necessary)
                if (this.Inclination != this._runtime.NuiCamera.ElevationAngle)
                {
                    this._runtime.NuiCamera.ElevationAngle = this.Inclination;
                }

                // Stop the Inclination Timer
                this._timerInclination.Stop();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == this.ButtonInclinationPlus)
            {
                if (this.Inclination == Microsoft.Research.Kinect.Nui.Camera.ElevationMaximum) { return; }
                this.Inclination++;
            }
            else if (sender == this.ButtonInclinationMinus)
            {
                if (this.Inclination == Microsoft.Research.Kinect.Nui.Camera.ElevationMinimum) { return; }
                this.Inclination--;
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == this.RadioButtonVideo)
            {
                this.DisplayMode = DisplayMode.Video;
            }
            else if (sender == this.RadioButtonDepth)
            {
                this.DisplayMode = DisplayMode.Depth;
            }
            else if (sender == this.RadioButtonBlend)
            {
                this.DisplayMode = DisplayMode.Blend;
            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Exit if in designer (Visual Studiio IDE)
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(
                DesignerProperties.IsInDesignModeProperty,
                typeof(FrameworkElement)
            );
            bool isDesignMode = (bool)dpd.Metadata.DefaultValue;
            if (isDesignMode) { return; }

            // Initialize Kinect Sensor
            this._runtime = new Runtime();

            try
            {
                this._runtime.Initialize(
                    RuntimeOptions.UseDepthAndPlayerIndex |
                    RuntimeOptions.UseSkeletalTracking |
                    RuntimeOptions.UseColor
                );
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Runtime initialization failed. Please make sure Kinect device is plugged in.");
                return;
            }

            try
            {
                this._runtime.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                this._runtime.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Failed to open stream. Please make sure to specify a supported image type and resolution.");
                return;
            }

            this._runtime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(this.Kinect_SkeletonFrameReady);
            this._runtime.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(this.Kinect_VideoFrameReady);
            this._runtime.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(this.Kinect_DepthFrameReady);

            // Enable Skeleton Smoothing
            this._runtime.SkeletonEngine.SmoothParameters = new TransformSmoothParameters()
            {
                // ** Default **
                // Correction = 0.5f
                // JitterRadius = 0.05f
                // MaxDeviationRadius = 0.04f
                // Prediction = 0.5f
                // Smoothing = 0.5f
                Correction = 0.5f,
                JitterRadius = 0.1f,
                MaxDeviationRadius = 0.04f,
                Prediction = 0.5f,
                Smoothing = 1f
            };
            this._runtime.SkeletonEngine.TransformSmooth = true;

            // Store Inclination
            this.Inclination = this._runtime.NuiCamera.ElevationAngle;
        }
        private void Kinect_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // Exit if not visible
            if (!this.IsVisible) { return; }
            if (!this.ImageVideo.IsVisible) { return; }

            // 32-bit per pixel, RGB image
            PlanarImage planarImage = e.ImageFrame.Image;
            this.ImageVideo.Source = BitmapSource.Create(
                planarImage.Width,
                planarImage.Height,
                96d,
                96d,
                PixelFormats.Bgr32,
                null,
                planarImage.Bits,
                planarImage.Width * planarImage.BytesPerPixel
            );
        }
        private void Kinect_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // Exit if not visible
            if (!this.IsVisible) { return; }

            // Get Display Frame Rate
            this._frameCount++;
            DateTime now = DateTime.Now;
            TimeSpan diff = now.Subtract(this._lastFrame);
            if (diff > TimeSpan.FromSeconds(1d))
            {
                this.FrameRate = (double)this._frameCount / diff.TotalSeconds;
                this._frameCount = 0;
                this._lastFrame = now;
            }

            // Exit if depth view is turned off
            if (!this.ImageDepth.IsVisible) { return; }

            //
            PlanarImage planarImage = e.ImageFrame.Image;
            byte[] depth = planarImage.Bits;
            byte[] color = new byte[planarImage.Width * planarImage.Height * 4];
            ImageViewArea viewArea = new ImageViewArea()
            {
                CenterX = 0,
                CenterY = 0,
                Zoom = ImageDigitalZoom.Zoom1x
            };

            for (int y = 0; y < planarImage.Height; y++)
            {
                for (int x = 0; x < planarImage.Width; x++)
                {
                    int indexDepth = (y * planarImage.Width + x) * 2;

                    switch (this.DisplayMode)
                    {
                        case DisplayMode.Depth:
                            int realDepth = (depth[indexDepth + 1] << 5) | (depth[indexDepth] >> 3);
                            byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));
                            int indexColor = (y * planarImage.Width + x) * 4;
                            color[indexColor + 0] = (byte)(intensity / 2);
                            color[indexColor + 1] = (byte)(intensity / 2);
                            color[indexColor + 2] = (byte)(intensity / 2);
                            color[indexColor + 3] = (byte)255;
                            break;
                        case DisplayMode.Blend:
                            int player = planarImage.Bits[indexDepth] & 7;
                            if (player == 0) { break; }
                            int colorX;
                            int colorY;
                            this._runtime.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(
                                ImageResolution.Resolution640x480,
                                viewArea,
                                x,
                                y,
                                0,
                                out colorX,
                                out colorY
                            );
                            int adjustedX = colorX / 2;
                            int adjustedY = colorY / 2;
                            if (adjustedX < 0 || adjustedX > 319) { break; }
                            if (adjustedY < 0 || adjustedY > 239) { break; }
                            int indexColor2 = (adjustedY * planarImage.Width + adjustedX) * 4;
                            Color col = Colors.Transparent;
                            switch (player)
                            {
                                case 1:
                                    col = Colors.Red;
                                    break;
                                case 2:
                                    col = Colors.Green;
                                    break;
                                case 3:
                                    col = Colors.Blue;
                                    break;
                                case 4:
                                    col = Colors.White;
                                    break;
                                case 5:
                                    col = Colors.Gold;
                                    break;
                                case 6:
                                    col = Colors.Cyan;
                                    break;
                                case 7:
                                    col = Colors.Plum;
                                    break;
                            }
                            color[indexColor2 + 0] = (byte)col.B;
                            color[indexColor2 + 1] = (byte)col.R;
                            color[indexColor2 + 2] = (byte)col.G;
                            color[indexColor2 + 3] = (byte)col.A;

                            break;
                    }
                }
            }

            this.ImageDepth.Source = BitmapSource.Create(
                planarImage.Width,
                planarImage.Height,
                96d,
                96d,
                PixelFormats.Bgra32,
                null,
                color,
                planarImage.Width * 4
            );
        }
        private void Kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            try
            {
                //
                if (!this.IsVisible) { return; }

                // Clear Skeleton from display
                this.CanvasSkeleton.Children.Clear();

                // Get Closest Tracked Skeleton
                var tracked = e.SkeletonFrame.Skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked);
                var ordered = tracked.OrderBy(s => s.Position.ToWindowsVector().Length);
                var primary = ordered.FirstOrDefault();

                // Left Arm
                bool trackLeftArm =
                    primary != null &&
                    primary.TrackingState == SkeletonTrackingState.Tracked &&
                    primary.Joints[JointID.HandLeft].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.WristLeft].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.ElbowLeft].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.ShoulderLeft].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.ShoulderCenter].TrackingState == JointTrackingState.Tracked;

                // Right Arm
                bool trackRightArm =
                    primary != null &&
                    primary.TrackingState == SkeletonTrackingState.Tracked &&
                    primary.Joints[JointID.HandRight].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.WristRight].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.ElbowRight].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.ShoulderRight].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.ShoulderCenter].TrackingState == JointTrackingState.Tracked;

                // Head
                bool trackHead =
                    primary != null &&
                    primary.TrackingState == SkeletonTrackingState.Tracked &&
                    primary.Joints[JointID.Head].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.ShoulderCenter].TrackingState == JointTrackingState.Tracked;

                // Torso
                bool trackTorso =
                    primary != null &&
                    primary.TrackingState == SkeletonTrackingState.Tracked &&
                    primary.Joints[JointID.ShoulderCenter].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.Spine].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.HipCenter].TrackingState == JointTrackingState.Tracked;

                // Left Leg
                bool trackLeftLeg =
                    primary != null &&
                    primary.TrackingState == SkeletonTrackingState.Tracked &&
                    primary.Joints[JointID.HipCenter].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.HipLeft].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.KneeLeft].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.AnkleLeft].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.FootLeft].TrackingState == JointTrackingState.Tracked;

                // Right Leg
                bool trackRightLeg =
                    primary != null &&
                    primary.TrackingState == SkeletonTrackingState.Tracked &&
                    primary.Joints[JointID.HipCenter].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.HipRight].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.KneeRight].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.AnkleRight].TrackingState == JointTrackingState.Tracked &&
                    primary.Joints[JointID.FootRight].TrackingState == JointTrackingState.Tracked;

                // General Flag to stop drawing and tracking
                bool active = trackLeftArm || trackRightArm || trackLeftLeg || trackRightLeg;

                // Draw to Canvas
                if (this.CheckBoxSkeleton.IsChecked.Value && active)
                {
                    // Left Arm
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.HandLeft], primary.Joints[JointID.WristLeft], trackLeftArm));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.WristLeft], primary.Joints[JointID.ElbowLeft], trackLeftArm));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.ElbowLeft], primary.Joints[JointID.ShoulderLeft], trackLeftArm));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.ShoulderLeft], primary.Joints[JointID.ShoulderCenter], trackLeftArm));

                    // Right Arm
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.HandRight], primary.Joints[JointID.WristRight], trackRightArm));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.WristRight], primary.Joints[JointID.ElbowRight], trackRightArm));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.ElbowRight], primary.Joints[JointID.ShoulderRight], trackRightArm));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.ShoulderRight], primary.Joints[JointID.ShoulderCenter], trackRightArm));

                    // Head
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.Head], primary.Joints[JointID.ShoulderCenter], trackHead));

                    // Torso
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.ShoulderCenter], primary.Joints[JointID.Spine], trackTorso));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.Spine], primary.Joints[JointID.HipCenter], trackTorso));

                    // Left Leg
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.HipCenter], primary.Joints[JointID.HipLeft], trackLeftLeg));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.HipLeft], primary.Joints[JointID.KneeLeft], trackLeftLeg));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.KneeLeft], primary.Joints[JointID.AnkleLeft], trackLeftLeg));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.AnkleLeft], primary.Joints[JointID.FootLeft], trackLeftLeg));

                    // Right Leg
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.HipCenter], primary.Joints[JointID.HipRight], trackRightLeg));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.HipRight], primary.Joints[JointID.KneeRight], trackRightLeg));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.KneeRight], primary.Joints[JointID.AnkleRight], trackRightLeg));
                    this.CanvasSkeleton.Children.Add(this.JointToSegment(primary.Joints[JointID.AnkleRight], primary.Joints[JointID.FootRight], trackRightLeg));

                    // Draw Joints
                    foreach (Joint joint in primary.Joints)
                    {
                        if (joint.TrackingState == JointTrackingState.NotTracked) { continue; }
                        Point point = this.SkeletonToImage(joint.Position);
                        Ellipse ellipse = new Ellipse()
                        {
                            Height = 20d,
                            Width = 20d,
                            Fill = new SolidColorBrush(Colors.White),
                            StrokeThickness = 0d
                        };
                        Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2d);
                        Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2d);
                        this.CanvasSkeleton.Children.Add(ellipse);
                    }
                }

                // Show Skeleton Clips Warning
                Visibility head = (primary != null) && ((primary.Quality & SkeletonQuality.ClippedTop) == SkeletonQuality.ClippedTop) ? Visibility.Visible : Visibility.Hidden;
                Visibility left = (primary != null) && ((primary.Quality & SkeletonQuality.ClippedLeft) == SkeletonQuality.ClippedLeft) ? Visibility.Visible : Visibility.Hidden;
                Visibility righ = (primary != null) && ((primary.Quality & SkeletonQuality.ClippedRight) == SkeletonQuality.ClippedRight) ? Visibility.Visible : Visibility.Hidden;
                Visibility bott = (primary != null) && ((primary.Quality & SkeletonQuality.ClippedBottom) == SkeletonQuality.ClippedBottom) ? Visibility.Visible : Visibility.Hidden;
                if (this.StickmanHead.Visibility != head)
                {
                    this.StickmanHead.Visibility = head;
                }
                if (this.StickmanLeftArm.Visibility != left)
                {
                    this.StickmanLeftArm.Visibility = left;
                }
                if (this.StickmanRightArm.Visibility != righ)
                {
                    this.StickmanRightArm.Visibility = righ;
                }
                if (this.StickmanLeftLeg.Visibility != bott)
                {
                    this.StickmanLeftLeg.Visibility = bott;
                }
                if (this.StickmanRightLeg.Visibility != bott)
                {
                    this.StickmanRightLeg.Visibility = bott;
                }

                // Update Arm Tracking Flag
                this.LeftArm.IsTracked = active;
                this.RightArm.IsTracked = active;

                // Exit if not tracking
                if (!active) { return; }

                // Get Skeleton Center of Mass
                Vector3D com = primary.Position.ToWindowsVector();

                // Get Camera Tilt
                Vector3D tilt = e.SkeletonFrame.NormalToGravity.ToWindowsVector();

                // Get Joints
                Joint jShoulderL = primary.Joints[JointID.ShoulderLeft];
                Joint jShoulderR = primary.Joints[JointID.ShoulderRight];
                Joint jElbowL = primary.Joints[JointID.ElbowLeft];
                Joint jElbowR = primary.Joints[JointID.ElbowRight];
                Joint jHandL = primary.Joints[JointID.HandLeft];
                Joint jHandR = primary.Joints[JointID.HandRight];

                // Update Joints
                double horz;
                double vert;
                double extension;

                this.CalculateLimb(jShoulderL, jElbowL, jHandL, tilt, com, out horz, out vert, out extension);
                this.LeftArm.HorizontalAngle = horz;
                this.LeftArm.VerticalAngle = vert;
                this.LeftArm.Extension = extension;

                this.CalculateLimb(jShoulderR, jElbowR, jHandR, tilt, com, out horz, out vert, out extension);
                this.RightArm.HorizontalAngle = horz;
                this.RightArm.VerticalAngle = vert;
                this.RightArm.Extension = extension;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private void CalculateLimb(Joint shoulder, Joint elbow, Joint hand, Vector3D tilt, Vector3D com, out double horz, out double vert, out double extension)
        {
            // Get Joint Positions
            Vector3D shoulderL = shoulder.Position.ToWindowsVector();
            Vector3D elbowL = elbow.Position.ToWindowsVector();
            Vector3D handL = hand.Position.ToWindowsVector();

            // Get Arm
            Vector3D armL = handL - shoulderL;

            // Get Forearm
            Vector3D forearmL = handL - elbowL;

            // Get Upperarm
            Vector3D backarmL = elbowL - shoulderL;

            // Calculate Arm Expansion
            extension = armL.Length / (forearmL.Length + backarmL.Length);

            // Offset Camera Tilt *** tilt not supported in beta ***
            armL.Normalize();
            armL -= tilt;

            // Offset Sensor Elevation Angle
            Vector3D xaxis = new Vector3D(1d, 0d, 0d);
            Quaternion q = new Quaternion(xaxis, -this._runtime.NuiCamera.ElevationAngle);
            Matrix3D m = Matrix3D.Identity;
            m.Rotate(q);
            armL *= m;

            // Calcualte Horizontal/Vertical Angles
            vert = Math.Atan(armL.Y / Math.Sqrt(Math.Pow(armL.Z, 2) + Math.Pow(armL.X, 2))) * 180d / Math.PI;
            horz = Math.Atan(armL.X / armL.Z) * 180d / Math.PI;
            horz -= Math.Atan(com.X / com.Z) * 180d / Math.PI;
            horz *= -1;
        }
        private Line JointToSegment(Joint j1, Joint j2, bool tracked)
        {
            Point p1 = this.SkeletonToImage(j1.Position);
            Point p2 = this.SkeletonToImage(j2.Position);
            Line line = new Line()
            {
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y,
                Stroke = tracked ? new SolidColorBrush(Colors.Yellow) : new SolidColorBrush(Colors.Red),
                StrokeThickness = 4d
            };
            return line;
        }
        private Point SkeletonToImage(Microsoft.Research.Kinect.Nui.Vector vector)
        {
            float depthX;
            float depthY;
            this._runtime.SkeletonEngine.SkeletonToDepthImage(vector, out depthX, out depthY);

            // Convert to 320, 240 space
            depthX = Math.Max(0, Math.Min(depthX * 320, 320));
            depthY = Math.Max(0, Math.Min(depthY * 240, 240));

            int colorX;
            int colorY;
            ImageViewArea iv = new ImageViewArea();
            this._runtime.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(
                ImageResolution.Resolution640x480,
                iv,
                (int)depthX,
                (int)depthY,
                (short)0,
                out colorX,
                out colorY
            );

            //
            double imageWidth = (double)this._runtime.VideoStream.Width;
            double imageHeight = (double)this._runtime.VideoStream.Height;
            double ratiox = this.ActualWidth / imageWidth;
            double ratioy = this.ActualHeight / imageHeight;
            double scale = ratiox < ratioy ? this.ActualHeight / 480d : this.ActualWidth / 680d;
            double imageCenterX = imageWidth / 2d;
            double imageCenterY = imageHeight / 2d;
            double windowCenterX = this.ActualWidth / 2d;
            double windowCenterY = this.ActualHeight / 2d;
            double imageDiffX = imageCenterX - colorX;
            double imageDiffY = imageCenterY - colorY;
            double X = windowCenterX - (scale * imageDiffX);
            double Y = windowCenterY - (scale * imageDiffY);
            return new Point(X, Y);
        }
    }
}