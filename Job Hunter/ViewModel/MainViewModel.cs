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

namespace Job_Hunter.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private SettingsViewModel _settingsViewModel;
        private TabsViewModel _tabsViewModel;
        private AboutViewModel _aboutViewModel;
        private ViewModelBase _currentViewModel;
        private string _nextSettingsViewTitle;
        private string _nextAboutViewTitle;
        private string _settingsViewTitle = "SETTINGS";
        private string _mainViewTitle = "MAIN";
        private string _aboutViewTitle = "ABOUT";

        public MainViewModel()
        {
            // Initialize the views.
            _settingsViewModel = new SettingsViewModel();
            _tabsViewModel = new TabsViewModel();
            _aboutViewModel = new AboutViewModel();
            _currentViewModel = _tabsViewModel;
            _nextSettingsViewTitle = _settingsViewTitle;
            _nextAboutViewTitle = _aboutViewTitle;

            // Set command relays.
            this.SwitchSettingsViewCommand = new RelayCommand(this.SwitchSettingsView);
            this.SwitchAboutViewCommand = new RelayCommand(this.SwitchAboutView);
        }

        // Return current view
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            protected set
            {
                if (_currentViewModel == value) { return; }
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        // Return current view name
        public string Button1Content
        {
            get { return _nextSettingsViewTitle; }
            protected set
            {
                if (_nextSettingsViewTitle == value) { return; }
                _nextSettingsViewTitle = value;
                RaisePropertyChanged("Button1Content");
            }
        }

        // Return current view name
        public string Button2Content
        {
            get { return _nextAboutViewTitle; }
            protected set
            {
                if (_nextAboutViewTitle == value) { return; }
                _nextAboutViewTitle = value;
                RaisePropertyChanged("Button2Content");
            }
        }

        // Switch settings view
        public void SwitchSettingsView()
        {
            if (_currentViewModel != _settingsViewModel)
            {
                CurrentViewModel = _settingsViewModel;
                Button1Content = _mainViewTitle;
                Button2Content = _aboutViewTitle;
            }
            else
            {
                CurrentViewModel = _tabsViewModel;
                Button1Content = _settingsViewTitle;
                Button2Content = _aboutViewTitle;
            }
        }

        // Switch about view
        public void SwitchAboutView()
        {
            if (_currentViewModel != _aboutViewModel)
            {
                CurrentViewModel = _aboutViewModel;
                Button1Content = _settingsViewTitle;
                Button2Content = _mainViewTitle;
            }
            else
            {
                CurrentViewModel = _tabsViewModel;
                Button1Content = _settingsViewTitle;
                Button2Content = _aboutViewTitle;
            }
        }

        // Commands
        public RelayCommand SwitchSettingsViewCommand { get; private set; }
        public RelayCommand SwitchAboutViewCommand { get; private set; }

    }
}
