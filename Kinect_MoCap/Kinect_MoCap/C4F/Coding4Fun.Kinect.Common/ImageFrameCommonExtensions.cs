// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Research.Kinect.Nui;

[assembly: CLSCompliant(true)]
namespace Coding4Fun.Kinect.Common
{
    internal static class ImageFrameCommonExtensions
    {
        // inlined the functions for perf boost
		// included here for reference
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static int CalculateDepthFromDepthFrameWithPlayerIndex(byte firstFrame, byte secondFrame)
        {
            return (firstFrame >> 3) | (secondFrame << 5); // first three are player index
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static int CalculateDepthFromDepthFrame(byte firstFrame, byte secondFrame)
        {
            return (firstFrame) | (secondFrame << 8);
        }

        public const int RedIndex = 2;
        public const int GreenIndex = 1;
        public const int BlueIndex = 0;

		const float MaxDepthDistance = 4000; // 4000 seemed to be max value returned
        const float MinDepthDistance = 800; // 800 seemed to be the min value returned
        const float MaxDepthDistanceOffset = MaxDepthDistance - MinDepthDistance;

		public static short GetDistance(this ImageFrame image, int x, int y)
		{
			var width = image.Image.Width;

			if (x > width)
				throw new ArgumentOutOfRangeException("x", "x is larger than the width");

			if (y > image.Image.Height)
				throw new ArgumentOutOfRangeException("y", "y is larger than the height");

			if (x < 0)
				throw new ArgumentOutOfRangeException("x", "x is smaller than zero");

			if (y < 0)
				throw new ArgumentOutOfRangeException("y", "y is smaller than zero");

			
			var index = width * y;
			short distance = 0;

			switch (image.Type)
			{
				case ImageType.DepthAndPlayerIndex:
					index += x;
					index *= 2; // depth array is 2 bytes per
					distance = (short)((image.Image.Bits[index] >> 3) | (image.Image.Bits[index + 1] << 5));
					break;
				case ImageType.Depth:
					index += (width - x - 1);
					index *= 2; // depth array is 2 bytes per
					distance = (short)(image.Image.Bits[index] | image.Image.Bits[index + 1] << 8);
					break;
			}

			return distance;
		}

    	public static void GetMidpoint(this short[] depthData, int width, int height, int startX, int startY, int endX, int endY, int minimumDistance, out double xLocation, out double yLocation)
        {
            if (depthData == null)
                throw new ArgumentNullException("depthData");

            if (width * height != depthData.Length)
                throw new ArgumentOutOfRangeException("depthData", "Depth Data length does not match target height and width");

            if (endX > width)
                throw new ArgumentOutOfRangeException("endX", "endX is larger than the width");

            if (endY > height)
                throw new ArgumentOutOfRangeException("endY", "endY is larger than the height");

            if (startX < 0)
                throw new ArgumentOutOfRangeException("startX", "startX is smaller than zero");

            if (startY < 0)
                throw new ArgumentOutOfRangeException("startY", "startY is smaller than zero");

            xLocation = 0;
            yLocation = 0;

            var counter = 0;
            for (var x = startX; x < endX; x++)
            {
                for (var y = startY; y < endY; y++)
                {
                    var depth = depthData[x + width * y];
                    if (depth > minimumDistance || depth <= 0)
                        continue;

                    xLocation += x;
                    yLocation += y;

                    counter++;
                }
            }

            if (counter <= 0)
                return;

            xLocation /= counter;
            yLocation /= counter;
        }

		public static short[] ToDepthArray(this ImageFrame image)
        {
            if (image == null)
                throw new ArgumentNullException("image");
			
			var width = image.Image.Width;
			var height = image.Image.Height;
			var depthArray = new short[width * image.Image.Height];
           
			var greyIndex = 0;
			for (var y = 0; y < height; y++)
			{
				var heightOffset = y * width;

				for (var x = 0; x < width; x++)
				{
					switch (image.Type)
					{
						case ImageType.DepthAndPlayerIndex:
							depthArray[x + heightOffset] = (short)((image.Image.Bits[greyIndex] >> 3) | (image.Image.Bits[greyIndex + 1] << 5));
							break;
						case ImageType.Depth:
							depthArray[(width - x - 1) + heightOffset] = (short)((image.Image.Bits[greyIndex] | image.Image.Bits[greyIndex + 1] << 8));
							break;
					}

					greyIndex += 2;
				}
			}


            return depthArray;
        }

		public static short[][] ToDepthArray2D(this ImageFrame image)
		{
			if (image == null)
				throw new ArgumentNullException("image");

            var width = image.Image.Width;
            var height = image.Image.Height;
			var depthData = new short[width][];
            var greyIndex = 0;
            
			for (var x = 0; x < width; x++)
				depthData[x] = new short[height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    switch (image.Type)
                    {
                        case ImageType.DepthAndPlayerIndex:
                            depthData[x][y] = (short)((image.Image.Bits[greyIndex] >> 3) | (image.Image.Bits[greyIndex + 1] << 5));
                            break;
                        case ImageType.Depth: // depth comes back mirrored
							depthData[width - x - 1][y] = (short)((image.Image.Bits[greyIndex] | image.Image.Bits[greyIndex + 1] << 8));
                            break;
                    }

                    greyIndex += 2;
                }
            }

            return depthData;
		}

        public static byte CalculateIntensityFromDepth(int distance)
        {
            // realDepth is now millimeter
            // transform 13-bit depth information into an 8-bit intensity appropriate
            // for display (we disregard information in most significant bit)
            return (byte)(255 - (255 * Math.Max(distance - MinDepthDistance, 0) / (MaxDepthDistanceOffset)));
        }

        public static byte CalculateIntensityFromDepth(byte firstFrame, byte secondFrame)
        {
            // realDepth is now millimeter
            // transform 13-bit depth information into an 8-bit intensity appropriate
            // for display (we disregard information in most significant bit)
            return (byte)(255 - (255 * Math.Max(((firstFrame) | (secondFrame << 8)) - MinDepthDistance, 0) / (MaxDepthDistanceOffset)));
        }

        public static byte CalculateIntensityFromDepthWithPlayerIndex(byte firstFrame, byte secondFrame)
        {
            // realDepth is now millimeter
            // transform 13-bit depth information into an 8-bit intensity appropriate
            // for display (we disregard information in most significant bit)
            return (byte)(255 - (255 * Math.Max(((firstFrame >> 3) | (secondFrame << 5)) - MinDepthDistance, 0) / (MaxDepthDistanceOffset)));
        }

        public static void SkeletonOverlay(ref byte redFrame, ref byte greenFrame, ref byte blueFrame, int player)
        {
            switch (player)
            {
                default: // case 0:
                    break;
                case 1:
                    greenFrame = 0;
                    blueFrame = 0;
                    break;
                case 2:
                    redFrame = 0;
                    greenFrame = 0;
                    break;
                case 3:
                    redFrame = 0;
                    blueFrame = 0;
                    break;
                case 4:
                    greenFrame = 0;
                    break;
                case 5:
                    blueFrame = 0;
                    break;
                case 6:
                    redFrame = 0;
                    break;
                case 7:
                    redFrame /= 2;
                    blueFrame = 0;
                    break;
            }
        }

		public static byte[] ConvertDepthFrameDataToBitmapData(IList<byte> depthData, int width, int height)
		{
			var colorFrame = new byte[height * width * 4];

			var greyIndex = 0;
			for (var y = 0; y < height; y++)
			{
				var heightOffset = y * width;

				for (var x = 0; x < width; x++)
				{
					var intensity = CalculateIntensityFromDepth(depthData[greyIndex], depthData[greyIndex + 1]);
					var index = ((width - x - 1) + heightOffset) * 4;

					colorFrame[index + RedIndex] = intensity;
					colorFrame[index + GreenIndex] = intensity;
					colorFrame[index + BlueIndex] = intensity;

					greyIndex += 2;
				}
			}

			return colorFrame;
		}

		public static byte[] ConvertDepthFrameDataWithSkeletonToBitmapData(IList<byte> depthData, int width, int height)
		{
			var colorFrame = new byte[height * width * 4];

			for (int greyIndex = 0, colorIndex = 0;
				 greyIndex < depthData.Count && colorIndex < colorFrame.Length;
				 greyIndex += 2, colorIndex += 4)
			{
				var player = depthData[greyIndex] & 0x07;
				var intensity = CalculateIntensityFromDepthWithPlayerIndex(depthData[greyIndex], depthData[greyIndex + 1]);

				colorFrame[colorIndex + RedIndex] = intensity;
				colorFrame[colorIndex + GreenIndex] = intensity;
				colorFrame[colorIndex + BlueIndex] = intensity;

				SkeletonOverlay(
					ref colorFrame[colorIndex + RedIndex],
					ref colorFrame[colorIndex + GreenIndex],
					ref colorFrame[colorIndex + BlueIndex], player);
			}

			return colorFrame;
		}
    }
}
