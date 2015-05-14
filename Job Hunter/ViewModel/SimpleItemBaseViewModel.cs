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
    public abstract class SimpleItemBaseViewModel : ViewModelBase
    {
        protected readonly IDataService _dataService;
        private ObservableCollection<SimpleItem> _itemCollection;
        private SimpleItem _selectedItem;
        private Visibility _notesVisibility;
        private Visibility _addItemVisibility;
        private bool _itemListEnabled;
        private string _selectedNote;
        private string _item;
        private string _note;
        private bool editFlag;  // True -> edit, False -> add
        private int nextPosition;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SimpleItemBaseViewModel(IDataService dataService)
        {
            // Get reference to dataService
            _dataService = dataService;

            // Initialize status collection
            _itemCollection = new ObservableCollection<SimpleItem>();

            // Default visibility
            _notesVisibility = Visibility.Visible;
            _addItemVisibility = Visibility.Collapsed;

            // Default list state
            _itemListEnabled = true;

            // Commands
            AddButtonCommand = new RelayCommand(AddPrepareCommand);
            EditButtonCommand = new RelayCommand(EditPrepareCommand);
            DeleteButtonCommand = new RelayCommand(DeleteCommandPre);
            UpButtonCommand = new RelayCommand(UpCommandPre);
            DownButtonCommand = new RelayCommand(DownCommandPre);
            OkButtonCommand = new RelayCommand(OkCommandPre);
            CancelButtonCommand = new RelayCommand(CancelCommand);
            SaveButtonCommand = new RelayCommand(SaveCommand);
        }

        /*
         *
         * Collection fill
         *
         */
        public void FillItemCollection(SimpleItem[] itemArray)
        {
            ItemCollection = new ObservableCollection<SimpleItem>(itemArray);

            // Default status
            if (nextPosition < 0)
            {
                nextPosition = 0;
            }
            if (ItemCollection.Count > nextPosition)
            {
                SelectedItem = ItemCollection[nextPosition];
            }
            else if (ItemCollection.Count > 0)
            {
                SelectedItem = ItemCollection[0];
            }
            else
            {
                SelectedItem = null;
                SelectedNote = "";
            }

        }

        /*
         *
         * Collection
         *
         */

        public ObservableCollection<SimpleItem> ItemCollection
        {
            get { return _itemCollection; }
            private set
            {
                if (_itemCollection == value) { return; }
                _itemCollection = value;
                RaisePropertyChanged("ItemCollection");
            }
        }

        /*
         *
         * Selected status properties
         *
         */

        public SimpleItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value) { return; }
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
                if (_selectedItem != null)
                {
                    SelectedNote = _selectedItem.Note;
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

        public string Item
        {
            get { return _item; }
            set
            {
                if (_item == value) { return; }
                _item = value;
                RaisePropertyChanged("Item");
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

        public void AddPrepareCommand()
        {
            // Set edit flag
            editFlag = false;

            // Adjust visibility
            NotesVisibility = Visibility.Collapsed;
            AddItemVisibility = Visibility.Visible;

            // Disable status list
            ItemListEnabled = false;

            // Initialize data entry fields.
            Item = Note = "";
        }

        public async void EditPrepareCommand()
        {
            if (SelectedItem != null)
            {
                // Set edit flag
                editFlag = true;

                // Adjust visibility
                NotesVisibility = Visibility.Collapsed;
                AddItemVisibility = Visibility.Visible;

                // Disable status list
                ItemListEnabled = false;

                // Initialize data entry fields.
                Item = SelectedItem.Title;
                Note = SelectedItem.Note;
            }
            else
            {
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };
                await dialogService.ShowMessageDialog("No item selected", "ERROR", buttonText);
            }
        }

        public async void DeleteCommandPre()
        {
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            if (SelectedItem != null)
            {
                string[] buttonText = new string[] { "Ok", "Cancel" };

                int dialogResult = await dialogService.ShowMessageDialog("Delete \""
                    + _selectedItem.Title + "\"?", "DELETE", buttonText);

                if (dialogResult == 0)
                {
                    // Store position of previous entry
                    nextPosition = ItemCollection.IndexOf(SelectedItem) - 1;
                    if (nextPosition < 0) { nextPosition = 0; }

                    // Delete entry
                    DeleteCommand(SelectedItem.Id);
                }
            }
            else
            {
                string[] buttonText = new string[] { "Ok" };
                await dialogService.ShowMessageDialog("No item selected", "ERROR", buttonText);
            }
        }

        public async void OkCommandPre()
        {
            // Check that a status text has been entered. If not notify error.
            if (Item != "")
            {
                // Update or add entry
                if (editFlag)
                {
                    // Store position of current entry
                    nextPosition = ItemCollection.IndexOf(SelectedItem);

                    // Update status
                    UpdateCommand(SelectedItem.Id, SelectedItem.Position, Item, Note);
                }
                else
                {
                    // Move to end of list. After insertion list count will have increased by one.
                    nextPosition = ItemCollection.Count;

                    // Add new status
                    AddCommand(Item, Note);
                }

                // Enable status list
                ItemListEnabled = true;

                // Adjust visibility
                NotesVisibility = Visibility.Visible;
                AddItemVisibility = Visibility.Collapsed;
            }
            else
            {
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };
                await dialogService.ShowMessageDialog("Empty title", "ERROR", buttonText);
            }
        }

        public void CancelCommand()
        {
            NotesVisibility = Visibility.Visible;
            AddItemVisibility = Visibility.Collapsed;
            ItemListEnabled = true;
            Item = Note = "";
        }

        public async void SaveCommand()
        {
            IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            if (SelectedItem != null)
            {
                {
                    string itemTitle = SelectedItem.Title;

                    // Store position of current entry
                    nextPosition = ItemCollection.IndexOf(SelectedItem);

                    //Update status data
                    UpdateCommand(SelectedItem.Id, SelectedItem.Position, SelectedItem.Title, SelectedNote);

                    //Notify note saved
                    string[] buttonText = new string[] { "Ok" };
                    await dialogService.ShowMessageDialog("\"" + itemTitle + "\" note saved", "SAVE", buttonText);
                }
            }
            else
            {
                string[] buttonText = new string[] { "Ok" };
                await dialogService.ShowMessageDialog("No item selected", "ERROR", buttonText);
            }
        }

        public void UpCommandPre()
        {
            // If selected item is null do nothing
            if (SelectedItem == null) { return; }

            // Store position of current entry's new position
            nextPosition = ItemCollection.IndexOf(SelectedItem);
            nextPosition = nextPosition > 0 ? nextPosition - 1 : 0;

            // Move item up in list
            UpCommand(SelectedItem.Position);
        }

        public void DownCommandPre()
        {
            // If selected item is null nothing to do
            if (SelectedItem == null) { return; }

            // Store position of current entry's new position
            nextPosition = ItemCollection.IndexOf(SelectedItem);
            nextPosition = nextPosition < ItemCollection.Count - 1 ? nextPosition + 1 : ItemCollection.Count - 1;

            // Move item down in list
            DownCommand(SelectedItem.Position);
        }

        /*
         * 
         * Command processors
         * 
         */
        public abstract int AddCommand(string title, string note);
        public abstract int UpdateCommand(long id, long position, string title, string note);
        public abstract int DeleteCommand(long itemId);
        public abstract void UpCommand(long position);
        public abstract void DownCommand(long position);

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

        public Visibility AddItemVisibility
        {
            get { return _addItemVisibility; }
            private set
            {
                if (_addItemVisibility == value) { return; }
                _addItemVisibility = value;
                RaisePropertyChanged("AddItemVisibility");
            }
        }

        public bool ItemListEnabled
        {
            get { return _itemListEnabled; }
            private set
            {
                if (_itemListEnabled == value) { return; }
                _itemListEnabled = value;
                RaisePropertyChanged("ItemListEnabled");
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