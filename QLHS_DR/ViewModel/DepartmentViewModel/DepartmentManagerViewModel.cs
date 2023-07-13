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

namespace QLHS_DR.ViewModel.DepartmentViewModel
{
    internal class DepartmentManagerViewModel : BaseViewModel
    {
        ServiceFactory serviceFactory;
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set
            {
                if (_IsBusy != value)
                {
                    _IsBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }
        private User _AddUserToDepartment;
        public User AddUserToDepartment
        {
            get => _AddUserToDepartment;
            set
            {
                if (_AddUserToDepartment != value)
                {
                    _AddUserToDepartment = value;
                    OnPropertyChanged("AddUserToDepartment");
                }
            }
        }
        private UserDepartment _UserDepartmentSelected;
        public UserDepartment UserDepartmentSelected
        {
            get => _UserDepartmentSelected;
            set
            {
                if (_UserDepartmentSelected != value)
                {
                    _UserDepartmentSelected = value;
                    if(_UserDepartmentSelected!=null)
                    {
                        PositionSelected = _UserDepartmentSelected.Position;
                    }          
                    OnPropertyChanged("UserDepartmentSelected");
                }
            }
        }
        private Position _PositionSelected;
        public Position PositionSelected
        {
            get => _PositionSelected;
            set
            {
                if (_PositionSelected != value)
                {
                    _PositionSelected = value;
                    OnPropertyChanged("PositionSelected");
                }
            }
        }
        private ObservableCollection<User> _Users;
        public ObservableCollection<User> Users
        {
            get => _Users;
            set
            {
                if (_Users != value)
                {
                    _Users = value;
                    OnPropertyChanged("Users");
                }
            }
        }
        private ObservableCollection<UserDepartment> _UserDepartmentsOfDepartmentSelected;
        public ObservableCollection<UserDepartment> UserDepartmentsOfDepartmentSelected
        {
            get => _UserDepartmentsOfDepartmentSelected;
            set
            {
                if (_UserDepartmentsOfDepartmentSelected != value)
                {
                    _UserDepartmentsOfDepartmentSelected = value;
                    foreach (UserDepartment userDepartment in _UserDepartmentsOfDepartmentSelected)
                    {
                        userDepartment.User = _Users.Where(x => x.Id == userDepartment.UserId).FirstOrDefault();
                    }
                    OnPropertyChanged("UserDepartmentsOfDepartmentSelected");
                }
            }
        }

        private ObservableCollection<UserDepartment> _UserDepartments;
        private ObservableCollection<Position> _Positions;
        public ObservableCollection<Position> Positions
        {
            get => _Positions;
            set
            {
                if (_Positions != value)
                {
                    _Positions = value;
                    OnPropertyChanged("Positions");
                }
            }
        }

        private ObservableCollection<Department> _Departments;
        public ObservableCollection<Department> Departments
        {
            get => _Departments;
            set
            {
                if (_Departments != value)
                {
                    _Departments = value;
                    OnPropertyChanged("Departments");
                }
            }
        }
        private ObservableCollection<GroupDepartment> _GroupDepartments;
        public ObservableCollection<GroupDepartment> GroupDepartments
        {
            get => _GroupDepartments;
            set
            {
                if (_GroupDepartments != value)
                {
                    _GroupDepartments = value;
                    OnPropertyChanged("GroupDepartments");
                }
            }
        }
        private Department _DepartmentSelected;
        public Department DepartmentSelected
        {
            get => _DepartmentSelected;
            set
            {
                if (_DepartmentSelected != value)
                {
                    _DepartmentSelected = value;
                    UserDepartmentsOfDepartmentSelected = serviceFactory.LoadUserDepartmentsByDepartmentId(_DepartmentSelected.Id);
                    OnPropertyChanged("DepartmentSelected");
                }
            }
        }

        private GroupDepartment _SelectedNewGroupDepartment;
        public GroupDepartment SelectedNewGroupDepartment
        {
            get => _SelectedNewGroupDepartment;
            set
            {
                if (_SelectedNewGroupDepartment != value)
                {
                    _SelectedNewGroupDepartment = value;
                    OnPropertyChanged("SelectedNewGroupDepartment");
                }
            }
        }
        private Position _SelectedNewPosition1;
        public Position SelectedNewPosition1
        {
            get => _SelectedNewPosition1;
            set
            {
                if (_SelectedNewPosition1 != value)
                {
                    _SelectedNewPosition1 = value;
                    OnPropertyChanged("SelectedNewPosition1");
                }
            }
        }
        private Position _SelectedNewPosition2;
        public Position SelectedNewPosition2
        {
            get => _SelectedNewPosition2;
            set
            {
                if (_SelectedNewPosition2 != value)
                {
                    _SelectedNewPosition2 = value;
                    OnPropertyChanged("SelectedNewPosition2");
                }
            }
        }
        private string _NewDepartmentCode;
        public string NewDepartmentCode
        {
            get => _NewDepartmentCode;
            set
            {
                if (_NewDepartmentCode != value)
                {
                    _NewDepartmentCode = value;
                    OnPropertyChanged("NewDepartmentCode");
                }
            }
        }
        private string _NewDepartmentName;
        public string NewDepartmentName
        {
            get => _NewDepartmentName;
            set
            {
                if (_NewDepartmentName != value)
                {
                    _NewDepartmentName = value;
                    OnPropertyChanged("NewDepartmentName");
                }
            }
        }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ApplyCommand { get; set; }
        public ICommand CloseDialogCommand { get; private set; }
        public ICommand RemoveUserCommand { get; private set; }
        public ICommand LockDepartmentCommand { get; private set; }
        public ICommand UnLockDepartmentCommand { get; private set; }
        public ICommand NewDepartmentCommand { get; private set; }
        public ICommand DeleteDepartmentCommand { get; private set; }
        public DepartmentManagerViewModel()
        {
            serviceFactory = new ServiceFactory();
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                try
                {
                    IsBusy = true;
                    GroupDepartments = serviceFactory.LoadGroupDepartments();
                    Departments = serviceFactory.GetDepartments();
                    Positions = serviceFactory.LoadPositions();
                    Users = serviceFactory.LoadUsers();
                    _UserDepartments = serviceFactory.LoadUserDepartments();
                    //LoadUserAndDepartmentToUserDepartments();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { IsBusy = false; }
            });
            RemoveUserCommand = new RelayCommand<UserDepartment>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Bạn có muốn xóa tài khoản " + p.User.FullName + " khỏi " + _DepartmentSelected?.Name, "Cảnh báo !", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                {
                    serviceFactory.RemoveUserToDepartment(p.DepartmentId, p.UserId);
                    UserDepartmentsOfDepartmentSelected = serviceFactory.LoadUserDepartmentsByDepartmentId(_DepartmentSelected.Id);
                }
            });
            DeleteDepartmentCommand = new RelayCommand<Department>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Bạn có muốn xóa đơn vị " + p.Name, "Cảnh báo !", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                {
                    serviceFactory.DeleteDepartment(p.Id);
                    Departments = serviceFactory.GetDepartments();
                }
            });
            ApplyCommand = new RelayCommand<Object>((p) => { if (_UserDepartmentSelected != null) return true; else return false; }, (p) =>
            {
                serviceFactory.ChangePositionOfUserDepartment(_UserDepartmentSelected.Id, _PositionSelected.Id);
                if (_UserDepartmentSelected.PriorityProcessing != null)
                {
                    serviceFactory.ChangePriorityProcessingOfUserDepartment(_UserDepartmentSelected.Id, _UserDepartmentSelected.PriorityProcessing.Value);
                }
                UserDepartmentsOfDepartmentSelected = serviceFactory.LoadUserDepartmentsByDepartmentId(_DepartmentSelected.Id);
            });
            CloseDialogCommand = new RelayCommand<bool>((p) => { return true; }, (p) =>
            {
                if (p && _AddUserToDepartment != null)
                {
                    serviceFactory.AddUserToDepartment(_DepartmentSelected.Id, _AddUserToDepartment.Id);
                    UserDepartmentsOfDepartmentSelected = serviceFactory.LoadUserDepartmentsByDepartmentId(_DepartmentSelected.Id);
                }
            });
            LockDepartmentCommand = new RelayCommand<Department>((p) => { if (p != null && p.Locked == false) return true; else return false; }, (p) =>
            {
                serviceFactory.LockDepartment(p.Id, true);
                Departments = serviceFactory.GetDepartments();
            });
            UnLockDepartmentCommand = new RelayCommand<Department>((p) => { if (p != null && p.Locked == true) return true; else return false; }, (p) =>
            {
                serviceFactory.LockDepartment(p.Id, false);
                Departments = serviceFactory.GetDepartments();
            });
            NewDepartmentCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                if(_SelectedNewPosition1!=null &&_SelectedNewGroupDepartment != null && _SelectedNewPosition2!=null)
                {
                    serviceFactory.NewDepartment(_NewDepartmentCode, _NewDepartmentName, _SelectedNewGroupDepartment.Id, _SelectedNewPosition1.Id, _SelectedNewPosition2.Id);
                    Departments = serviceFactory.GetDepartments();
                }
            });

        }
        private void LoadUserDepartmentsOfDepartmentSelected()
        {
            try
            {
                if (_DepartmentSelected != null && _UserDepartments != null)
                {
                    _UserDepartmentsOfDepartmentSelected = serviceFactory.LoadUserDepartmentsByDepartmentId(_DepartmentSelected.Id);
                    foreach (UserDepartment userDepartment in _UserDepartmentsOfDepartmentSelected)
                    {
                        userDepartment.User = _Users.Where(x => x.Id == userDepartment.UserId).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadUserAndDepartmentToUserDepartments()
        {
            try
            {
                if (_UserDepartments != null)
                {
                    foreach (var userDepartment in _UserDepartments)
                    {
                        userDepartment.User = _Users.Where(x => x.Id == userDepartment.UserId).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
