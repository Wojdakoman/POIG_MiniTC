using MiniTC.Model;
using MiniTC.ViewModel.Base;
using System.Windows;
using System.Windows.Input;

namespace MiniTC.ViewModel
{
    using R = Properties.Resources;
    class PanelController : ViewModelBase
    {
        //model
        DirectoryManager dm = new DirectoryManager();

        //private
        /// <summary>
        /// Is used to avoid changing currentPath to master on command changeDrive (when user enter path manually or path is reseted)
        /// </summary>
        private bool _blockDriveChange = false;

        //public
        public string CurrentPath { get => dm.CurrentPath; set {
                string tempPath = value;
                int result = dm.UpdatePath(tempPath);
                if(result > 0)
                {
                    if(result == 1) MessageBox.Show(R.DirectoryNoExists, R.Error, MessageBoxButton.OK, MessageBoxImage.Warning);
                    if(result == 2) MessageBox.Show(R.PermissionsError, R.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    _blockDriveChange = true;
                    SelectedDrive = dm.GetNewDriveIndex();
                    
                    dm.SetPath(tempPath, true);
                    OnPropertyChanged(nameof(ItemsList), nameof(SelectedDrive), nameof(CurrentPath));
                    _blockDriveChange = false;
                }
            } }
        public string[] Drives { get => dm.AvailableDrives; }
        public int SelectedDrive { get; set; } = 0;
        public int ListIdxSelected { get; set; } = -1;
        public string[] ItemsList { get {
                if(!dm.HasConnection()) {
                    _blockDriveChange = true;
                    MessageBox.Show(R.DirectoryNoExists, R.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    SelectedDrive = 0;
                    OnPropertyChanged(nameof(ItemsList), nameof(CurrentPath), nameof(SelectedDrive));
                    _blockDriveChange = false;
                }
                return dm.GetDirectoryElements();
            } }

        #region commands
        //command used when selecting drive from combobox
        private ICommand _changeDrive = null;
        public ICommand ChangeDrive
        {
            get
            {
                if (_changeDrive == null)
                {
                    _changeDrive = new RelayCommand(
                        arg =>
                        {
                            if(dm.HasConnection())
                                dm.SetPath(Drives[SelectedDrive], true);
                            OnPropertyChanged(nameof(CurrentPath), nameof(SelectedDrive), nameof(ItemsList));
                        },
                        arg => !_blockDriveChange
                    );
                }
                return _changeDrive;
            }
        }

        //command called on dropping combobox
        private ICommand _updateDrives = null;
        public ICommand UpdateDrives
        {
            get
            {
                if (_updateDrives == null)
                {
                    _updateDrives = new RelayCommand(
                        arg =>
                        {
                            OnPropertyChanged(nameof(Drives));
                        },
                        arg => true
                    );
                }
                return _updateDrives;
            }
        }

        //command used when duble click to move to directory
        private ICommand _changeDirectory = null;
        public ICommand ChangeDirectory
        {
            get
            {
                if (_changeDirectory == null)
                {
                    _changeDirectory = new RelayCommand(
                        arg =>
                        {
                            string newDirectory = dm.ClearName(ItemsList[ListIdxSelected]);
                            dm.SetPath(newDirectory);
                            ListIdxSelected = -1;
                            if (!dm.HasAccess())
                                MessageBox.Show(R.PermissionsError, R.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                            OnPropertyChanged(nameof(CurrentPath), nameof(ItemsList), nameof(ListIdxSelected));
                        },
                        arg => ListIdxSelected != -1 && (ItemsList[ListIdxSelected].Contains("<D>") || ItemsList[ListIdxSelected] == "..")
                    );
                }
                return _changeDirectory;
            }
        }

        //command used when click "Go" button next to path
        private ICommand _updateDirectory = null;
        public ICommand UpdateDirectory
        {
            get
            {
                if (_updateDirectory == null)
                {
                    _updateDirectory = new RelayCommand(
                        arg =>
                        {
                            SelectedDrive = dm.GetNewDriveIndex();
                            OnPropertyChanged(nameof(ItemsList), nameof(ListIdxSelected), nameof(SelectedDrive));
                        },
                        //arg => !string.IsNullOrEmpty(CurrentPath) /*&& dm.UpdatePath(CurrentPath) == 0*/
                        arg => true
                    );
                }
                return _updateDirectory;
            }
        }

        //command used on right-click on list to deselect element
        private ICommand _lostFocus = null;
        public ICommand LostFocus
        {
            get
            {
                if (_lostFocus == null)
                {
                    _lostFocus = new RelayCommand(
                        arg =>
                        {
                            ListIdxSelected = -1;
                            OnPropertyChanged(nameof(ListIdxSelected), nameof(ItemsList));
                        },
                        arg => true
                    );
                }
                return _lostFocus;
            }
        }
        #endregion

        /// <summary>
        /// Updates view after coping
        /// </summary>
        public void UpdateView()
        {
            OnPropertyChanged(nameof(ItemsList));
        }
    }
}
