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
    public class JobTypeViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private ObservableCollection<JobTypeItem> _jobTypeCollection;
        private JobTypeItem _selectedJobType;
        private string _selectedNote;
        private Visibility _notesVisibility;
        private Visibility _addJobTypeVisibility;
        private bool _jobTypeListEnabled;
        private string _jobType;
        private string _note;
        private bool editFlag;      // True -> edit, False -> add

        /// <summary>
        /// Initializes a new instance of the JobTypesViewModel class.
        /// </summary>
        public JobTypeViewModel(IDataService dataService)
        {
            // Get reference to dataService
            _dataService = dataService;

            // Fill job type collection
            _jobTypeCollection = new ObservableCollection<JobTypeItem>();
            _dataService.JobTypeList(FillJobTypeCollection);
            _dataService.JobTypeModified += (sender, e) => _dataService.JobTypeList(FillJobTypeCollection);

            // Default visibility
            _notesVisibility = Visibility.Visible;
            _addJobTypeVisibility = Visibility.Collapsed;

            // Default list state
            _jobTypeListEnabled = true;

            // Commands
            AddButtonCommand = new RelayCommand(AddCommand);
            EditButtonCommand = new RelayCommand(EditCommand);
            DeleteButtonCommand = new RelayCommand(DeleteCommand);
            UpButtonCommand = new RelayCommand(UpCommand);
            DownButtonCommand = new RelayCommand(DownCommand);
            OkButtonCommand = new RelayCommand(OkCommand);
            CancelButtonCommand = new RelayCommand(CancelCommand);
            SaveButtonCommand = new RelayCommand(SaveCommand);
        }

        /*
         *
         * Collection fill callback
         *
         */
        public void FillJobTypeCollection(JobTypeItem[] jobTypeArray)
        {
            JobTypeCollection = new ObservableCollection<JobTypeItem>(jobTypeArray);

            // Default job type
            SelectedJobType = JobTypeCollection.Count > 0 ? JobTypeCollection[0] : null;
        }

        /*
         * 
         * Collection
         * 
         */
        public ObservableCollection<JobTypeItem> JobTypeCollection
        {
            get { return _jobTypeCollection; }
            private set
            {
                _jobTypeCollection = value;
                RaisePropertyChanged("JobTypeCollection");
            }
        }

        /*
         * 
         * Selected job type properties
         * 
         */
        public JobTypeItem SelectedJobType
        {
            get { return _selectedJobType; }
            set
            {
                if (_selectedJobType == value) { return; }
                _selectedJobType = value;
                RaisePropertyChanged("SelectedJobType");
                if (_selectedJobType != null)
                {
                    SelectedNote = _selectedJobType.Note;
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

        /*
         * 
         * Add/edit job type properties
         * 
         */
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
         * Commands
         * 
         */
        public void AddCommand()
        {
            // Set edit flag
            editFlag = false;

            // Adjust visibility
            NoteVisibility = Visibility.Collapsed;
            AddJobTypeVisibility = Visibility.Visible;
            
            // Disable job type list
            JobTypeListEnabled = false;

            // Initialize data entry fields
            JobType = Note = "";
        }

        public void EditCommand()
        {
            // Set edit flag
            editFlag = true;

            // Adjust visibility
            NoteVisibility = Visibility.Collapsed;
            AddJobTypeVisibility = Visibility.Visible;

            // Disable job type list
            JobTypeListEnabled = false;

            // Initialize data entry fields
            JobType = SelectedJobType.JobType;
            Note = SelectedJobType.Note;
        }

        public async void DeleteCommand()
        {
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            string[] buttonText = new string[] { "Ok", "Cancel" };

            int dialogResult = await dialogService.ShowMessageDialog("Delete \""
                + _selectedJobType.JobType + "\"?", "DELETE", buttonText);

            if (dialogResult == 0)
            {
                _dataService.DeleteJobType(SelectedJobType);
            }
        }

        public void OkCommand()
        {
            // Check that a job type text has been entered. If not notify error.
            if (_jobType != "")
            {
                // Update or add entry
                if (editFlag)
                {
                    // Update job type
                    _dataService.UpdateJobType(new JobTypeItem(SelectedJobType.Id, SelectedJobType.Position, 
                        JobType, Note));
                }
                else
                {
                    // Search next available position
                    long maxPosition = 0;
                    foreach (JobTypeItem item in _jobTypeCollection)
                    {
                        maxPosition = item.Position > maxPosition ? item.Position : maxPosition;
                    }

                    // Add new job type
                    _dataService.AddJobType(new JobTypeItem(-1, ++maxPosition, _jobType, _note));
                    JobType = Note = "";
                }

                // Enable job type list
                JobTypeListEnabled = true;

                // Adjust visibility
                NoteVisibility = Visibility.Visible;
                AddJobTypeVisibility = Visibility.Collapsed;
            }
            else
            {
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };
                dialogService.ShowMessageDialog("Empty job type", "ERROR", buttonText);
            }
        }

        public void CancelCommand()
        {
            NoteVisibility = Visibility.Visible;
            AddJobTypeVisibility = Visibility.Collapsed;
            JobTypeListEnabled = true;
            JobType = Note = "";
        }

        public async void SaveCommand()
        {
            if (JobTypeCollection.Count > 0)
            {
                string jobTypeTitle = SelectedJobType.JobType;

                // Update job type
                _dataService.UpdateJobType(new JobTypeItem(SelectedJobType.Id, SelectedJobType.Position, 
                    SelectedJobType.JobType, SelectedNote));

                // Notify note saved
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };
                await dialogService.ShowMessageDialog("\"" + _selectedJobType.JobType + "\" notes saved", "SAVE", buttonText);
            }
        }

        public void UpCommand()
        {
            // If current position is first nothing to do
            if (SelectedJobType.Position == 0) { return; }

            // Search for the item in the previous position
            JobTypeItem previousItem = null;
            foreach (JobTypeItem item in JobTypeCollection)
            {
                if (item.Position == SelectedJobType.Position - 1)
                {
                    previousItem = item;
                    break;
                }
            }

            // Update the items
            _dataService.UpdateJobTypeList(new JobTypeItem[]
            {
                new JobTypeItem(previousItem.Id, previousItem.Position + 1, previousItem.JobType, previousItem.Note),
                new JobTypeItem(SelectedJobType.Id, SelectedJobType.Position - 1, SelectedJobType.JobType, SelectedJobType.Note)
            });
        }

        public void DownCommand()
        {
            // If current position is first nothing to do
            if (SelectedJobType.Position == JobTypeCollection.Count - 1) { return; }

            // Search for the item in the previous position
            JobTypeItem nextItem = null;
            foreach (JobTypeItem item in JobTypeCollection)
            {
                if (item.Position == SelectedJobType.Position + 1)
                {
                    nextItem = item;
                    break;
                }
            }

            // Update the items
            _dataService.UpdateJobTypeList(new JobTypeItem[]
            {
                new JobTypeItem(nextItem.Id, nextItem.Position - 1, nextItem.JobType, nextItem.Note),
                new JobTypeItem(SelectedJobType.Id, SelectedJobType.Position + 1, SelectedJobType.JobType, SelectedJobType.Note)
            });
        }

        /*
         * 
         * Note/add-edit form visibility
         * 
         */
        public Visibility NoteVisibility
        {
            get { return _notesVisibility; }
            private set
            {
                if (_notesVisibility == value) { return; }
                _notesVisibility = value;
                RaisePropertyChanged("NoteVisibility");
            }
        }

        public Visibility AddJobTypeVisibility
        {
            get { return _addJobTypeVisibility; }
            private set
            {
                if (_addJobTypeVisibility == value) { return; }
                _addJobTypeVisibility = value;
                RaisePropertyChanged("AddJobTypeVisibility");
            }
        }

        public bool JobTypeListEnabled
        {
            get { return _jobTypeListEnabled; }
            private set
            {
                if (_jobTypeListEnabled == value) { return; }
                _jobTypeListEnabled = value;
                RaisePropertyChanged("JobTypeListEnabled");
            }
        }


        /*
         * 
         * Relay commands
         * 
         */
        public RelayCommand AddButtonCommand { get; private set; }
        public RelayCommand EditButtonCommand { get; private set; }
        public RelayCommand DeleteButtonCommand { get; private set; }
        public RelayCommand UpButtonCommand { get; private set; }
        public RelayCommand DownButtonCommand { get; private set; }
        public RelayCommand OkButtonCommand { get; private set; }
        public RelayCommand CancelButtonCommand { get; private set; }
        public RelayCommand SaveButtonCommand { get; private set; }
    }
}