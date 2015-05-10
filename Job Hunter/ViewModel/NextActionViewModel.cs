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
    public class NextActionViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private ObservableCollection<NextActionItem> _nextActionCollection;
        private NextActionItem _selectedNextAction;
        private Visibility _notesVisibility;
        private Visibility _addNextActionVisibility;
        private bool _nextActionListEnabled;
        private string _selectedNote;
        private string _nextAction;
        private string _note;
        private bool editFlag;      // True -> edit, False -> add

        /// <summary>
        /// Initializes a new instance of the NextActionViewModel class.
        /// </summary>
        public NextActionViewModel(IDataService dataService)
        {
            // Get reference to dataService
            _dataService = dataService;

            // Fill next action collection
            NextActionCollection = new ObservableCollection<NextActionItem>();
            _dataService.NextActionList(FillNextActionList);
            _dataService.NextActionModified += (sender, e) => _dataService.NextActionList(FillNextActionList);

            // Default visibility
            _notesVisibility = Visibility.Visible;
            _addNextActionVisibility = Visibility.Collapsed;

            // Default list state
            _nextActionListEnabled = true;

            // Commands
            UpButtonCommand = new RelayCommand(UpCommand);
            DownButtonCommand = new RelayCommand(DownCommand);
            AddButtonCommand = new RelayCommand(AddCommand);
            EditButtonCommand = new RelayCommand(EditCommand);
            DeleteButtonCommand = new RelayCommand(DeleteCommand);
            OkButtonCommand = new RelayCommand(OkCommand);
            CancelButtonCommand = new RelayCommand(CancelCommand);
            SaveButtonCommand = new RelayCommand(SaveCommand);
        }

        /*
         *
         * Collection fill callback
         *
         */
        public void FillNextActionList(NextActionItem[] nextActionArray)
        {
            NextActionCollection = new ObservableCollection<NextActionItem>(nextActionArray);

            // Default Next Action
            SelectedNextAction = NextActionCollection.Count > 0 ? NextActionCollection[0] : null;
        }

        /*
         *
         * Collection
         *
         */
        public ObservableCollection<NextActionItem> NextActionCollection
        {
            get { return _nextActionCollection; }
            private set
            {
                _nextActionCollection = value;
                RaisePropertyChanged("NextActionCollection");
            }
        }

        /*
         *
         * Selected next action properties
         *
         */
        public NextActionItem SelectedNextAction
        {
            get { return _selectedNextAction; }
            set
            {
                if (_selectedNextAction == value) { return; }
                _selectedNextAction = value;
                RaisePropertyChanged("SelectedNextAction");
                if (_selectedNextAction != null)
                {
                    SelectedNote = _selectedNextAction.Note;
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
         * Add/edit next action properties
         *
         */
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
        public void UpCommand()
        {
            // If current position is first nothing to do
            if (SelectedNextAction.Position == 0) { return; }

            // Search for the item in the previous position
            NextActionItem previousItem = null;
            foreach (NextActionItem item in NextActionCollection)
            {
                if (item.Position == SelectedNextAction.Position - 1)
                {
                    previousItem = item;
                    break;
                }
            }

            // Update the items
            _dataService.UpdateNextActionList(new NextActionItem[]
            {
                new NextActionItem(previousItem.Id, previousItem.Position + 1, previousItem.NextAction, previousItem.Note),
                new NextActionItem(SelectedNextAction.Id, SelectedNextAction.Position - 1, SelectedNextAction.NextAction, SelectedNextAction.Note)
            });
        }

        public void DownCommand()
        {
            // If current position is first nothing to do
            if (SelectedNextAction.Position == NextActionCollection.Count - 1) { return; }

            // Search for the item in the previous position
            NextActionItem nextItem = null;
            foreach (NextActionItem item in NextActionCollection)
            {
                if (item.Position == SelectedNextAction.Position + 1)
                {
                    nextItem = item;
                    break;
                }
            }

            // Update the items
            _dataService.UpdateNextActionList(new NextActionItem[]
            {
                new NextActionItem(nextItem.Id, nextItem.Position - 1, nextItem.NextAction, nextItem.Note),
                new NextActionItem(SelectedNextAction.Id, SelectedNextAction.Position + 1, SelectedNextAction.NextAction, SelectedNextAction.Note)
            });
        }

        public void AddCommand()
        {
            // Set edit flag
            editFlag = false;

            // Adjust visibility
            NotesVisibility = Visibility.Collapsed;
            AddNextActionVisibility = Visibility.Visible;

            // Disable next action list
            NextActionListEnabled = false;

            // Initialize data entry fields
            NextAction = Note = "";
        }

        public void EditCommand()
        {
            // Set edit flag
            editFlag = true;

            // Adjust visibility
            NotesVisibility = Visibility.Collapsed;
            AddNextActionVisibility = Visibility.Visible;

            // Disable next action list
            NextActionListEnabled = false;

            // Initialize data entry fields
            NextAction = SelectedNextAction.NextAction;
            Note = SelectedNextAction.Note;
        }

        public async void DeleteCommand()
        {
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            string[] buttonText = new string[] { "Ok", "Cancel" };

            int dialogResult = await dialogService.ShowMessageDialog("Delete \"" + SelectedNextAction.NextAction + "\"?", "DELETE", buttonText);

            if (dialogResult == 0)
            {
                _dataService.DeleteNextAction(SelectedNextAction);
            }
        }

        public async void OkCommand()
        {
            // Check that a next action has been entered. If not notify error.
            if (_nextAction != "")
            {
                // Update or add entry
                if (editFlag)
                {
                    // Update next action
                    _dataService.UpdateNextAction(new NextActionItem(SelectedNextAction.Id, SelectedNextAction.Position, 
                        NextAction, Note));
                }
                else
                {
                    // Search next available position
                    long maxPosition = 0;
                    foreach (NextActionItem item in _nextActionCollection)
                    {
                        maxPosition = item.Position > maxPosition ? item.Position : maxPosition;
                    }
                    _dataService.AddNextAction(new NextActionItem(-1, ++maxPosition, _nextAction, _note));
                    NextAction = Note = "";
                }

                // Enable next action list
                NextActionListEnabled = true;

                // Adjust visibility
                NotesVisibility = Visibility.Visible;
                AddNextActionVisibility = Visibility.Collapsed;
            }
            else
            {
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };
                await dialogService.ShowMessageDialog("Empty next action", "ERROR", buttonText);
            }
        }

        public void CancelCommand()
        {
            NotesVisibility = Visibility.Visible;
            AddNextActionVisibility = Visibility.Collapsed;
            NextActionListEnabled = true;
            NextAction = Note = "";
        }

        public async void SaveCommand()
        {
            if (NextActionCollection.Count > 0)
            {
                _dataService.UpdateNextAction(new NextActionItem(SelectedNextAction.Id, SelectedNextAction.Position, 
                    SelectedNextAction.NextAction, SelectedNote));
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };
                await dialogService.ShowMessageDialog("\"" + _selectedNextAction.NextAction + "\" notes saved", "SAVE", buttonText);
            }
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

        public Visibility AddNextActionVisibility
        {
            get { return _addNextActionVisibility; }
            private set
            {
                if (_addNextActionVisibility == value) { return; }
                _addNextActionVisibility = value;
                RaisePropertyChanged("AddNextActionVisibility");
            }
        }

        public bool NextActionListEnabled
        {
            get { return _nextActionListEnabled; }
            private set
            {
                if (_nextActionListEnabled == value) { return; }
                _nextActionListEnabled = value;
                RaisePropertyChanged("NextActionListEnabled");
            }
        }

        // Commands
        public RelayCommand UpButtonCommand { get; private set; }
        public RelayCommand DownButtonCommand { get; private set; }
        public RelayCommand AddButtonCommand { get; private set; }
        public RelayCommand EditButtonCommand { get; private set; }
        public RelayCommand DeleteButtonCommand { get; private set; }
        public RelayCommand OkButtonCommand { get; private set; }
        public RelayCommand CancelButtonCommand { get; private set; }
        public RelayCommand SaveButtonCommand { get; private set; }
    }
}
