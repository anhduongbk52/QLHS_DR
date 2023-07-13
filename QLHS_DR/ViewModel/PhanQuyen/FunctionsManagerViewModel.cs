using QLHS_DR.ChatAppServiceReference;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.PhanQuyen
{
    internal class FunctionsManagerViewModel : BaseViewModel
    {
        private Core.ServiceFactory _serviceFactory;
        private string _NewPermissionCode;
        public string NewPermissionCode
        {
            get => _NewPermissionCode;
            set
            {
                if (_NewPermissionCode != value)
                {
                    _NewPermissionCode = value;
                    OnPropertyChanged("NewPermissionCode");
                }
            }
        }
        private string _NewPermissionDescription;
        public string NewPermissionDescription
        {
            get => _NewPermissionDescription;
            set
            {
                if (_NewPermissionDescription != value)
                {
                    _NewPermissionDescription = value;
                    OnPropertyChanged("NewPermissionDescription");
                }
            }
        }
        private string _NewModulCode;
        public string NewModulCode
        {
            get => _NewModulCode;
            set
            {
                if (_NewModulCode != value)
                {
                    _NewModulCode = value;
                    OnPropertyChanged("NewModulCode");
                }
            }
        }
        private string _NewModulDescription;
        public string NewModulDescription
        {
            get => _NewModulDescription;
            set
            {
                if (_NewModulDescription != value)
                {
                    _NewModulDescription = value;
                    OnPropertyChanged("NewModulDescription");
                }
            }
        }
        private Permission _PermissionSelected;
        public Permission PermissionSelected
        {
            get => _PermissionSelected;
            set
            {
                if (_PermissionSelected != value)
                {
                    _PermissionSelected = value;
                    OnPropertyChanged("PermissionSelected");
                }
            }
        }

        private ObservableCollection<Modul> _Moduls;
        public ObservableCollection<Modul> Moduls
        {
            get => _Moduls;
            set
            {
                if (_Moduls != value)
                {
                    _Moduls = value;
                    OnPropertyChanged("Moduls");
                }
            }
        }
        private ObservableCollection<Permission> _PermissionsOfModulSelected;
        public ObservableCollection<Permission> PermissionsOfModulSelected
        {
            get => _PermissionsOfModulSelected;
            set
            {
                if (_PermissionsOfModulSelected != value)
                {
                    _PermissionsOfModulSelected = value;
                    OnPropertyChanged("PermissionsOfModulSelected");
                }
            }
        }
        private Modul _ModulSelected;
        public Modul ModulSelected
        {
            get => _ModulSelected;
            set
            {
                if (_ModulSelected != value)
                {
                    _ModulSelected = value;
                    if (_ModulSelected != null && _serviceFactory != null)
                    {
                        PermissionsOfModulSelected = _serviceFactory.GetPermissionsOfModul(_ModulSelected.Id);
                    }
                    OnPropertyChanged("ModulSelected");
                }
            }
        }

        public ICommand LoadedWindowCommand { get; set; }
        public ICommand NewPermissionCommand { get; set; }
        public ICommand NewModulCommand { get; set; }
        public ICommand CloseDialogCommand { get; private set; }
        public ICommand RemovePermissionCommand { get; private set; }
        public ICommand RemoveModulCommand { get; private set; }
        public FunctionsManagerViewModel()
        {
            _serviceFactory = new Core.ServiceFactory();
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                Moduls = _serviceFactory.LoadModuls();
            });
            RemoveModulCommand = new RelayCommand<Modul>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                _serviceFactory.DeleteModul(p.Id);
                Moduls = _serviceFactory.LoadModuls();
            });
            RemovePermissionCommand = new RelayCommand<Permission>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Bạn có muốn xóa chức năng: " + p.Description, "Cảnh báo !", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK && _serviceFactory != null && _ModulSelected != null)
                {
                    _serviceFactory.DeletePermission(p.Id);
                    PermissionsOfModulSelected = _serviceFactory.GetPermissionsOfModul(_ModulSelected.Id);
                }
            });
            NewModulCommand = new RelayCommand<bool>((p) => { if (p && _NewModulCode != null && _NewModulDescription != null) return true; else return false; }, (p) =>
            {
                try
                {
                    if (p && _NewModulCode != null && _NewModulDescription != null && _serviceFactory != null)
                    {
                        _serviceFactory.NewModul(_NewModulCode, _NewModulDescription);
                        Moduls = _serviceFactory.LoadModuls();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            NewPermissionCommand = new RelayCommand<bool>((p) => { if (p && _NewPermissionCode != null && _NewPermissionDescription != null) return true; else return false; }, (p) =>
            {
                try
                {
                    if (p && _NewPermissionCode != null && _NewPermissionDescription != null && _ModulSelected != null)
                    {
                        _serviceFactory.NewPermission(_NewPermissionCode, _NewPermissionDescription, _ModulSelected.Id);
                        PermissionsOfModulSelected = _serviceFactory.GetPermissionsOfModul(_ModulSelected.Id);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
