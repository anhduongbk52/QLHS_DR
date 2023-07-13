using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
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
    class PhanQuyenViewModel : BaseViewModel
    {
        #region "Field and Properties"
        private ServiceFactory _ServiceFactory;
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set
            {
                if (_IsBusy != value)
                {
                    _IsBusy = value;
                    NotifyPropertyChanged("IsBusy");
                }
            }
        }
        private ObservableCollection<RowPermission> _RowPermissions;
        public ObservableCollection<RowPermission> RowPermissions
        {
            get => _RowPermissions;
            set
            {
                if (_RowPermissions != value)
                {
                    _RowPermissions = value;
                    NotifyPropertyChanged("RowPermissions");
                }
            }
        }

        private Role _RoleSelected;
        public Role RoleSelected
        {
            get => _RoleSelected;
            set
            {
                if (_RoleSelected != value)
                {
                    _RoleSelected = value;
                    NotifyPropertyChanged("RoleSelected");
                }
            }
        }

        private ObservableCollection<Role> _Roles;
        public ObservableCollection<Role> Roles
        {
            get => _Roles;
            set
            {
                if (_Roles != value)
                {
                    _Roles = value;
                    NotifyPropertyChanged("Roles");
                }
            }
        }
        private ObservableCollection<RolePermissionD> _RolePermissionDs;
        public ObservableCollection<RolePermissionD> RolePermissionDs
        {
            get => _RolePermissionDs;
            set
            {
                if (_RolePermissionDs != value)
                {
                    _RolePermissionDs = value;
                    NotifyPropertyChanged("RolePermissionDs");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand SaveCommand { get; set; }
        public ICommand AddPermisionCommand { get; set; }
        public ICommand AddModulCommand { get; set; }
        public ICommand RoleSelectionCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        #endregion

        public PhanQuyenViewModel()
        {
            _ServiceFactory = new ServiceFactory();
            Roles = _ServiceFactory.LoadRoles();

            RowPermissions = new ObservableCollection<RowPermission>();
            SaveCommand = new RelayCommand<Object>((p) => { if (_RoleSelected != null && _RowPermissions != null) return true; else return false; }, (p) =>
            {
                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient();
                try
                {
                    IsBusy = true;
                    int length = RowPermissions.Count();
                    int[] permissionIds = new int[length];
                    bool[] isChecked = new bool[length];
                    for (int i = 0; i < length; i++)
                    {
                        permissionIds[i] = RowPermissions[i].Permission.Id;
                        isChecked[i] = RowPermissions[i].IsChecked;
                    }
                    _MyClient.Open();
                    _MyClient.UpdateRolePermissionD(_RoleSelected.Id, permissionIds, isChecked);
                    _MyClient.Close();
                    MessageBox.Show("Lưu thành công");
                    RowPermissions = LoadAllRowPermission();
                }
                catch (Exception ex)
                {
                    _MyClient.Abort();
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            });

            RoleSelectionCommand = new RelayCommand<Role>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                RowPermissions = LoadAllRowPermission();
            });
            ExitCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                p.Close();
            });
        }
        private ObservableCollection<RowPermission> LoadAllRowPermission()
        {
            ObservableCollection<RowPermission> ketqua = new ObservableCollection<RowPermission>();
            var moduls = _ServiceFactory.LoadModuls();
            var permissions = _ServiceFactory.LoadPermissions().Where(x=>x.ModulId!= 1011 & x.ModulId!= 11);
            var permissionRoles = _ServiceFactory.LoadRolePermissionDs();
            foreach (var pm in permissions)
            {
                pm.Modul = moduls.Where(x => x.Id == pm.ModulId).FirstOrDefault();
                RowPermission rowPermission = new RowPermission()
                {
                    IsChecked = permissionRoles.Any(x => x.PermissionId == pm.Id && x.RoleId == _RoleSelected.Id),
                    Permission = pm
                };
                ketqua.Add(rowPermission);
            }
            return ketqua;
        }
        
        public class RowPermission : BaseViewModel
        {
            private bool _IsChecked;
            public bool IsChecked
            {
                get => _IsChecked;
                set
                {
                    if (_IsChecked != value)
                    {
                        _IsChecked = value;
                        NotifyPropertyChanged("IsChecked");
                    }
                }
            }
            private Permission _Permission;
            public Permission Permission
            {
                get => _Permission;
                set
                {
                    if (_Permission != value)
                    {
                        _Permission = value;
                        NotifyPropertyChanged("Permission");
                    }
                }
            }
        }
    }
}
