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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using System;
using Baqsoft.CountryInfo;
using System.Diagnostics;

namespace Job_Hunter.ViewModel
{
    public class ApplicationViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        // Collections
        private ObservableCollection<ApplicationOrganizationVMItem> _applicationCollection;
        private ObservableCollection<OrganizationViewModelItem> _organizationCollection;
        private ObservableCollection<NextActionItem> _nextActionCollection;
        private ObservableCollection<JobTypeItem> _jobTypeCollection;
        private ObservableCollection<StatusItem> _statusCollection;
        private ObservableCollection<CountryViewModelItem> _countryCollection;

        // Selected application parameters
        private ApplicationOrganizationVMItem _selectedApplication;
        private string _selectedJobId;
        private string _selectedJobUrl;
        private string _selectedDescription;
        private string _selectedNote;

        // Add/edit parameters
        private long _id;
        private string _title;
        private string _jobId;
        private string _jobUrl;
        private string _jobType;
        private string _status;
        private int _priority;
        private string _nextAction;
        private string _nextActionDate;
        private OrganizationViewModelItem _organization;
        private string _iso2;
        private string _city;
        private string _contactName;
        private string _contactPhone;
        private string _contactEmail;
        private string _description;
        private string _note;
        
        private Visibility _lstVisibility;
        private Visibility _editVisibility;
        private bool _editFlag;     //True -> edit, False -> add.

        // Constructor
        public ApplicationViewModel(IDataService dataService)
        {
            // Initialize edit flag
            _editFlag = false;

            // Get reference to dataService
            _dataService = dataService;

            // Get country collection
            _countryCollection = new ObservableCollection<CountryViewModelItem>();
            _dataService.ActiveCountryList(FillCountryCollection);
            _dataService.CountryModified += (sender, e) => _dataService.ActiveCountryList(FillCountryCollection);

            // Get application collection
            _applicationCollection = new ObservableCollection<ApplicationOrganizationVMItem>();
            _dataService.ApplicationList(FillApplicationCollection);
            _dataService.ApplicationModified += (sender, e) => _dataService.ApplicationList(FillApplicationCollection);

            // Get organization collection
            _organizationCollection = new ObservableCollection<OrganizationViewModelItem>();
            _dataService.OrganizationList(FillOrganizationCollection);
            _dataService.OrganizationModified += (sender, e) => _dataService.OrganizationList(FillOrganizationCollection);

            // Get next action collection
            _nextActionCollection = new ObservableCollection<NextActionItem>();
            _dataService.NextActionList(FillNextActionCollection);
            _dataService.NextActionModified += (sender, e) => _dataService.NextActionList(FillNextActionCollection);

            // Get job type collection
            _jobTypeCollection = new ObservableCollection<JobTypeItem>();
            _dataService.JobTypeList(FillJobTypeCollection);
            _dataService.JobTypeModified += (sender, e) => _dataService.JobTypeList(FillJobTypeCollection);

            // Get status collection and subscribe to data modified events.
            _statusCollection = new ObservableCollection<StatusItem>();
            _dataService.StatusList(FillStatusCollection);
            _dataService.StatusModified += (sender, e) => _dataService.StatusList(FillStatusCollection);

            //Visibility defaults
            if (ViewModelBase.IsInDesignModeStatic)
            {
                _lstVisibility = Visibility.Visible;
                _editVisibility = Visibility.Collapsed;
                //_lstVisibility = Visibility.Collapsed;
                //_editVisibility = Visibility.Visible;
            }
            else
            {
                _lstVisibility = Visibility.Visible;
                _editVisibility = Visibility.Collapsed;
            }

            // Commands
            AddButtonCommand = new RelayCommand(AddCommand);
            EditButtonCommand = new RelayCommand(EditCommand);
            DeleteButtonCommand = new RelayCommand(DeleteCommand);
            OkButtonCommand = new RelayCommand(OkCommand);
            CancelButtonCommand = new RelayCommand(CancelCommand);
            SaveNotesButtonCommand = new RelayCommand(SaveNotesCommand);
            NavigateToJobUrlCommand = new RelayCommand(NavigateToJobUrl);
        }

        /*
         *
         * Collection fill callbacks and event handlers.
         * 
         */
        public void FillApplicationCollection(ApplicationOrganizationItem[] applicationArray)
        {
            ApplicationCollection.Clear();
            foreach (ApplicationOrganizationItem item in applicationArray)
            {
                ApplicationCollection.Add(new ApplicationOrganizationVMItem(item));
            }

            SelectedApplication = ApplicationCollection.Count() > 0 ? ApplicationCollection[0] : null;
        }

        public void FillOrganizationCollection(OrganizationItem[] organizationArray)
        {
            OrganizationCollection.Clear();
            foreach (OrganizationItem item in organizationArray)
            {
                OrganizationCollection.Add(new OrganizationViewModelItem(item));
            }

            Organization = OrganizationCollection.Count() > 0 ? OrganizationCollection[0] : null;
        }

        public void FillNextActionCollection(NextActionItem[] nextActionArray)
        {
            NextActionCollection = new ObservableCollection<NextActionItem>(nextActionArray);
        }

        public void FillJobTypeCollection(JobTypeItem[] jobTypeArray)
        {
            JobTypeCollection = new ObservableCollection<JobTypeItem>(jobTypeArray);
        }

        public void FillStatusCollection(StatusItem[] statusArray)
        {
            StatusCollection = new ObservableCollection<StatusItem>(statusArray);
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
            Iso2 = CountryCollection.Count() > 0 ? CountryCollection[0].Iso2Code : "";
        }

        /*
         * 
         * Selected application properties
         * 
         */
        public ApplicationOrganizationVMItem SelectedApplication
        {
            get { return _selectedApplication; }
            set
            {
                if (_selectedApplication == value) { return; }
                _selectedApplication = value;
                RaisePropertyChanged("SelectedApplication");

                if (_selectedApplication != null)
                {
                    //Set job id
                    SelectedJobId = _selectedApplication.JobId;

                    //Set job url
                    SelectedJobUrl = _selectedApplication.JobUrl;

                    //Set description
                    SelectedDescription = _selectedApplication.Description;

                    //Set note
                    SelectedNote = _selectedApplication.Note;
                }
                else
                {
                    SelectedJobId = SelectedJobUrl = SelectedDescription = SelectedNote = "";
                }
            }
        }

        public string SelectedJobId
        {
            get { return _selectedJobId; }
            set
            {
                if (_selectedJobId == value) { return; }
                _selectedJobId = value;
                RaisePropertyChanged("SelectedJobId");
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

        public string SelectedDescription
        {
            get { return _selectedDescription; }
            set
            {
                if (_selectedDescription == value) { return; }
                _selectedDescription = value;
                RaisePropertyChanged("SelectedDescription");
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

        /*
         * 
         * Add/edit application properties
         * 
         */
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value) { return; }
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        public string JobId
        {
            get { return _jobId; }
            set
            {
                if (_jobId == value) { return; }
                _jobId = value;
                RaisePropertyChanged("JobId");
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

        public string JobType
        {
            get { return _jobType; }
            set
            {
                if (_jobType == value) { return; }
                _jobType = value;
                RaisePropertyChanged("JobType");
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status == value) { return; }
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        public int Priority
        {
            get { return _priority; }
            set
            {
                if (_priority == value) { return; }
                _priority = value;
                RaisePropertyChanged("Priority");
            }
        }

        public string NextAction
        {
            get { return _nextAction; }
            set
            {
                if (_nextAction == value) { return; }
                _nextAction = value;
                RaisePropertyChanged("NextAction");
            }
        }

        public string NextActionDate
        {
            get { return _nextActionDate; }
            set
            {
                if (_nextActionDate == value) { return; }

                // Convert string value to short date or empty string.
                DateTime tmpDateTime;
                if (DateTime.TryParse(value, out tmpDateTime))
                {
                    _nextActionDate = tmpDateTime.ToShortDateString();
                }
                else
                {
                    _nextActionDate = "";
                }
                RaisePropertyChanged("NextActionDate");
            }
        }

        public OrganizationViewModelItem Organization
        {
            get { return _organization; }
            set
            {
                if (_organization == value) { return; }
                _organization = value;
                RaisePropertyChanged("Organization");
                if (_organization != null && !_editFlag)
                {
                    Iso2 = _organization.Iso2Code;
                    City = _organization.City;
                    ContactName = _organization.ContactName;
                    ContactPhone = _organization.ContactPhone;
                    ContactEmail = _organization.ContactEmail;
                }
            }
        }

        public string Iso2
        {
            get { return _iso2; }
            set
            {
                if (_iso2 == value) { return; }
                _iso2 = value;
                RaisePropertyChanged("Iso2");
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

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) { return; }
                _description = value;
                RaisePropertyChanged("Description");
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
         * Collections
         * 
         */
        //Application collection
        public ObservableCollection<ApplicationOrganizationVMItem> ApplicationCollection
        {
            get { return _applicationCollection; }
            set
            {
                if (_applicationCollection == value) { return; }
                _applicationCollection = value;
                RaisePropertyChanged("ApplicationCollection");
            }
        }

        //Organization collection
        public ObservableCollection<OrganizationViewModelItem> OrganizationCollection
        {
            get { return _organizationCollection; }
            set
            {
                if (_organizationCollection == value) { return; }
                _organizationCollection = value;
                RaisePropertyChanged("OrganizationCollection");
            }
        }

        //Status
        public ObservableCollection<StatusItem> StatusCollection
        {
            get { return _statusCollection; }
            set
            {
                if (_statusCollection == value) { return; }
                _statusCollection = value;
                RaisePropertyChanged("StatusCollection");
            }
        }

        //Next actions
        public ObservableCollection<NextActionItem> NextActionCollection
        {
            get { return _nextActionCollection; }
            set
            {
                if (_nextActionCollection == value) { return; }
                _nextActionCollection = value;
                RaisePropertyChanged("NextActionCollection");
            }
        }

        //Job type collection
        public ObservableCollection<JobTypeItem> JobTypeCollection
        {
            get { return _jobTypeCollection; }
            set
            {
                if (_jobTypeCollection == value) { return; }
                _jobTypeCollection = value;
                RaisePropertyChanged("JobTypeCollection");
            }
        }

        // Priority list
        public int[] PriorityArray
        {
            get
            {
                return new int[] { 0, 1, 2, 3, 4, 5 };
            }
            private set { return; }
        }

        // Country collection
        public ObservableCollection<CountryViewModelItem> CountryCollection
        {
            get { return _countryCollection; }
            set
            {
                if (_countryCollection == value) { return; }
                _countryCollection = value;
                RaisePropertyChanged("CountryCollection");
            }
        }

        /*
         * 
         * List/edit forms visibility
         * 
         */
        // List visibility
        public Visibility ListVisibility
        {
            get { return _lstVisibility; }
            set
            {
                if (_lstVisibility == value) { return; }
                _lstVisibility = value;
                RaisePropertyChanged("ListVisibility");
            }
        }

        // Design visibility
        public Visibility EditVisibility
        {
            get { return _editVisibility; }
            set
            {
                if (_editVisibility == value) { return; }
                _editVisibility = value;
                RaisePropertyChanged("EditVisibility");
            }
        }

        /*
         * 
         * Commands
         * 
         */
        public async void AddCommand()
        {
            //Prepare dialog service
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            string[] buttonText = new string[] { "Ok" };

            // Create new item and initialize
            if (_organizationCollection.Count > 0)
            {
                Organization = _organizationCollection[0];
            }
            else
            {
                int dialogResult = await dialogService.ShowMessageDialog("Add organizations first", "ERROR", buttonText);
                return;
            }

            //Adjust edit flag
            _editFlag = false;

            // Initialize fields
            Title = "";
            JobId = "";
            JobUrl = "";
            JobType = "";
            Status = "";
            Priority = 0;
            NextAction = "";
            NextActionDate = "";
            City = "";

            //Adjust visibility
            ListVisibility = Visibility.Collapsed;
            EditVisibility = Visibility.Visible;
        }

        public void EditCommand()
        {
            //Adjust edit flag
            _editFlag = true;

            // Initialize fields
            _id = SelectedApplication.Id;
            Title = SelectedApplication.Title;
            JobId = SelectedApplication.JobId;
            JobUrl = SelectedApplication.JobUrl;
            JobType = SelectedApplication.JobType;
            Status = SelectedApplication.Status;
            Priority = SelectedApplication.Priority;
            NextAction = SelectedApplication.NextAction;
            NextActionDate = SelectedApplication.NextActionDate;
            Iso2 = SelectedApplication.Iso2;
            City = SelectedApplication.City;
            ContactName = SelectedApplication.ContactName;
            ContactPhone = SelectedApplication.ContactPhone;
            ContactEmail = SelectedApplication.ContactEmail;
            Description = SelectedApplication.Description;
            Note = SelectedApplication.Note;

            // Select organization
            OrganizationViewModelItem[] organizationArray = (from item in _organizationCollection
                                                             where item.Id == SelectedApplication.OrganizationId
                                                             select item).ToArray();
            if (organizationArray.Count() > 0) { Organization = organizationArray[0]; }

            //Adjust visibility
            ListVisibility = Visibility.Collapsed;
            EditVisibility = Visibility.Visible;
        }

        public async void DeleteCommand()
        {
            //Request delete confirmation
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            string[] buttonText = new string[] { "Ok", "Cancel" };

            int dialogResult = await dialogService.ShowMessageDialog("Delete \"" + SelectedApplication.Description + "\"?", "DELETE", buttonText);

            if (dialogResult == 0)
            {
                //Delete the application
                if (ApplicationCollection.Count() > 0)
                {
                    _dataService.DeleteApplication(SelectedApplication.GetApplicationItem());
                }
            }
        }

        public async void OkCommand()
        {
            //Prepare dialog service
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            string[] buttonText = new string[] { "Ok" };

            //Verify that an application description has been introduced
            if (String.IsNullOrWhiteSpace(Title))
            {
                int dialogResult = await dialogService.ShowMessageDialog("Missing application title", "ERROR", buttonText);
                return;
            }

            //Edit
            if(_editFlag)
            {
                //Update application data
                _dataService.UpdateApplication(new ApplicationItem(_id, Title, JobId, JobUrl, JobType, Status, Priority,
                    NextAction, NextActionDate, Organization.Id, Iso2, City, ContactName, ContactPhone, ContactEmail,
                    Description, Note));
            }

            //Add
            else
            {
                //Add application data
                _dataService.AddApplication(new ApplicationItem(-1, Title, JobId, JobUrl, JobType, Status, Priority, 
                    NextAction, NextActionDate, Organization.Id, Iso2, City, ContactName, ContactPhone, ContactEmail, 
                    Description, Note));
            }

            //Adjust visibility
            ListVisibility = Visibility.Visible;
            EditVisibility = Visibility.Collapsed;
        }

        public void CancelCommand()
        {
            ListVisibility = Visibility.Visible;
            EditVisibility = Visibility.Collapsed;
        }

        public async void SaveNotesCommand()
        {
            string applicationTitle = SelectedApplication.Title;

            //Update application data
            _dataService.UpdateApplication(new ApplicationItem(SelectedApplication.Id, SelectedApplication.Title, SelectedApplication.JobId, 
                SelectedApplication.JobUrl, SelectedApplication.JobType, SelectedApplication.Status, SelectedApplication.Priority,
                SelectedApplication.NextAction, SelectedApplication.NextActionDate, SelectedApplication.OrganizationId,
                SelectedApplication.Iso2, SelectedApplication.City, SelectedApplication.ContactName, SelectedApplication.ContactPhone,
                SelectedApplication.ContactEmail, SelectedApplication.Description, SelectedNote));

            //Notify notes saved
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            string[] buttonText = new string[] { "Ok" };
            int dialogResult = await dialogService.ShowMessageDialog("\"" + applicationTitle + "\" note saved", "SAVE", buttonText);
        }

        public void NavigateToJobUrl()
        {
            OpenUrl(SelectedJobUrl);
        }

        private async void OpenUrl(string urlString)
        {
            urlString = urlString.StartsWith("http://") ? urlString : "http://" + urlString;
            bool urlError = false;
            try
            {
                Process.Start(new ProcessStartInfo(urlString));
            }
            catch (Exception e)
            {
                urlError = true;
            }
            if (urlError)
            {
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };

                int dialogResult = await dialogService.ShowMessageDialog("An error occurred while opening " + urlString + "\nInvalid Url?", "Error", buttonText);
            }
        }

        /*
         * 
         * Relay commands.
         * 
         */
        public RelayCommand AddButtonCommand { get; private set; }
        public RelayCommand EditButtonCommand { get; private set; }
        public RelayCommand DeleteButtonCommand { get; private set; }
        public RelayCommand OkButtonCommand { get; set; }
        public RelayCommand CancelButtonCommand { get; private set; }
        public RelayCommand SaveNotesButtonCommand { get; private set; }
        public RelayCommand NavigateToJobUrlCommand { get; private set; }
    }
}