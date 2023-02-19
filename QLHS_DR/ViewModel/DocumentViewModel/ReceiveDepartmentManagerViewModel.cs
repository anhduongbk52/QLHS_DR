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
                        //Department = dept,
                        //IsReceive = false
                    };
                    ReceiveDepartments.Add(receiveDepartment);
                }
                _MyClient.Close();
            });

        }

    }
    internal class ReceiveDepartment:BaseViewModel
    {
        private bool _IsEnablePermission;
        public bool IsEnablePermission
        {
            get { return _IsEnablePermission; }
            set
            {
                if (_IsEnablePermission != value)
                {
                    _IsEnablePermission = value; 
                    OnPropertyChanged("IsEnablePermission");
                }
            }
        }
        private bool _IsProcessTemp;
        public bool IsProcessTemp
        {
            get { return _IsProcessTemp; }
            set
            {
                if (value == true)
                {
                    _IsProcessTemp = true;
                    _IsViewOnlyTemp = false;
                    _ReceivedDepartmentDTO.IsProcess = true;
                    _ReceivedDepartmentDTO.IsViewOnly = false;
                }
                else
                {
                    _IsProcessTemp = false;                   
                    _ReceivedDepartmentDTO.IsProcess = false;
                }
                ReceivedDepartmentDTO.CanPrint = (_IsProcessTemp || _IsViewOnlyTemp);
                if ((!_IsProcessTemp) && (!_IsViewOnlyTemp)) ReceivedDepartmentDTO.CanSave = false;
                IsEnablePermission = _IsProcessTemp || _IsViewOnlyTemp;
                OnPropertyChanged("IsProcessTemp");
                OnPropertyChanged("IsViewOnlyTemp");
            }
        }
        private bool _IsViewOnlyTemp;
        public bool IsViewOnlyTemp
        {
            get { return _IsViewOnlyTemp; }
            set
            {
                if (value == true)
                {
                    _IsViewOnlyTemp = true;
                    _IsProcessTemp = false;
                    _ReceivedDepartmentDTO.IsProcess = false;
                    _ReceivedDepartmentDTO.IsViewOnly = true;
                }
                else
                {
                    _IsViewOnlyTemp = false;
                    _ReceivedDepartmentDTO.IsViewOnly = false;
                }
                ReceivedDepartmentDTO.CanPrint = (_IsProcessTemp || _IsViewOnlyTemp);
               if((!_IsProcessTemp) && (!_IsViewOnlyTemp)) ReceivedDepartmentDTO.CanSave=false;
                IsEnablePermission = (_IsProcessTemp || _IsViewOnlyTemp);
                OnPropertyChanged("IsProcessTemp");
                OnPropertyChanged("IsViewOnlyTemp");
            }
        }
        private ReceivedDepartmentDTO _ReceivedDepartmentDTO;
        public ReceivedDepartmentDTO ReceivedDepartmentDTO
        {
            get { return _ReceivedDepartmentDTO; }
            set
            {
                if (_ReceivedDepartmentDTO != value)
                {
                    _ReceivedDepartmentDTO = value;
                    OnPropertyChanged("ReceivedDepartmentDTO");
                }
            }
        }
        
        private Department _Department;
        public Department Department
        {
            get { return _Department; }
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
