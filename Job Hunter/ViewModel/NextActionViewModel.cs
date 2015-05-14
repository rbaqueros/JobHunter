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

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Job_Hunter.Model;
using Job_Hunter.View;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Job_Hunter.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class NextActionViewModel : SimpleItemBaseViewModel
    {
        public NextActionViewModel(IDataService dataService) : base(dataService)
        {
            // Fill next action collection
            _dataService.NextActionList(FillItemCollection);

            // Set event handling
            _dataService.NextActionModified += (sender, e) => _dataService.NextActionList(FillItemCollection);
        }

        /*
         * 
         * Command processors
         * 
         */
        public override int AddCommand(string title, string note)
        {
            return _dataService.AddNextAction(title, note);
        }

        public override int UpdateCommand(long id, long position, string title, string note)
        {
            return _dataService.UpdateNextAction(id, position, title, note);
        }

        public override int DeleteCommand(long id)
        {
            return _dataService.DeleteNextAction(id);
        }

        public override void UpCommand(long position)
        {
            _dataService.NextActionUp(position);
        }

        public override void DownCommand(long position)
        {
            _dataService.NextActionDown(position);
        }
    }
}
