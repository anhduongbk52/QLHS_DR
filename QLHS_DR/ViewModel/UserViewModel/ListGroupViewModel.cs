using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.UserViewModel
{
    class ListGroupViewModel : BaseViewModel
    {
        ServiceFactory serviceFactory;
        private ObservableCollection<User> _Users;
        public ObservableCollection<User> Users { get => _Users; set { _Users = value; OnPropertyChanged("Users"); } }
        private Role _RoleSelected;
        public Role RoleSelected
        {
            get => _RoleSelected;
            set
            {
                if (_RoleSelected != value  )
                {
                    _RoleSelected = value;
                    if (serviceFactory != null && _RoleSelected != null)
                    {
                        UsersInRole = serviceFactory.LoadUserInRole(_RoleSelected.Id);

                        OnPropertyChanged("UsersInRole");
                    }
                    OnPropertyChanged("RoleSelected");
                }
            }
        }

        private ObservableCollection<User> _UsersInRole;
        public ObservableCollection<User> UsersInRole
        {
            get => _UsersInRole;
            set
            {
                if (_UsersInRole != value)
                {
                    _UsersInRole = value;
                    OnPropertyChanged("UsersInRole");
                }
            }
        }
        private ObservableCollection<Role> _Roles;
        public ObservableCollection<Role> Roles { get => _Roles; set { _Roles = value; OnPropertyChanged("Roles"); } }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand RemoveRoleCommand { get; set; }
        public ICommand AddNewUserToRoleCommand { get; set; }
        public ICommand RemoveUserInRoleCommand { get; set; }
        public ICommand RenameRoleCommand { get; set; }
        public ICommand ChangeDescriptionRoleCommand { get; set; }
        public ListGroupViewModel()
        {

            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                serviceFactory = new ServiceFactory();
                Roles = serviceFactory.LoadRoles();
                Users = serviceFactory.LoadUsers();
            });
            AddNewUserToRoleCommand = new RelayCommand<User>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                if (_RoleSelected != null)
                {
                    if (_UsersInRole.Any(x => x.Id == p.Id))
                        MessageBox.Show("User: " + p.FullName + " đã có ở trong nhóm");
                    else
                    {
                        serviceFactory.NewUserRole(_RoleSelected.Id, p.Id);
                        UsersInRole = serviceFactory.LoadUserInRole(_RoleSelected.Id);
                    }
                }
            });
            RemoveUserInRoleCommand = new RelayCommand<User>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                if (_RoleSelected != null)
                {
                    if (_UsersInRole.Contains(p))
                    {
                        serviceFactory.RemoveUserRole(_RoleSelected.Id, p.Id);
                        UsersInRole = serviceFactory.LoadUserInRole(_RoleSelected.Id);
                    }
                }
            });
            RenameRoleCommand = new RelayCommand<Object>((p) => { if (_RoleSelected != null) return true; else return false; }, (p) =>
            {
                InputDialogWindow inputDialogWindow = new InputDialogWindow();
                inputDialogWindow.Input = _RoleSelected.Name;
                inputDialogWindow.ShowDialog();
                string newName = inputDialogWindow.Input;
                if (!string.IsNullOrEmpty(newName))
                {
                    serviceFactory.RenameRole(_RoleSelected.Id, newName);
                    Roles = serviceFactory.LoadRoles();
                }
            });
            RemoveRoleCommand = new RelayCommand<Object>((p) => { if (_RoleSelected != null) return true; else return false; }, (p) =>
            {
                if (MessageBox.Show("Bạn có muốn xóa Group: " + _RoleSelected.Name, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    serviceFactory.DeleteRole(_RoleSelected.Id);
                    Roles = serviceFactory.LoadRoles();
                }
            });
            ChangeDescriptionRoleCommand = new RelayCommand<Object>((p) => { if (_RoleSelected != null) return true; else return false; }, (p) =>
            {
                InputDialogWindow inputDialogWindow = new InputDialogWindow();
                inputDialogWindow.Input = _RoleSelected.Name;
                inputDialogWindow.ShowDialog();
                string newName = inputDialogWindow.Input;
                if (!string.IsNullOrEmpty(newName))
                {
                    serviceFactory.ChangeDescriptionRole(_RoleSelected.Id, newName);
                    Roles = serviceFactory.LoadRoles();
                }
            });
        }

    }
}
