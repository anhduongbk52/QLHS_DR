using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.Core;
using QLHS_DR.EOfficeServiceReference;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class ReceiveDepartmentManagerViewModel:BaseViewModel
    {
        #region "Properties and Field"
        private ObservableCollection<ReceiveDepartment> _ReceiveDepartments;
        public ObservableCollection<ReceiveDepartment> ReceiveDepartments
        {
            get => _ReceiveDepartments;
            set
            {
                if (_ReceiveDepartments != value)
                {
                    _ReceiveDepartments = value; OnPropertyChanged("ReceiveDepartments");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }      
        #endregion
        internal ReceiveDepartmentManagerViewModel()
        {
            ReceiveDepartments = new ObservableCollection<ReceiveDepartment>();
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                EofficeMainServiceClient _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                var depts= _MyClient.GetDepartments();
                
                foreach(var dept in depts)
                {
                    ReceiveDepartment receiveDepartment = new ReceiveDepartment()
                    {
                        Department = dept,
                        IsReceive = false
                    };
                    ReceiveDepartments.Add(receiveDepartment);
                }
                _MyClient.Close();
            });
        }

    }
    internal class ReceiveDepartment : BaseViewModel
    {
        private bool _IsReceive;
        public bool IsReceive
        {
            get => _IsReceive;
            set
            {
                if (_IsReceive != value)
                {
                    _IsReceive = value; OnPropertyChanged("IsReceive");
                }
            }
        }
        private Department _Department;
        public Department Department
        {
            get => _Department;
            set
            {
                if (_Department != value)
                {
                    _Department = value; OnPropertyChanged("Department");
                }
            }
        }
    }


}
