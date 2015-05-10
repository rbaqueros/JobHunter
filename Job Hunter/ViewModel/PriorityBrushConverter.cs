/*
 * Copyright (c) 2015 Rafael Baquero
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace Job_Hunter.ViewModel
{
    class PriorityBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int appPriority = (int)value;
            switch (appPriority)
            {
                case 0:
                    return new SolidColorBrush(Color.FromRgb(0, 210, 0));
                case 1:
                    return new SolidColorBrush(Color.FromRgb(0, 0, 210));
                case 2:
                    return new SolidColorBrush(Color.FromRgb(255, 165, 0));
                case 3:
                    return new SolidColorBrush(Color.FromRgb(255, 140, 105));
                case 4:
                    return new SolidColorBrush(Color.FromRgb(160, 82, 45));
                case 5:
                    return new SolidColorBrush(Color.FromRgb(210, 0, 0));
                default:
                    {
                        throw new Exception("Invalid application priority brush value");
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
