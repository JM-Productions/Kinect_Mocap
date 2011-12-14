// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Coding4Fun.Kinect.Common;
using Microsoft.Research.Kinect.Nui;

namespace Coding4Fun.Kinect.Wpf
{
	public static class SkeletalExtensions
	{
		public static Joint ScaleTo(this Joint joint, int width, int height)
		{
			return SkeletalCommonExtensions.ScaleTo(joint, width, height);
		}

		public static Joint ScaleTo(this Joint joint, int width, int height, float maxSkeletonX, float maxSkeletonY)
		{
			return SkeletalCommonExtensions.ScaleTo(joint, width, height, maxSkeletonX, maxSkeletonY);
		}
	}
}
