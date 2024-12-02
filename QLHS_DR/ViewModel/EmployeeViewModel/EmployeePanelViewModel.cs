using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using QLHS_DR.ChatAppServiceReference;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Forms;
using EofficeClient.Core;

namespace QLHS_DR.ViewModel.EmployeeViewModel
{
    internal class EmployeePanelViewModel:BaseViewModel
    {
        #region "Properties and Fields"
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
        ObservableCollection<Department> _Departments;
        ObservableCollection<Position> _Positions;
        private Employee _Employee;
        public Employee Employee
        {
            get => _Employee;
            set
            {
                if (_Employee != value)
                {
                    _Employee = value;
                    if(Employee != null)
                    {
                        EmployeeAvatar = _ServiceFactory.GetAvatar(_Employee.Id);
                        Employee.EmployeeDepartments = _ServiceFactory.LoadEmployeeDepartments(Employee.Id).ToArray();
                        for (int i = 0; i < Employee.EmployeeDepartments.Length; i++)
                        {
                            Employee.EmployeeDepartments[i].Department = _Departments.Where(x => x.Id == Employee.EmployeeDepartments[i].DepartmentId).FirstOrDefault();
                            Employee.EmployeeDepartments[i].Position = _Positions.Where(x=>x.Id == Employee.EmployeeDepartments[i].PositionId).FirstOrDefault();
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
      
        public ICommand SaveChangeEmployeeInfoCommand { get; set; }
        #endregion
        internal EmployeePanelViewModel()
        {
            _ServiceFactory = new ServiceFactory();
            _Departments = _ServiceFactory.GetDepartments();
            _Positions = _ServiceFactory.LoadPositions();
            IsReadOnly = !SectionLogin.Ins.ListPermissions.Any(x => x.Code == "employeeEditEmployeeInfo");
            SaveChangeEmployeeInfoCommand = new RelayCommand<Object>((p) => { if (!IsReadOnly) return true; else return false; }, (p) =>
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Bạn có muốn lưu thay đổi không", "Cảnh báo", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        //_ServiceFactory.GetAllProductIdOfTask(42376);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    finally
                    {
                        
                    }
                }
            });
        }
    }
}
