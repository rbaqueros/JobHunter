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
    public class StatusViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private ObservableCollection<StatusItem> _statusCollection;
        private StatusItem _selectedStatus;
        private Visibility _notesVisibility;
        private Visibility _addStatusVisibility;
        private bool _statusListEnabled;
        private string _selectedNote;
        private string _status;
        private string _note;
        private bool editFlag;  // True -> edit, False -> add

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public StatusViewModel(IDataService dataService)
        {
            // Get reference to dataService
            _dataService = dataService;

            // Fill status collection
            _statusCollection = new ObservableCollection<StatusItem>();
            _dataService.StatusList(FillStatusCollection);
            _dataService.StatusModified += (sender, e) => _dataService.StatusList(FillStatusCollection);

            // Default visibility
            _notesVisibility = Visibility.Visible;
            _addStatusVisibility = Visibility.Collapsed;

            // Default list state
            _statusListEnabled = true;

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
        public void FillStatusCollection(StatusItem[] statusArray)
        {
            StatusCollection = new ObservableCollection<StatusItem>(statusArray);

            // Default status
            SelectedStatus = StatusCollection.Count > 0 ? StatusCollection[0] : null;
        }

        /*
         *
         * Collection
         *
         */
        public ObservableCollection<StatusItem> StatusCollection
        {
            get { return _statusCollection; }
            private set
            {
                if (_statusCollection == value) { return; }
                _statusCollection = value;
                RaisePropertyChanged("StatusCollection");
            }
        }

        /*
         *
         * Selected status properties
         *
         */
        public StatusItem SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                if (_selectedStatus == value) { return; }
                _selectedStatus = value;
                RaisePropertyChanged("SelectedStatus");
                if (_selectedStatus != null)
                {
                    SelectedNote = _selectedStatus.Note;
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
         * Add/edit status properties
         *
         */
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
         * Command
         *
         */
        public void AddCommand()
        {
            // Set edit flag
            editFlag = false;

            // Adjust visibility
            NotesVisibility = Visibility.Collapsed;
            AddStatusVisibility = Visibility.Visible;

            // Disable status list
            StatusListEnabled = false;

            // Initialize data entry fields.
            Status = Note = "";
        }

        public void EditCommand()
        {
            // Set edit flag
            editFlag = true;

            // Adjust visibility
            NotesVisibility = Visibility.Collapsed;
            AddStatusVisibility = Visibility.Visible;

            // Disable status list
            StatusListEnabled = false;

            // Initialize data entry fields.
            Status = SelectedStatus.Status;
            Note = SelectedStatus.Note;
        }

        public async void DeleteCommand()
        {
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            string[] buttonText = new string[] { "Ok", "Cancel" };

            int dialogResult = await dialogService.ShowMessageDialog("Delete \""
                + _selectedStatus.Status + "\"?", "DELETE", buttonText);

            if (dialogResult == 0)
            {
                _dataService.DeleteStatus(SelectedStatus);
            }
        }

        public void OkCommand()
        {
            // Check that a status text has been entered. If not notify error.
            if (Status != "")
            {
                // Update or add entry
                if (editFlag)
                {
                    // Update status
                    _dataService.UpdateStatus(new StatusItem(SelectedStatus.Id, SelectedStatus.Position,
                        Status, Note));
                }
                else
                {
                    // Search next available position
                    long maxPosition = 0;
                    foreach (StatusItem item in _statusCollection)
                    {
                        maxPosition = item.Position > maxPosition ? item.Position : maxPosition;
                    }

                    // Add new status
                    _dataService.AddStatus(new StatusItem(-1, ++maxPosition, Status, Note));
                    Status = Note = "";
                }

                // Enable status list
                StatusListEnabled = true;

                // Adjust visibility
                NotesVisibility = Visibility.Visible;
                AddStatusVisibility = Visibility.Collapsed;
            }
            else
            {
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };
                dialogService.ShowMessageDialog("Empty status", "ERROR", buttonText);
            }
        }

        public void CancelCommand()
        {
            NotesVisibility = Visibility.Visible;
            AddStatusVisibility = Visibility.Collapsed;
            StatusListEnabled = true;
            Status = Note = "";
        }

        public async void SaveCommand()
        {
            if (StatusCollection.Count > 0)
            {
                string statusTitle = SelectedStatus.Status;

                //Update status data
                _dataService.UpdateStatus(new StatusItem(SelectedStatus.Id, SelectedStatus.Position,
                    SelectedStatus.Status, SelectedNote));

                //Notify note saved
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };
                int dialogResult = await dialogService.ShowMessageDialog("\"" + statusTitle + "\" note saved", "SAVE", buttonText);
            }
        }

        public void UpCommand()
        {
            // If current position is first nothing to do
            if (SelectedStatus.Position == 0) { return; }

            // Search for the item in the previous position
            StatusItem previousItem = null;
            foreach (StatusItem item in StatusCollection)
            {
                if (item.Position == SelectedStatus.Position - 1)
                {
                    previousItem = item;
                    break;
                }
            }

            // Update the items
            _dataService.UpdateStatusList(new StatusItem[]
            {
                new StatusItem(previousItem.Id, previousItem.Position + 1, previousItem.Status, previousItem.Note),
                new StatusItem(SelectedStatus.Id, SelectedStatus.Position - 1, SelectedStatus.Status, SelectedStatus.Note)
            });
        }

        public void DownCommand()
        {
            // If current position is first nothing to do
            if (SelectedStatus.Position == StatusCollection.Count - 1) { return; }

            // Search for the item in the previous position
            StatusItem nextItem = null;
            foreach (StatusItem item in StatusCollection)
            {
                if (item.Position == SelectedStatus.Position + 1)
                {
                    nextItem = item;
                    break;
                }
            }

            // Update the items
            _dataService.UpdateStatusList(new StatusItem[]
            {
                new StatusItem(nextItem.Id, nextItem.Position - 1, nextItem.Status, nextItem.Note),
                new StatusItem(SelectedStatus.Id, SelectedStatus.Position + 1, SelectedStatus.Status, SelectedStatus.Note)
            });
        }

        /*
         *
         * Note/add-edit form visibility
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

        public Visibility AddStatusVisibility
        {
            get { return _addStatusVisibility; }
            private set
            {
                if (_addStatusVisibility == value) { return; }
                _addStatusVisibility = value;
                RaisePropertyChanged("AddStatusVisibility");
            }
        }

        public bool StatusListEnabled
        {
            get { return _statusListEnabled; }
            private set
            {
                if (_statusListEnabled == value) { return; }
                _statusListEnabled = value;
                RaisePropertyChanged("StatusListEnabled");
            }
        }

        // Commands
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