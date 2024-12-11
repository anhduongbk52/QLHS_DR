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

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;
                    CanEdit = !value;
                    NotifyPropertyChanged("IsReadOnly");
                }
            }
        }
        private bool _CanEdit;
        public bool CanEdit
        {
            get => _CanEdit;
            set
            {
                if (_CanEdit != value)
                {
                    _CanEdit = value;
                    NotifyPropertyChanged("CanEdit");
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
                    if (Employee != null)
                    {
                        EmployeeAvatar = _ServiceFactory.GetAvatar(_Employee.Id);
                        Employee.EmployeeDepartments = _ServiceFactory.LoadEmployeeDepartments(Employee.Id).ToArray();
                        for (int i = 0; i < Employee.EmployeeDepartments.Length; i++)
                        {
                            Employee.EmployeeDepartments[i].Department = _Departments.Where(x => x.Id == Employee.EmployeeDepartments[i].DepartmentId).FirstOrDefault();
                            Employee.EmployeeDepartments[i].Position = _Positions.Where(x => x.Id == Employee.EmployeeDepartments[i].PositionId).FirstOrDefault();
                        }
                    }
                    NotifyPropertyChanged("Employee");
                    NotifyPropertyChanged("Employee.EmployeeDepartments");
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
            IsReadOnly = !SectionLogin.Ins.ListPermissions.Any(x => x.Code == "employeeEditEmployeeInfo");
            SaveCommand = new RelayCommand<Object>((p) => { if (!IsReadOnly) return true; else return false; }, (p) =>
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Bạn có muốn lưu thay đổi không", "Cảnh báo", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        if (_ServiceFactory.SaveChangeEmployee(_Employee))
                        {
                            System.Windows.MessageBox.Show("Cập nhật thành công");
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
