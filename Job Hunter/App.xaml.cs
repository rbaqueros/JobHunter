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

using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Job_Hunter.Model;
using System.Windows.Media;

namespace Job_Hunter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly PriorityItem[] priorityArray;

        static App()
        {
            DispatcherHelper.Initialize();
            
            // Priority array.
            priorityArray = new PriorityItem[6];
            priorityArray[0] = new PriorityItem(1, "Very High", new SolidColorBrush(Color.FromRgb(0, 210, 0)));
            priorityArray[1] = new PriorityItem(2, "High", new SolidColorBrush(Color.FromRgb(0, 0, 210)));
            priorityArray[2] = new PriorityItem(3, "Medium", new SolidColorBrush(Color.FromRgb(255, 165, 0)));
            priorityArray[3] = new PriorityItem(4, "Low", new SolidColorBrush(Color.FromRgb(255, 140, 105)));
            priorityArray[4] = new PriorityItem(5, "Very Low", new SolidColorBrush(Color.FromRgb(160, 82, 45)));
            priorityArray[5] = new PriorityItem(6, "Concluded", new SolidColorBrush(Color.FromRgb(210, 0, 0)));
        }
    }
}
