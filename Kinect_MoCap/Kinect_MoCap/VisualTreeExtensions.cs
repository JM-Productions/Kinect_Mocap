//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kinect_MoCap {
    public static class VisualTreeExtensions {
        private static UIElement[] GetAllUIElements(this Panel parentPanel) {
            var elements = new List<UIElement>();
            if (null != parentPanel) {
                GetAllUIElements(parentPanel, elements);
            }
            return elements.ToArray();
        }

        private static void GetAllUIElements(Panel parent, ICollection<UIElement> elements) {
            foreach (var element in parent.Children) {
                if (element is Panel) {
                    GetAllUIElements(element as Panel, elements);
                }
                else if (element is ContentControl) {
                    var contentElement = element as ContentControl;
                    if (contentElement.Content is Panel) {
                        GetAllUIElements(contentElement.Content as Panel, elements);
                    }
                    else if (contentElement.Content is UIElement) {
                        elements.Add(contentElement.Content as UIElement);
                    }
                }
                else if (element is UIElement) {
                    elements.Add(element as UIElement);
                }
            }
        }

        public static T FindVisualParent<T>(this DependencyObject obj) where T : DependencyObject {
            var parent = VisualTreeHelper.GetParent(obj);
            while (parent != null) {
                T typed = parent as T;
                if (typed != null) {
                    return typed;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }
}
