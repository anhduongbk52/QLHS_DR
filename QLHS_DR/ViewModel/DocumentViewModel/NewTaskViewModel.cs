using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class NewTaskViewModel : BaseViewModel
    {
        #region "Properties and Field"
        private ObservableCollection<ReceiveDepartment> _ListReceiveDepartment;
        public ObservableCollection<ReceiveDepartment> ListReceiveDepartment { get => _ListReceiveDepartment; set { _ListReceiveDepartment = value; OnPropertyChanged("ListReceiveDepartment"); } }
        private Department _MyDepartment;
        public Department MyDepartment { get => _MyDepartment; set { _MyDepartment = value; OnPropertyChanged("MyDepartment"); } }
        private string _TaskName;
        public string TaskName { get => _TaskName; set { _TaskName = value; OnPropertyChanged("TaskName"); } }
        private string _TaskDrecription;
        public string TaskDrecription
        {
            get => _TaskDrecription;
            set
            {
                _TaskDrecription = value;
                OnPropertyChanged("TaskDrecription");
            }
        }
        private ObservableCollection<int> _ConfidentialLevels;
        public ObservableCollection<int> ConfidentialLevels
        {
            get => _ConfidentialLevels;
            set { _ConfidentialLevels = value; OnPropertyChanged("ConfidentialLevels"); }
        }
        private int _ConfidentialSelected;
        public int ConfidentialSelected
        {
            get => _ConfidentialSelected;
            set
            {
                _ConfidentialSelected = value;
                OnPropertyChanged("ConfidentialSelected");
            }
        }

        private Object _DocumentSourcePdf;
        public Object DocumentSourcePdf
        {
            get => _DocumentSourcePdf;
            set
            {
                _DocumentSourcePdf = value;
                OnPropertyChanged("DocumentSourcePdf");
            }
        }
        private bool _CanSaveFile;
        public bool CanSaveFile
        {
            get => _CanSaveFile;
            set { _CanSaveFile = value; OnPropertyChanged("CanSaveFile"); }
        }


        //private List<int> _UserNotInTaskIds;

        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand OkCommand { get; set; }

        #endregion
        internal NewTaskViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ConfidentialLevels = new ObservableCollection<int>() { 0, 1, 2, 3 };
                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                try
                {
                    ListReceiveDepartment = GetReceiveDepartments();
                    _MyClient.Open();
                    string mydeptName = _MyClient.GetDepartmentName(SectionLogin.Ins.CurrentUser.Id);
                    _MyClient.Close();
                    var defaultTemp = ListReceiveDepartment.Where(x => x.Department.Name.Contains("Kế hoạch")).FirstOrDefault();
                    if (defaultTemp != null)
                    {
                        defaultTemp.ReceivedDepartmentDTO = new ReceivedDepartmentDTO()
                        {
                            CanPrint = true,
                            CanSave = true,
                            CanViewFileAttachment = true,
                            IndexInTree = 1,
                            DepartmentId = defaultTemp.Department.Id,
                            IsProcess = true
                        };
                        defaultTemp.IsProcessTemp = true;
                    }
                    var defaultTemp1 = ListReceiveDepartment.Where(x => x.Department.Name.Contains(mydeptName)).FirstOrDefault();
                    if (defaultTemp1 != null)
                    {
                        defaultTemp1.ReceivedDepartmentDTO = new ReceivedDepartmentDTO()
                        {
                            CanPrint = true,
                            CanSave = true,
                            CanViewFileAttachment = true,
                            IndexInTree = 0,
                            DepartmentId = defaultTemp1.Department.Id,
                            IsProcess = true
                        };
                        defaultTemp1.IsProcessTemp = true;
                    }

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + "Function: LoadedWindowCommand");
                    if (ex.InnerException != null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.Message + "Function: LoadedWindowCommand");
                    }

                    _MyClient.Abort();
                }
            });
            OkCommand = new RelayCommand<Window>((p) => { if (_DocumentSourcePdf != null && _TaskDrecription != null && _TaskName != null) return true; else return false; }, (p) =>
            {
                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);

                try
                {
                    if (_ListReceiveDepartment.Any(x => x.IsProcessTemp || x.IsViewOnlyTemp))
                    {
                        _MyClient.Open();

                        Task task = new Task()
                        {
                            AssignedUserId = null, /*receive user*/
                            OwnerUserId = SectionLogin.Ins.CurrentUser.Id,
                            Description = _TaskDrecription,
                            EndDate = DateTime.Now.AddYears(3),
                            Reminder = false,
                            StartDate = DateTime.Now,
                            Status = TaskStatus.InProgess,
                            Subject = _TaskName,
                            Priority = TaskPriority.Normal,
                            CanSaveFile = _CanSaveFile
                        };
                        List<ReceivedDepartmentDTO> temDTo = new List<ReceivedDepartmentDTO>();
                        foreach (var receiveDept in _ListReceiveDepartment)
                        {
                            if (receiveDept.IsProcessTemp || receiveDept.IsViewOnlyTemp)
                                temDTo.Add(receiveDept.ReceivedDepartmentDTO);
                        }
                        string pathFile = DocumentSourcePdf.ToString();
                        TaskAttachedFileDTO taskAttachedFileDTO = new TaskAttachedFileDTO()
                        {
                            ModifiedBy = SectionLogin.Ins.CurrentUser.Id,
                            FileName = System.IO.Path.GetFileName(DocumentSourcePdf.ToString()),
                            Content = System.IO.File.ReadAllBytes(pathFile),
                            ConfidentialLevel = _ConfidentialSelected
                        };

                        _MyClient.NewTask(task, temDTo.ToArray(), taskAttachedFileDTO);
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
                    foreach (var dep in departments)
                    {
                        dep.GroupDepartment = _MyClient.GetGroupDepartment(dep.GroupDepartmentId);
                        ReceivedDepartmentDTO receivedDepartmentDTO = new ReceivedDepartmentDTO()
                        {
                            CanPrint = false,
                            CanSave = false,
                            CanViewFileAttachment = false,
                            DepartmentId = dep.Id,
                            IndexInTree = 1,
                            IsProcess = false,
                            IsViewOnly = false,
                        };
                        result.Add(new ReceiveDepartment()
                        {
                            Department = dep,
                            ReceivedDepartmentDTO = receivedDepartmentDTO
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

}
