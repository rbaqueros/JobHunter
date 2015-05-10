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

using MahApps.Metro.Controls;
using System;
using Job_Hunter.ViewModel;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using MahApps.Metro.Controls.Dialogs;

namespace Job_Hunter
{
    /// <summary>
    /// Description for MainWindow.
    /// </summary>
    public partial class MainWindow : MetroWindow, IDialogService
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // Set current culture.
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            // Initialize
            InitializeComponent();

            // Get reference to ViewModelLocator
            ViewModelLocator vm = new ViewModelLocator();

            // Register this object for dialog service.
            vm.DialogService = this;
        }

        //public void callback(int returnValue)
        //{
        //    System.Console.WriteLine("Return value: " + returnValue);
        //}

        public async Task<int> ShowMessageDialog(string message, string title, string[] buttonText)
        {
            MetroDialogSettings dialogSettings;
            MessageDialogStyle dialogStyle;

            switch(buttonText.Length)
            {
                case 1:
                    dialogSettings = new MetroDialogSettings() { AffirmativeButtonText = buttonText[0] };
                    dialogStyle = MessageDialogStyle.Affirmative;
                    break;
                case 2:
                    dialogSettings = new MetroDialogSettings() { AffirmativeButtonText = buttonText[0],
                                                                 NegativeButtonText=buttonText[1] };
                    dialogStyle = MessageDialogStyle.AffirmativeAndNegative;
                    break;
                case 3:
                    dialogSettings = new MetroDialogSettings() { AffirmativeButtonText = buttonText[0],
                                                                 NegativeButtonText=buttonText[1],
                                                                 FirstAuxiliaryButtonText=buttonText[2] };
                    dialogStyle = MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary;
                    break;
                case 4:
                    dialogSettings = new MetroDialogSettings() { AffirmativeButtonText = buttonText[0],
                                                                 NegativeButtonText=buttonText[1],
                                                                 FirstAuxiliaryButtonText=buttonText[2],
                                                                 SecondAuxiliaryButtonText=buttonText[3] };
                    dialogStyle = MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary;
                    break;
                default:
                    throw new Exception("Invalid button text array");
            }

            MessageDialogResult result = await this.ShowMessageAsync(title, message, dialogStyle, dialogSettings);

            int dialogResult;
            switch(result)
            {
                case MessageDialogResult.Affirmative:
                    dialogResult = 0;
                    break;
                case MessageDialogResult.Negative:
                    dialogResult = 1;
                    break;
                case MessageDialogResult.FirstAuxiliary:
                    dialogResult = 2;
                    break;
                case MessageDialogResult.SecondAuxiliary:
                    dialogResult = 3;
                    break;
                default:
                    throw new Exception("Invalid dialog result");
            }

            return dialogResult;
        }

        public string ShowFileDialog(string fileName, string fileFilter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.CheckPathExists = true;

            // File filter
            dlg.Filter = fileFilter;

            // Show file open dialog and get result.
            Nullable<bool> result = dlg.ShowDialog();

            // If result not true return.
            if (result != true) { return null; }

            return dlg.FileName;
        }
    }
}