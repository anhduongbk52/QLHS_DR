using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.EmployeeViewModel
{
    internal class NewEmployeeViewModel:BaseViewModel
    {
        #region "Properties and Fields"
        public List<string> GenderList { get; set; } = new List<string> { "Nam", "Nữ" };

        private bool _CanNewEmployee;
        public bool CanNewEmployee
        {
            get => _CanNewEmployee;
            set
            {
                if (_CanNewEmployee != value)
                {
                    _CanNewEmployee = value;                    
                    NotifyPropertyChanged("CanNewEmployee");
                }
            }
        }
        private DateTime _FromDate;
        public DateTime FromDate
        {
            get => _FromDate;
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyPropertyChanged("FromDate");
                }
            }
        }
        ServiceFactory _ServiceFactory = new ServiceFactory();
        private ObservableCollection<Department> _Departments;
        public ObservableCollection<Department> Departments
        {
            get => _Departments;
            set
            {
                if (_Departments != value)
                {
                    _Departments = value;
                    NotifyPropertyChanged("Departments");
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
                    NotifyPropertyChanged("DepartmentSelected");
                }
            }
        }
        private ObservableCollection<Position> _Positions;
        public ObservableCollection<Position> Positions
        {
            get => _Positions;
            set
            {
                if (_Positions != value)
                {
                    _Positions = value;
                    NotifyPropertyChanged("Positions");
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
                    NotifyPropertyChanged("PositionSelected");
                }
            }
        }
        private Employee _Employee;
        public Employee Employee
        {
            get => _Employee;
            set
            {
                if (_Employee != value)
                {
                    _Employee = value;                    
                    NotifyPropertyChanged("Employee");
                }
            }
        }
        private byte[] _EmployeeAvatar;
        public byte[] EmployeeAvatar
        {
            get => _EmployeeAvatar;
            set
            {
                if (_EmployeeAvatar != value)
                {
                    _EmployeeAvatar = value;
                    NotifyPropertyChanged("EmployeeAvatar");
                }
            }
        }
        
        
        #endregion
        #region "Command"

        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion
        internal NewEmployeeViewModel()
        {
            _ServiceFactory = new ServiceFactory();
            _Departments = _ServiceFactory.GetDepartments();
            _Positions = _ServiceFactory.LoadPositions();
            Employee = new Employee();
            FromDate = DateTime.Today;
            CanNewEmployee = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "employeeNewEmployee");
            SaveCommand = new RelayCommand<Window>((p) => { if (CanNewEmployee && _Employee.MSNV!=null && _Employee.FirtName!=null && _Employee.LastName!=null) return true; else return false; }, (p) =>
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Bạn có muốn thêm mới User!", "Cảnh báo", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        int employeeId = _ServiceFactory.NewEmployee(_Employee, _EmployeeAvatar);
                        if (employeeId != 0)
                        {
                            if(_DepartmentSelected!=null && _PositionSelected!=null)
                            {
                                _ServiceFactory.AddEmployeeToDepartment(new EmployeeDepartment()
                                {
                                     DepartmentId = _DepartmentSelected.Id,
                                     EmployeeId = employeeId,
                                     PositionId = _PositionSelected.Id,
                                     FromDate = _FromDate
                                });
                            }
                            System.Windows.MessageBox.Show("Thêm mới thành công");
                            p.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                        System.Windows.MessageBox.Show(ex.StackTrace);
                    }
                    finally
                    {

                    }
                }
            });
            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });
        }
    }
}
