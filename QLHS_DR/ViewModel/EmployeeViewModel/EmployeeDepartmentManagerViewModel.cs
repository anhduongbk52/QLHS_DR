using DevExpress.Mvvm.Native;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.UserView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.EmployeeViewModel
{

    internal class EmployeeDepartmentManagerViewModel : BaseViewModel
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
        private Employee _AddEmployeeToDepartment;
        public Employee AddEmployeeToDepartment
        {
            get => _AddEmployeeToDepartment;
            set
            {
                if (_AddEmployeeToDepartment != value)
                {
                    _AddEmployeeToDepartment = value;
                    OnPropertyChanged("AddEmployeeToDepartment");
                }
            }
        }
        private DateTime? _FromDate;
        public DateTime? FromDate
        {
            get => _FromDate;
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    OnPropertyChanged("FromDate");
                }
            }
        }
        private DateTime? _ToDate;
        public DateTime? ToDate
        {
            get => _ToDate;
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    OnPropertyChanged("ToDate");
                }
            }
        }
        private EmployeeDepartment _EmployeeDepartmentSelected;
        public EmployeeDepartment EmployeeDepartmentSelected
        {
            get => _EmployeeDepartmentSelected;
            set
            {
                if (_EmployeeDepartmentSelected != value)
                {
                    _EmployeeDepartmentSelected = value;
                    if (_EmployeeDepartmentSelected != null)
                    {
                        PositionSelected = _EmployeeDepartmentSelected.Position;
                        FromDate = _EmployeeDepartmentSelected.FromDate;
                        ToDate  = _EmployeeDepartmentSelected.ToDate;
                    }
                    OnPropertyChanged("EmployeeDepartmentSelected");
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
        private ObservableCollection<Employee> _Employees;
        public ObservableCollection<Employee> Employees
        {
            get => _Employees;
            set
            {
                if (_Employees != value)
                {
                    _Employees = value;
                    OnPropertyChanged("Employees");
                }
            }
        }
        private ObservableCollection<Employee> _ResultSearchEmployees;
        public ObservableCollection<Employee> ResultSearchEmployees
        {
            get => _ResultSearchEmployees;
            set
            {
                if (_ResultSearchEmployees != value)
                {
                    _ResultSearchEmployees = value;
                    OnPropertyChanged("ResultSearchEmployees");
                }
            }
        }
        private ObservableCollection<EmployeeDepartment> _EmployeeDepartmentsOfDepartmentSelected;
        public ObservableCollection<EmployeeDepartment> EmployeeDepartmentsOfDepartmentSelected
        {
            get => _EmployeeDepartmentsOfDepartmentSelected;
            set
            {
                if (_EmployeeDepartmentsOfDepartmentSelected != value)
                {
                    _EmployeeDepartmentsOfDepartmentSelected = value;
                    foreach (EmployeeDepartment employeeDepartment in _EmployeeDepartmentsOfDepartmentSelected)
                    {
                        employeeDepartment.Employee = _Employees.Where(x => x.Id == employeeDepartment.EmployeeId).FirstOrDefault();
                        employeeDepartment.Position= _Positions.Where(x => x.Id == employeeDepartment.PositionId).FirstOrDefault();
                    }
                    OnPropertyChanged("EmployeeDepartmentsOfDepartmentSelected");
                }
            }
        }

        private ObservableCollection<EmployeeDepartment> _EmployeeDepartments;
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
                    if (_ViewAllRadioBtn)
                    {
                        EmployeeDepartmentsOfDepartmentSelected = serviceFactory.LoadEmployeeDepartmentsByDepartmentId(_DepartmentSelected.Id);
                    }
                    else if (_ViewActiveRadioBtn)
                    {
                        EmployeeDepartmentsOfDepartmentSelected = serviceFactory.LoadEmployeeDepartmentsUnActiveByDepartmentId(_DepartmentSelected.Id);
                    }
                    else if (_ViewUnActiveRadioBtn)
                    {
                        EmployeeDepartmentsOfDepartmentSelected = serviceFactory.LoadEmployeeDepartmentsUnActiveByDepartmentId(_DepartmentSelected.Id);
                    }
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

        private string _SearchKeyWord;
        public string SearchKeyWord
        {
            get => _SearchKeyWord;
            set
            {
                if (_SearchKeyWord != value)
                {
                    _SearchKeyWord = value;
                    ResultSearchEmployees = SearchEmployeesByKey(SearchKeyWord);
                    OnPropertyChanged("SearchKeyWord");
                    OnPropertyChanged("ResultSearchEmployees");
                }
            }
        }
        private bool _ViewAllRadioBtn;
        public bool ViewAllRadioBtn
        {
            get => _ViewAllRadioBtn;
            set
            {
                if (_ViewAllRadioBtn != value)
                {
                    _ViewAllRadioBtn = value;
                    if(_ViewAllRadioBtn && _DepartmentSelected!=null)
                    {
                        EmployeeDepartmentsOfDepartmentSelected = serviceFactory.LoadEmployeeDepartmentsByDepartmentId(_DepartmentSelected.Id);
                    }                    
                    OnPropertyChanged("ViewAllRadioBtn");
                }
            }
        }
        private bool _ViewActiveRadioBtn;
        public bool ViewActiveRadioBtn
        {
            get => _ViewActiveRadioBtn;
            set
            {
                if (_ViewActiveRadioBtn != value)
                {
                    _ViewActiveRadioBtn = value;
                    if(_ViewActiveRadioBtn && _DepartmentSelected != null)
                    {
                        EmployeeDepartmentsOfDepartmentSelected = serviceFactory.LoadEmployeeDepartmentsActiveByDepartmentId(_DepartmentSelected.Id);
                    }    
                    
                    OnPropertyChanged("ViewActiveRadioBtn");
                }
            }
        }
        private bool _ViewUnActiveRadioBtn;
        public bool ViewUnActiveRadioBtn
        {
            get => _ViewUnActiveRadioBtn;
            set
            {
                if (_ViewUnActiveRadioBtn != value)
                {
                    _ViewUnActiveRadioBtn = value;
                    if (_ViewUnActiveRadioBtn && _DepartmentSelected != null)
                    {
                        EmployeeDepartmentsOfDepartmentSelected = serviceFactory.LoadEmployeeDepartmentsUnActiveByDepartmentId(_DepartmentSelected.Id);
                    }
                    OnPropertyChanged("ViewUnActiveRadioBtn");
                }
            }
        }


        private ObservableCollection<Employee> SearchEmployeesByKey(string key)
        {
            ObservableCollection<Employee> kq = _Employees.Where(e => e.FirtName.Contains(key) || e.LastName.Contains(key)|| e.MSNV.Contains(key)).ToObservableCollection();
            return kq;
        }

        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ApplyCommand { get; set; }
        public ICommand CloseDialogCommand { get; private set; }
        public ICommand RemoveEmployeeCommand { get; private set; }
        public ICommand LockDepartmentCommand { get; private set; }
        public ICommand UnLockDepartmentCommand { get; private set; }
        public ICommand NewDepartmentCommand { get; private set; }
        public ICommand DeleteDepartmentCommand { get; private set; }
        public EmployeeDepartmentManagerViewModel()
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
                    Employees = serviceFactory.LoadEmployees();
                    ViewAllRadioBtn = true;

                    //_EmployeeDepartments = serviceFactory.LoadUserDepartments();
                    //LoadUserAndDepartmentToUserDepartments();
                }
                catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
                finally { IsBusy = false; }
            });
            RemoveEmployeeCommand = new RelayCommand<EmployeeDepartment>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                MessageBoxResult dialogResult = System.Windows.MessageBox.Show($"Bạn có muốn xóa {p.Employee.LastName} {p.Employee.FirtName} khỏi {_DepartmentSelected?.Name}", "Cảnh báo !", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                {
                    serviceFactory.RemoveEmployeeToDepartment(p.EmployeeId, p.DepartmentId);
                    EmployeeDepartmentsOfDepartmentSelected = serviceFactory.LoadEmployeeDepartmentsByDepartmentId(_DepartmentSelected.Id);
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
            ApplyCommand = new RelayCommand<Object>((p) => { if (_EmployeeDepartmentSelected != null) return true; else return false; }, (p) =>
            {
                try
                {
                    EmployeeDepartmentSelected.FromDate = _FromDate;
                    EmployeeDepartmentSelected.ToDate = _ToDate;
                    EmployeeDepartmentSelected.PositionId = _PositionSelected.Id;
                    serviceFactory.SaveChangeEmployeeDepartment(EmployeeDepartmentSelected);
                    EmployeeDepartmentsOfDepartmentSelected = serviceFactory.LoadEmployeeDepartmentsByDepartmentId(_DepartmentSelected.Id);
                } catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
                
            });
            CloseDialogCommand = new RelayCommand<bool>((p) => { return true; }, (p) =>
            {
                if (p && _AddEmployeeToDepartment != null)
                {
                    serviceFactory.AddEmployeeToDepartment( new EmployeeDepartment()
                    {
                        DepartmentId = _DepartmentSelected.Id,
                        EmployeeId  = _AddEmployeeToDepartment.Id,
                        PositionId = _PositionSelected.Id
                    });
                    EmployeeDepartmentsOfDepartmentSelected = serviceFactory.LoadEmployeeDepartmentsByDepartmentId(_DepartmentSelected.Id);
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
                if (_SelectedNewPosition1 != null && _SelectedNewGroupDepartment != null && _SelectedNewPosition2 != null)
                {
                    serviceFactory.NewDepartment(_NewDepartmentCode, _NewDepartmentName, _SelectedNewGroupDepartment.Id, _SelectedNewPosition1.Id, _SelectedNewPosition2.Id);
                    Departments = serviceFactory.GetDepartments();
                }
            });
        }
    }
}
