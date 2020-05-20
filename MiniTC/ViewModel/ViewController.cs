using MiniTC.Model;
using MiniTC.ViewModel.Base;
using System;
using System.Windows;
using System.Windows.Input;

namespace MiniTC.ViewModel
{
    using R = Properties.Resources;
    class ViewController : ViewModelBase
    {
        //model
        CopyManager cm = new CopyManager();
        CopyFile cf = new CopyFile();
        CopyDirectory cd = new CopyDirectory();

        //public
        public PanelController LeftPanel { get; }
        public PanelController RightPanel { get; }

        public ViewController()
        {
            LeftPanel = new PanelController();
            RightPanel = new PanelController();
        }

        #region commands
        private ICommand _copy = null;
        public ICommand Copy
        {
            get
            {
                if (_copy == null)
                {
                    _copy = new RelayCommand(
                        arg =>
                        {
                            //get pathes
                            string file;
                            string destination;
                            if (LeftPanel.ListIdxSelected == -1)
                            {
                                destination = LeftPanel.CurrentPath;
                                file = $"{RightPanel.CurrentPath}\\{RightPanel.ItemsList[RightPanel.ListIdxSelected]}";
                            }
                            else
                            {
                                destination = RightPanel.CurrentPath;
                                file = $"{LeftPanel.CurrentPath}\\{LeftPanel.ItemsList[LeftPanel.ListIdxSelected]}";
                            }
                            file = cm.ClearName(file);
                            //check access
                            if (!cm.HasAccess(file))
                            {
                                MessageBox.Show(R.PermissionsError, R.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                //check element type
                                CopyManager.ElementType fileType = cm.GetElementType(file);

                                if (fileType == CopyManager.ElementType.Directory)
                                {
                                    //check if exists
                                    if (cd.DirectoryExists(file, destination))
                                    {
                                        var result = MessageBox.Show(R.DirectoryExists, R.Attention, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                                        if (result == MessageBoxResult.Yes)
                                        {
                                            //override directory
                                            cd.Copy(file, destination, true);
                                        }
                                        else if (result == MessageBoxResult.No)
                                        {
                                            //find new name for directory and copy
                                            cd.CopyWithName(file, destination);
                                        }
                                    }
                                    else
                                    {
                                        cd.Copy(file, destination, false);
                                    }
                                }
                                else if (fileType == CopyManager.ElementType.File)
                                {
                                    //check if exists
                                    if (cf.FileExists(file, destination))
                                    {
                                        var result = MessageBox.Show(R.FileExists, R.Attention, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                                        if (result == MessageBoxResult.Yes)
                                        {
                                            //override file
                                            cf.Copy(file, destination, true);
                                        }
                                        else if (result == MessageBoxResult.No)
                                        {
                                            //find new name for file and copy
                                            cf.CopyWithName(file, destination);
                                        }
                                    }
                                    else
                                    {
                                        cf.Copy(file, destination, false);
                                    }
                                }
                            }
                            
                            
                            LeftPanel.UpdateView();
                            RightPanel.UpdateView();
                        },
                        arg => cm.GetCommandCondition(LeftPanel.ListIdxSelected, RightPanel.ListIdxSelected, LeftPanel.CurrentPath, RightPanel.CurrentPath, LeftPanel.ItemsList, RightPanel.ItemsList)
                    );
                }
                return _copy;
            }
        }

        private ICommand _help = null;
        public ICommand Help
        {
            get
            {
                if (_help == null)
                {
                    _help = new RelayCommand(
                        arg =>
                        {
                            MessageBox.Show(R.HelpText.Replace("\\n", Environment.NewLine), R.Help, MessageBoxButton.OK, MessageBoxImage.Information);
                        },
                        arg => true
                    );
                }
                return _help;
            }
        }
        #endregion
    }
}
