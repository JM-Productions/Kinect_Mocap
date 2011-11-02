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

using System.Windows;

namespace Kinect_MoCap {
    public class Limb : DependencyObject {
        public static readonly DependencyProperty IsTrackedProperty = DependencyProperty.Register(
            "IsTracked",
            typeof(bool),
            typeof(Limb),
            new PropertyMetadata(false)
        );
        public static readonly DependencyProperty HorizontalAngleProperty = DependencyProperty.Register(
            "HorizontalAngle",
            typeof(double),
            typeof(Limb),
            new PropertyMetadata(0d)
        );
        public static readonly DependencyProperty VerticalAngleProperty = DependencyProperty.Register(
            "VerticalAngle",
            typeof(double),
            typeof(Limb),
            new PropertyMetadata(0d)
        );
        public static readonly DependencyProperty ExtensionProperty = DependencyProperty.Register(
            "Extension",
            typeof(double),
            typeof(Limb),
            new PropertyMetadata(0d)
        );

        /// <summary>
        /// Is Arm Tracked
        /// </summary>
        public bool IsTracked {
            get { return ((bool)base.GetValue(Limb.IsTrackedProperty)); }
            set { base.SetValue(Limb.IsTrackedProperty, value); }
        }

        /// <summary>
        /// Horizontal angle of the arm. Where zero is pointing directly at camera and positive is to the right.
        /// </summary>
        public double HorizontalAngle {
            get { return ((double)base.GetValue(Limb.HorizontalAngleProperty)); }
            set { base.SetValue(Limb.HorizontalAngleProperty, value); }
        }

        /// <summary>
        /// Vertical angle of the arm. Where zero is pointing directly at camera and positive is above.
        /// </summary>
        public double VerticalAngle {
            get { return ((double)base.GetValue(Limb.VerticalAngleProperty)); }
            set { base.SetValue(Limb.VerticalAngleProperty, value); }
        }

        /// <summary>
        /// Relative extension of the arm. Where one is fully extended, less than one when hands are closer to the chest.
        /// </summary>
        public double Extension {
            get { return ((double)base.GetValue(Limb.ExtensionProperty)); }
            set { base.SetValue(Limb.ExtensionProperty, value); }
        }
    }
}
