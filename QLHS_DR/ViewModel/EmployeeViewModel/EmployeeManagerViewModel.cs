using DevExpress.Mvvm.POCO;
using DevExpress.Pdf;
using DevExpress.XtraRichEdit;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.EmployeeView;
using QLHS_DR.ViewModel.LsxViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.EmployeeViewModel
{
    internal class EmployeeManagerViewModel : BaseViewModel
    {
        #region "Properties and Fields"
        ServiceFactory _ServiceFactory;
        public EmployeePanelViewModel EmployeePanelViewModel { get; } = new EmployeePanelViewModel();
        
        private ViewSettings _ViewSettings;
        public ViewSettings ViewSettings
        {
            get => _ViewSettings;
            set
            {
                if (_ViewSettings != value)
                {
                    _ViewSettings = value;                    
                    NotifyPropertyChanged("ViewSettings");
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
                    NotifyPropertyChanged("Employees");
                }
            }
        }
        private Employee _SelectedEmployee;
        public Employee SelectedEmployee
        {
            get => _SelectedEmployee;
            set
            {
                if (_SelectedEmployee != value)
                {
                    _SelectedEmployee = value;
                    if(_SelectedEmployee!=null)
                    {
                        EmployeePanelViewModel.Employee = value;
                        ViewSettings.IsDataPaneVisible = true;
                    }
                    else ViewSettings.IsDataPaneVisible = false;
                    NotifyPropertyChanged("ViewSettings");
                    NotifyPropertyChanged("SelectedEmployee");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand NewEmployeeCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        #endregion
        internal EmployeeManagerViewModel()
        {           
            _ServiceFactory = new ServiceFactory();
            Employees = new ObservableCollection<Employee>();
            ViewSettings = new ViewSettings();

            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, async (p) => await LoadEmployeesAsync());
            RefreshCommand =      new RelayCommand<Object>((p) => { return true; }, async (p) => await LoadEmployeesAsync());
            NewEmployeeCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                NewEmployeeWindow newEmployeeWindow = new NewEmployeeWindow();
                newEmployeeWindow.Show();
            });
            


        }
        private async System.Threading.Tasks.Task LoadEmployeesAsync()
        {
            var employees = await  _ServiceFactory.LoadEmployeesAsync();
            Employees.Clear();
            foreach (var employee in employees)
            {
                Employees.Add(employee);
            }
        }
    }
    public class ViewSettings {
        public bool IsDataPaneVisible { get; set; }
        
        public ViewSettings()
        {
            IsDataPaneVisible = false;
        }
    }
}
