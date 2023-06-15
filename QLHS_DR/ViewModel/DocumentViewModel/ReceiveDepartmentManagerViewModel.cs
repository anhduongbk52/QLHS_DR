using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class ReceiveDepartmentManagerViewModel : BaseViewModel
    {
        #region "Properties and Field"
        private bool _DataChanged;
        public bool DataChanged
        {
            get => _DataChanged;
            set
            {
                if (_DataChanged != value)
                {
                    _DataChanged = value; OnPropertyChanged("DataChanged");
                }
            }
        }
        private string _WindowTitle;
        private MessageServiceClient _MyClient;
        public string WindowTitle { get => _WindowTitle; set { _WindowTitle = value; OnPropertyChanged("WindowTitle"); } }

        private Task _Task;
        public Task Task { get => _Task; set { _Task = value; OnPropertyChanged("Task"); } }
        private Stream _PdfContent;
        public Stream PdfContent { get => _PdfContent; set { _PdfContent = value; OnPropertyChanged("PdfContent"); } }

        private ObservableCollection<ReceiveDepartment> _ListReceiveDepartment;
        public ObservableCollection<ReceiveDepartment> ListReceiveDepartment
        {
            get => _ListReceiveDepartment;
            set
            {
                if (_ListReceiveDepartment != value)
                {
                    _ListReceiveDepartment = value; OnPropertyChanged("ListReceiveDepartment");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand OkCommand { get; set; }
        private void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO, UserTask userTask)
        {
            FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            byte[] orAdd = fileHelper.GetKeyDecryptOfTask(taskAttachedFileDTO.TaskId, userTask);
            if (orAdd != null)
            {
                taskAttachedFileDTO.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO.Content);
            }
        }
        #endregion
        internal ReceiveDepartmentManagerViewModel(UserTask userTask)
        {
            try
            {
                Task = userTask.Task;
                WindowTitle = userTask.Task.Subject;
                ListReceiveDepartment = GetReceiveDepartments();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    var taskAttachedFileDTOs = _MyClient.GetTaskDocuments(_Task.Id); //get all file PDF in task
                    _MyClient.SetSeenUserInTask(_Task.Id, SectionLogin.Ins.CurrentUser.Id);
                    _MyClient.Close();
                    if (taskAttachedFileDTOs != null && taskAttachedFileDTOs.Length > 0)
                    {
                        DecryptTaskAttachedFile(taskAttachedFileDTOs[0], userTask);
                        PdfContent = new MemoryStream(taskAttachedFileDTOs[0].Content);
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _MyClient.Abort();
                }
            });
            OkCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                try
                {
                    if (_ListReceiveDepartment.Any(x => x.IsProcessTemp || x.IsViewOnlyTemp))
                    {
                        _MyClient.Open();

                        List<ReceivedDepartmentDTO> temDTo = new List<ReceivedDepartmentDTO>();
                        foreach (var receiveDept in _ListReceiveDepartment)
                        {
                            if ((receiveDept.IsProcessTemp || receiveDept.IsViewOnlyTemp) && receiveDept.IsReceived)
                                temDTo.Add(receiveDept.ReceivedDepartmentDTO);
                        }
                        _MyClient.AddDepartmentToTask(_Task, temDTo.ToArray());
                        MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Bạn có muốn chuyển công việc này sang mục đã kết thúc ?", "?", MessageBoxButton.OKCancel);
                        if (dialogResult == MessageBoxResult.OK)
                        {
                            _MyClient.SetUserTaskFinish(_Task.Id, SectionLogin.Ins.CurrentUser.Id, true);
                        }
                        _MyClient.Close();
                        p.Close();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Bạn phải chọn tối thiểu một đơn vị xử lý hoặc xem");
                    };
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + " At OkCommand");
                    _MyClient.Abort();
                }
            });

            CancelCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                p.Close();
            });
        }

        private ObservableCollection<ReceiveDepartment> GetReceiveDepartments()
        {
            ObservableCollection<ReceiveDepartment> result = new ObservableCollection<ReceiveDepartment>();
            try
            {
                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                var departments = _MyClient.GetDepartments();
                if (departments != null)
                {
                    foreach (var department in departments)
                    {
                        department.GroupDepartment = _MyClient.GetGroupDepartment(department.GroupDepartmentId);
                        var departmentTask = _MyClient.GetDepartmentTask(department.Id, _Task.Id);
                        ReceivedDepartmentDTO receivedDepartmentDTO = new ReceivedDepartmentDTO();
                        if (departmentTask != null)
                        {
                            receivedDepartmentDTO.CanPrint = departmentTask.CanPrint;
                            receivedDepartmentDTO.CanSave = departmentTask.CanSave;
                            receivedDepartmentDTO.CanViewFileAttachment = departmentTask.CanViewFileAttachment;
                            receivedDepartmentDTO.DepartmentId = department.Id;
                            receivedDepartmentDTO.IndexInTree = departmentTask.IndexInTree;
                            receivedDepartmentDTO.IsProcess = departmentTask.IsProcess;
                            receivedDepartmentDTO.IsViewOnly = !departmentTask.IsProcess;
                        }
                        else
                        {
                            receivedDepartmentDTO.CanPrint = false;
                            receivedDepartmentDTO.CanSave = false;
                            receivedDepartmentDTO.CanViewFileAttachment = false;
                            receivedDepartmentDTO.DepartmentId = department.Id;
                            receivedDepartmentDTO.IndexInTree = 1;
                            receivedDepartmentDTO.IsProcess = false;
                            receivedDepartmentDTO.IsViewOnly = false;
                        }
                        bool isEnable = (!receivedDepartmentDTO.IsProcess) && (!receivedDepartmentDTO.IsViewOnly);
                        result.Add(new ReceiveDepartment()
                        {
                            IsReceived = (isEnable),
                            ReceivedDepartmentDTO = receivedDepartmentDTO,
                            IsProcessTemp = receivedDepartmentDTO.IsProcess,
                            IsViewOnlyTemp = receivedDepartmentDTO.IsViewOnly,
                            Department = department,
                            IsEnablePermission = isEnable,
                        });
                    }
                }
                _MyClient.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "Function: GetReceiveDepartments");
            }
            return result;
        }
    }
    internal class ReceiveDepartment : BaseViewModel
    {
        private bool _IsReceived;
        public bool IsReceived
        {
            get { return _IsReceived; }
            set
            {
                if (_IsReceived != value)
                {
                    _IsReceived = value;
                    OnPropertyChanged("IsReceived");
                }
            }
        }
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
                ReceivedDepartmentDTO.CanViewFileAttachment = ReceivedDepartmentDTO.CanPrint = (_IsProcessTemp || _IsViewOnlyTemp);
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
                ReceivedDepartmentDTO.CanViewFileAttachment = ReceivedDepartmentDTO.CanPrint = (_IsProcessTemp || _IsViewOnlyTemp);
                if ((!_IsProcessTemp) && (!_IsViewOnlyTemp)) ReceivedDepartmentDTO.CanSave = false;
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
