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

using System.Windows.Media;

namespace Job_Hunter.Model
{
    public struct PriorityItem
    {
        private int _priorityNumber;
        private string _priorityDescription;
        private SolidColorBrush _priorityColor;

        public PriorityItem(int newPriorityNumber, string newPriorityDescription, SolidColorBrush newPriorityColor)
        {
            _priorityNumber = newPriorityNumber;
            _priorityDescription = newPriorityDescription;
            _priorityColor = newPriorityColor;
        }

        /*
         * Properties
         */
        public int PriorityNumber
        {
            get { return _priorityNumber; }
        }

        public string PriorityDescription
        {
            get { return _priorityDescription; }
        }

        public SolidColorBrush PriorityColor
        {
            get { return _priorityColor; }
        }
    }
}
