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
using Job_Hunter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Baqsoft.CountryInfo;
using Job_Hunter.View;
using GalaSoft.MvvmLight.Ioc;

namespace Job_Hunter.ViewModel
{
    public class CountryViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private ObservableCollection<CountryViewModelItem> _activeCountryCollection;
        private ObservableCollection<CountryViewModelItem> _inactiveCountryCollection;
        private CountryViewModelItem _countryItem;
        private CountryViewModelItem _addCountryItem;
        private CountryViewModelItem _removeCountryItem;
        private Visibility _manageCountryVisibility;
        private Visibility _useCountryVisibility;

        // Constructor
        public CountryViewModel(IDataService dataService)
        {
            // Get reference to dataService
            _dataService = dataService;

            // Fill country collections
            _activeCountryCollection = new ObservableCollection<CountryViewModelItem>();
            _inactiveCountryCollection = new ObservableCollection<CountryViewModelItem>();
            _dataService.ActiveCountryList(FillCountryCollections);
            _dataService.CountryModified += (sender, e) => _dataService.ActiveCountryList(FillCountryCollections);

            // Default control states.
            _manageCountryVisibility = Visibility.Collapsed;
            _useCountryVisibility = Visibility.Visible;

            // Commands
            ManageCountryButtonCommand = new RelayCommand(ManageCountryCommand);
            AddCountryButtonCommand = new RelayCommand(AddCountryCommand);
            RemoveCountryButtonCommand = new RelayCommand(RemoveCountryCommand);
            DoneButtonCommand = new RelayCommand(DoneCountryCommand);
            SaveNotesButtonCommand = new RelayCommand(SaveNotesCommand);
        }

        public void FillCountryCollections(ActiveCountryItem[] activeCountryArray)
        {
            List<CountryViewModelItem> activeCountryList = new List<CountryViewModelItem>();
            List<CountryViewModelItem> inactiveCountryList = new List<CountryViewModelItem>();

            for (int count = 0; count < CountryArray.Length; count++)
            {
                Country country = CountryArray.GetCountry(count);

                ActiveCountryItem[] item = (from active_country in activeCountryArray
                                            where active_country.Iso2Code == country.Iso2
                                            select active_country).ToArray();
                if (item.Count() > 0)
                {
                    activeCountryList.Add(new CountryViewModelItem(item[0].Id, item[0].Iso2Code, country.Name, item[0].Note, country.Flag));
                }
                else
                {
                    inactiveCountryList.Add(new CountryViewModelItem(-1, country.Iso2, country.Name, "", country.Flag));
                }
            }

            // Make new observable collections from lists.
            activeCountryList.Sort();
            inactiveCountryList.Sort();
            ActiveCountryCollection = new ObservableCollection<CountryViewModelItem>(activeCountryList.ToArray());
            InactiveCountryCollection = new ObservableCollection<CountryViewModelItem>(inactiveCountryList.ToArray());

            // Default listbox selections.
            SelectedCountryItem = SelectedRemoveCountryItem = ActiveCountryCollection.Count() > 0 ? ActiveCountryCollection[0] : null;
            SelectedAddCountryItem = InactiveCountryCollection.Count() > 0 ? InactiveCountryCollection[0] : null;
        }

        // Return the active country collection.
        public ObservableCollection<CountryViewModelItem> ActiveCountryCollection
        {
            get { return _activeCountryCollection; }
            private set
            {
                if (_activeCountryCollection == value) { return; }
                _activeCountryCollection = value;
                RaisePropertyChanged("ActiveCountryCollection");
            }
        }

        // Return the inactive country collection.
        public ObservableCollection<CountryViewModelItem> InactiveCountryCollection
        {
            get { return _inactiveCountryCollection; }

            private set
            {
                if (_inactiveCountryCollection == value) { return; }
                _inactiveCountryCollection = value;
                RaisePropertyChanged("InactiveCountryCollection");
            }
        }

        // Selected country item
        public CountryViewModelItem SelectedCountryItem
        {
            get { return _countryItem; }
            set
            {
                if (_countryItem == value) { return; }
                _countryItem = value;
                RaisePropertyChanged("SelectedCountryItem");
                RaisePropertyChanged("SelectedCountryItemNotes");
            }
        }

        // Selected country item notes
        public string SelectedCountryItemNotes
        {
            get 
            {
                if (_countryItem != null)
                {
                    return _countryItem.Note;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (_countryItem.Note == value) { return; }
                _countryItem.Note = value;
                RaisePropertyChanged("SelectedCountryItemNotes");
            }
        }

        // Add selected country item
        public CountryViewModelItem SelectedAddCountryItem
        {
            get { return _addCountryItem; }
            set
            {
                if (_addCountryItem == value) { return; }
                _addCountryItem = value;
                RaisePropertyChanged("SelectedAddCountryItem");
            }
        }

        // Remove selected country item
        public CountryViewModelItem SelectedRemoveCountryItem
        {
            get { return _removeCountryItem; }
            set
            {
                if (_removeCountryItem == value) { return; }
                _removeCountryItem = value;
                RaisePropertyChanged("SelectedRemoveCountryItem");
            }
        }

        // Country manager visibility.
        public Visibility ManageCountryVisibility
        {
            get { return _manageCountryVisibility; }
            protected set 
            {
                if (_manageCountryVisibility == value) { return; }
                _manageCountryVisibility = value;
                RaisePropertyChanged("ManageCountryVisibility");
            }
        }

        // Country notes visibility.
        public Visibility UseCountryVisibility
        {
            get { return _useCountryVisibility; }
            protected set
            {
                if (_useCountryVisibility == value) { return; }
                _useCountryVisibility = value;
                RaisePropertyChanged("UseCountryVisibility");
            }
        }

        // Manage button command.
        public void ManageCountryCommand()
        {
            UseCountryVisibility = Visibility.Collapsed;
            ManageCountryVisibility = Visibility.Visible;
        }

        // Add button command.
        public void AddCountryCommand()
        {
            _dataService.AddActiveCountry(_addCountryItem.GetCountryItem());
        }

        // Remove button command.
        public async void RemoveCountryCommand()
        {
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            string[] buttonText = new string[] { "Ok", "Cancel" };

            string message = "Remove \"" + _removeCountryItem.Name + "\" from active countries list?\nThis will not modify existing records.";
            int dialogResult = await dialogService.ShowMessageDialog(message, "DELETE", buttonText);

            if (dialogResult == 0)
            {
                _dataService.DeleteActiveCountry(_removeCountryItem.GetCountryItem());
            }
        }

        // Save button command.
        public void SaveNotesCommand()
        {
            _dataService.UpdateActiveCountryNotes(_countryItem.GetCountryItem());
        }

        // Done button command.
        public void DoneCountryCommand()
        {
            UseCountryVisibility = Visibility.Visible;
            ManageCountryVisibility = Visibility.Collapsed;
        }

        // Commands
        public RelayCommand ManageCountryButtonCommand { get; private set; }
        public RelayCommand AddCountryButtonCommand { get; private set; }
        public RelayCommand RemoveCountryButtonCommand { get; private set; }
        public RelayCommand DoneButtonCommand { get; private set; }
        public RelayCommand SaveNotesButtonCommand { get; private set; }
    }
}
