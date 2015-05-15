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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using Baqsoft.CountryInfo;
using System.Diagnostics;

namespace Job_Hunter.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class OrganizationViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        //DataCollections data;
        private ObservableCollection<OrganizationViewModelItem> _organizationCollection;
        private OrganizationViewModelItem _selectedOrganization;
        private string _organization;
        private string _division;
        private string _city;
        private string _contactName;
        private string _contactPhone;
        private string _contactEmail;
        private string _url;
        private string _jobUrl;
        private string _note;
        private string _selectedNote;
        private string _selectedJobUrl;
        private string _selectedUrl;
        private Collection<CountryViewModelItem> _countryCollection;
        private CountryViewModelItem _selectedCountry;
        private bool addOperation;     // True -> Add, False -> Edit
        private Visibility _notesVisibility;
        private Visibility _editVisibility;

        /// <summary>
        /// Initializes a new instance of the OrganizationViewModel class.
        /// </summary>
        public OrganizationViewModel(IDataService dataService)
        {
            // Initialize data services
            _dataService = dataService;

            // Fill country list
            _countryCollection = new Collection<CountryViewModelItem>();
            _dataService.ActiveCountryList(FillCountryCollection);
            _dataService.CountryModified += (sender, e) => _dataService.ActiveCountryList(FillCountryCollection);

            // Fill organization collection
            _organizationCollection = new ObservableCollection<OrganizationViewModelItem>();
            _dataService.OrganizationList(FillOrganizationCollection);
            _dataService.OrganizationModified += (sender, e) => _dataService.OrganizationList(FillOrganizationCollection);

            // Default visibility
            _notesVisibility = Visibility.Visible;
            _editVisibility = Visibility.Collapsed;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                _notesVisibility = Visibility.Collapsed;
                _editVisibility = Visibility.Visible;
            }
            else
            {
                _notesVisibility = Visibility.Visible;
                _editVisibility = Visibility.Collapsed;
            }

            // Commands
            AddButtonCommand = new RelayCommand(AddCommand);
            EditButtonCommand = new RelayCommand(EditCommand);
            DeleteButtonCommand = new RelayCommand(DeleteCommand);
            CancelButtonCommand = new RelayCommand(CancelCommand);
            SaveButtonCommand = new RelayCommand(SaveCommand);
            SaveNotesButtonCommand = new RelayCommand(SaveNotesCommand);
            NavigateToUrlCommand = new RelayCommand(NavigateToUrl);
            NavigateToJobsUrlCommand = new RelayCommand(NavigateToJobsUrl);
        }

        /*
         * 
         * Collection fill callback
         * 
         */
        public void FillOrganizationCollection(OrganizationItem[] organizationArray)
        {
            _organizationCollection.Clear();
            foreach(OrganizationItem item in organizationArray)
            {
                _organizationCollection.Add(new OrganizationViewModelItem(item));
            }

            // Default organization item
            SelectedOrganization = OrganizationCollection.Count > 0 ? OrganizationCollection[0] : null;
        }

        public void FillCountryCollection(ActiveCountryItem[] activeCountryArray)
        {
            List<CountryViewModelItem> activeCountryList = new List<CountryViewModelItem>();

            // Get country data for active countries.
            foreach (ActiveCountryItem activeCountry in activeCountryArray)
            {
                Country country = CountryArray.GetCountry(activeCountry.Iso2Code);
                activeCountryList.Add(new CountryViewModelItem(activeCountry.Id, activeCountry.Iso2Code, country.Name, activeCountry.Note, country.Flag));
            }
            // Make new observable collections from lists.
            activeCountryList.Sort();
            CountryCollection = new ObservableCollection<CountryViewModelItem>(activeCountryList.ToArray());
        }

        /*
         * 
         * Collections
         * 
         */
        public ObservableCollection<OrganizationViewModelItem> OrganizationCollection
        {
            get { return _organizationCollection; }
            private set
            {
                if (_organizationCollection == value) { return; }
                _organizationCollection = value;
                RaisePropertyChanged("OrganizationCollection");
            }
        }

        public Collection<CountryViewModelItem> CountryCollection
        {
            get { return _countryCollection; }
            private set
            {
                if (_countryCollection == value) { return; }
                _countryCollection = value;
                RaisePropertyChanged("CountryCollection");
            }
        }

        /*
         * 
         * Selected item properties
         * 
         */
        public OrganizationViewModelItem SelectedOrganization
        {
            get { return _selectedOrganization; }
            set
            {
                if (_selectedOrganization == value) { return; }
                _selectedOrganization = value;
                RaisePropertyChanged("SelectedOrganization");
                if (_selectedOrganization != null)
                {
                    // Update selected note
                    SelectedNote = _selectedOrganization.Note;

                    // Update selected Url
                    SelectedUrl = _selectedOrganization.Url;

                    // Update selected job url
                    SelectedJobUrl = _selectedOrganization.JobsUrl;
                }
            }
        }

        public string SelectedNote
        {
            get { return _selectedNote; }
            set
            {
                if (_selectedNote == value) { return; }
                _selectedNote = value;
                RaisePropertyChanged("SelectedNote");
            }
        }

        public string SelectedUrl
        {
            get { return _selectedUrl; }
            set
            {
                if (_selectedUrl == value) { return; }
                _selectedUrl = value;
                RaisePropertyChanged("SelectedUrl");
            }
        }

        public string SelectedJobUrl
        {
            get { return _selectedJobUrl; }
            set
            {
                if (_selectedJobUrl == value) { return; }
                _selectedJobUrl = value;
                RaisePropertyChanged("SelectedJobUrl");
            }
        }

        /*
         * 
         * Add/Edit properties
         *           
         */
        public string Organization
        {
            get { return _organization; }
            set
            {
                if (_organization == value) { return; }
                _organization = value;
                RaisePropertyChanged("Organization");
            }
        }

        public string Division
        {
            get { return _division; }
            set
            {
                if (_division == value) { return; }
                _division = value;
                RaisePropertyChanged("Division");
            }
        }

        public CountryViewModelItem SelCountry
        {
            get { return _selectedCountry; }
            set
            {
                if (_selectedCountry == value) { return; }
                _selectedCountry = value;
                RaisePropertyChanged("SelCountry");
            }
        }

        public string City
        {
            get { return _city; }
            set
            {
                if (_city == value) { return; }
                _city = value;
                RaisePropertyChanged("City");
            }
        }

        public string ContactName
        {
            get { return _contactName; }
            set
            {
                if (_contactName == value) { return; }
                _contactName = value;
                RaisePropertyChanged("ContactName");
            }
        }

        public string ContactPhone
        {
            get { return _contactPhone; }
            set
            {
                if (_contactPhone == value) { return; }
                _contactPhone = value;
                RaisePropertyChanged("ContactPhone");
            }
        }

        public string ContactEmail
        {
            get { return _contactEmail; }
            set
            {
                if (_contactEmail == value) { return; }
                _contactEmail = value;
                RaisePropertyChanged("ContactEmail");
            }
        }

        public string Url
        {
            get { return _url; }
            set
            {
                if (_url == value) { return; }
                _url = value;
                RaisePropertyChanged("Url");
            }
        }

        public string JobUrl
        {
            get { return _jobUrl; }
            set
            {
                if (_jobUrl == value) { return; }
                _jobUrl = value;
                RaisePropertyChanged("JobUrl");
            }
        }

        public string Note
        {
            get { return _note; }
            set
            {
                if (_note == value) { return; }
                _note = value;
                RaisePropertyChanged("Note");
            }
        }

        /*
         * 
         * Visibility
         * 
         */

        public Visibility NotesVisibility
        {
            get { return _notesVisibility; }
            private set
            {
                if (_notesVisibility == value) { return; }
                _notesVisibility = value;
                RaisePropertyChanged("NotesVisibility");
            }
        }

        public Visibility EditVisibility
        {
            get { return _editVisibility; }
            private set
            {
                if (_editVisibility == value) { return; }
                _editVisibility = value;
                RaisePropertyChanged("EditVisibility");
            }
        }

        /*
         *           Commands
         */
        public void AddCommand()
        {
            // Initialize data
            Organization = "";
            Division = "";
            City = "";
            ContactName = "";
            ContactPhone = "";
            ContactEmail = "";
            Url = "";
            JobUrl = "";
            Note = "";
            NotesVisibility = Visibility.Collapsed;
            EditVisibility = Visibility.Visible;

            addOperation = true;
        }

        public void EditCommand()
        {
            if (SelectedOrganization != null)
            {
                // Initialize edit data
                Organization = SelectedOrganization.Organization;
                Division = SelectedOrganization.Division;
                City = SelectedOrganization.City;
                ContactName = SelectedOrganization.ContactName;
                ContactPhone = SelectedOrganization.ContactPhone;
                ContactEmail = SelectedOrganization.ContactEmail;
                Url = SelectedOrganization.Url;
                JobUrl = SelectedOrganization.JobsUrl;
                Note = SelectedOrganization.Note;

                // Initialize selected country
                CountryViewModelItem[] selCountry = (from selected_country in _countryCollection
                                                     where selected_country.Iso2Code == SelectedOrganization.Iso2Code
                                                     select selected_country).ToArray();
                Console.Out.WriteLine("Found " + selCountry.Count() + " countries");
                if (selCountry.Count() > 0)
                {
                    SelCountry = selCountry[0];
                    Console.Out.WriteLine("Selected country: " + selCountry[0].Name);
                }

                // Adjust visibility
                NotesVisibility = Visibility.Collapsed;
                EditVisibility = Visibility.Visible;

                // Set edit flag
                addOperation = false;
            }
        }

        public void CancelCommand()
        {
            NotesVisibility = Visibility.Visible;
            EditVisibility = Visibility.Collapsed;
        }

        public async void DeleteCommand()
        {
            if (_selectedOrganization != null)
            {
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok", "Cancel" };

                int dialogResult = await dialogService.ShowMessageDialog("Delete \"" + SelectedOrganization.Organization + " - " + SelectedOrganization.Division + " in " + SelCountry.Name + "\"?", "DELETE", buttonText);

                if (dialogResult == 0)
                {
                    _dataService.DeleteOrganization(SelectedOrganization.GetOrganizationItem());
                }
            }
        }

        public async void SaveCommand()
        {
            // Check that organization name is not empty
            if (Organization == "")
            {
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };

                int dialogResult = await dialogService.ShowMessageDialog("Empty organization name", "Error", buttonText);
                return;
            }

            // Add or update organization info.
            if (addOperation)
            {
                //Check that organization and division is not a duplicate
                OrganizationViewModelItem[] organizationList = (from organization in _organizationCollection
                                                                where organization.Organization == Organization && organization.Division == Division 
                                                                && organization.Iso2Code == SelCountry.Iso2Code
                                                                select organization).ToArray();
                if (organizationList.Count() > 0)
                {
                    IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                    string[] buttonText = new string[] { "Ok" };

                    int dialogResult = await dialogService.ShowMessageDialog("Duplicate organization - division in the same country", "Error", buttonText);
                    return;
                }

                // Add the organization.
                _dataService.AddOrganization(new OrganizationItem(-1, Organization, Division, SelCountry.Iso2Code, City, ContactName, 
                    ContactPhone, ContactEmail, Url, JobUrl, Note));
            }
            else
            {
                // Check that organization, division and country is not a duplicate
                int currentSelection = _organizationCollection.IndexOf(SelectedOrganization);
                for (int i = 0; i < _organizationCollection.Count; i++)
                {
                    if (_organizationCollection[i].Organization == Organization && _organizationCollection[i].Division == Division
                        && _organizationCollection[i].Iso2Code == SelCountry.Iso2Code && i != currentSelection)
                    {
                        IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                        string[] buttonText = new string[] { "Ok" };

                        int dialogResult = await dialogService.ShowMessageDialog("Duplicate organization - division in the same country", "Error", buttonText);
                        return;
                    }
                }

                // Update organization.
                _dataService.UpdateOrganization(new OrganizationItem(SelectedOrganization.Id, Organization, Division, SelCountry.Iso2Code, 
                    City, ContactName, ContactPhone, ContactEmail, Url, JobUrl, Note));
            }

            // Update visibility
            NotesVisibility = Visibility.Visible;
            EditVisibility = Visibility.Collapsed;
        }

        public async void SaveNotesCommand()
        {
            string selOrganization = SelectedOrganization.Organization;

            if (SelectedOrganization.Note != SelectedNote)
            {
                // Update organization note.
                _dataService.UpdateOrganization(new OrganizationItem(SelectedOrganization.Id, SelectedOrganization.Organization, 
                    SelectedOrganization.Division, SelectedOrganization.Iso2Code, SelectedOrganization.City, SelectedOrganization.ContactName, 
                    SelectedOrganization.ContactPhone, SelectedOrganization.ContactEmail, SelectedOrganization.Url, SelectedOrganization.JobsUrl, 
                    SelectedNote));

                // Notify note saved
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };
                await dialogService.ShowMessageDialog("\"" + selOrganization + "\" note saved", "SAVE", buttonText);
            }
        }

        public void NavigateToUrl()
        {
            ViewModelHelper.OpenUrl(SelectedOrganization.Url);
        }

        public void NavigateToJobsUrl()
        {
            ViewModelHelper.OpenUrl(SelectedOrganization.JobsUrl);
        }

        // Commands
        public RelayCommand AddButtonCommand { get; private set; }
        public RelayCommand EditButtonCommand { get; private set; }
        public RelayCommand CancelButtonCommand { get; private set; }
        public RelayCommand DeleteButtonCommand { get; private set; }
        public RelayCommand SaveButtonCommand { get; private set; }
        public RelayCommand SaveNotesButtonCommand { get; private set; }
        public RelayCommand NavigateToUrlCommand { get; private set; }
        public RelayCommand NavigateToJobsUrlCommand { get; private set; }
    }
}