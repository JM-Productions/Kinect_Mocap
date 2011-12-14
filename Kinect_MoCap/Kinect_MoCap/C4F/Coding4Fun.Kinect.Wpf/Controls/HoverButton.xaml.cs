// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Coding4Fun.Kinect.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for HoverButton.xaml
    /// </summary>
    public partial class HoverButton : UserControl
    {
        public event RoutedEventHandler Click;

		public Storyboard uxSBHold, uxSBPing;
		private bool _isHeld;

        public HoverButton()
        {
            InitializeComponent();

            uxSBHold = FindResource("uxSBHold") as Storyboard;
            uxSBPing = FindResource("uxSBPing") as Storyboard;

            if (uxSBHold != null)
                uxSBHold.Completed += uxSBHold_Completed;
        }

        public int TimeInterval
        {
            get { return _timeInterval; }
            set
            {
                _timeInterval = value;

                if (uxSBHold.Children.Count > 0 && TimeInterval > 0)
                {
                    var arcAnimation = uxSBHold.Children[0] as DoubleAnimationUsingKeyFrames;
                    if (arcAnimation == null)
                        return;

                    arcAnimation.KeyFrames.Clear();

                    var start = new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.Zero));
                    var end = new LinearDoubleKeyFrame(360, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, TimeInterval)));

                    arcAnimation.KeyFrames.Add(start);
                    arcAnimation.KeyFrames.Add(end);
                }
            }
        }
        private int _timeInterval;

		private double _imageSize;
		public double ImageSize
		{
			get { return _imageSize; }
            set
            {
                _imageSize = value;
                Height = Width = ImageSize;
            }
		}

		private string _imageSource;
		public string ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				if(ImageSource!=null)
					uxButtonImage.Source = new BitmapImage(new Uri(ImageSource, UriKind.RelativeOrAbsolute));
			}
		}

		private string _activeImageSource;
		public string ActiveImageSource
		{
			get { return _activeImageSource; }
			set
			{
				_activeImageSource = value;
				if (ActiveImageSource != null)
                    uxButtonActiveImage.Source = new BitmapImage(new Uri(ActiveImageSource, UriKind.RelativeOrAbsolute));
			}
		}

		public bool IsTriggeredOnHover{ get; set; }

		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				if ((IsTriggeredOnHover && value && !_isChecked) || (!IsTriggeredOnHover && value != _isChecked)) 
                    uxSBPing.Begin();
				
                _isChecked = value;

				if ((IsChecked && IsTriggeredOnHover || !IsTriggeredOnHover) && Click != null)
                    Click(this, null);

			    VisualStateManager.GoToState(this, IsChecked ? "Checked" : "Normal", true);
			}
		}

		void uxSBHold_Completed(object sender, EventArgs e)
		{
            ToggleIsCheck();
		}

        public void Hovering()
        {
            ToggleButtons(true, uxSBHold.Begin);
        }

	    public void Release()
		{
            ToggleButtons(false, uxSBHold.Stop);
		}

        private void ToggleButtons(bool wantedState, Action action)
        {
            if (_isHeld != wantedState)
            {
                _isHeld = wantedState;

                if (IsTriggeredOnHover)
                    ToggleIsCheck();
                else
                    action();
            }
        }

        private void ToggleIsCheck()
        {
            IsChecked = !IsChecked;
        }
    }
}
