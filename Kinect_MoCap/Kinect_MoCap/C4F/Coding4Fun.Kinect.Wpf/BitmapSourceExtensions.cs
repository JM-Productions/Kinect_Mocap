// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Coding4Fun.Kinect.Wpf
{
    public static class BitmapSourceExtensions
    {
		// securitycritical covers this
		[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		//[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		[SecurityCritical]
        public static void Save(this BitmapSource image, string filePath, ImageFormat format)
        {
            BitmapEncoder encoder = null;
            
            switch(format)
            {
                case ImageFormat.Png:
                    encoder = new PngBitmapEncoder();
                    break;
                case ImageFormat.Jpeg:
                    encoder = new JpegBitmapEncoder();
                    break;
                case ImageFormat.Bmp:
                    encoder = new BmpBitmapEncoder();
                    break;
            }

            if (encoder == null) 
                return;

            encoder.Frames.Add(BitmapFrame.Create(BitmapFrame.Create(image)));

            using (var stream = new FileStream(filePath, FileMode.Create))
                encoder.Save(stream);
        }

		public static BitmapSource ToBitmapSource(this byte[] pixels, int width, int height)
		{
			return ToBitmapSource(pixels, width, height, PixelFormats.Bgr32);
		}

		public static BitmapSource ToBitmapSource(this byte[] pixels, int width, int height, PixelFormat format)
		{
			return BitmapSource.Create(width, height, 96, 96, format, null, pixels, width * format.BitsPerPixel / 8);
		}
    }
}