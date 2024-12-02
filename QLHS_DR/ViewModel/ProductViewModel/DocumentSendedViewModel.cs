using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using Prism.Events;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.DocumentView;
using QLHS_DR.ViewModel.DocumentViewModel;
using QLHS_DR.ViewModel.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ProductViewModel
{
   
    internal class DocumentSendedViewModel : BaseViewModel
    {
        #region "Properties and Field"
        private bool _IsExpander;
        public bool IsExpander
        {
            get => _IsExpander;
            set
            {
                if (_IsExpander != value)
                {
                    _IsExpander = value; OnPropertyChanged("IsExpander");
                }
            }
        }
        private readonly IEventAggregator _eventAggregator;
        private ObservableCollection<Department> _Departments;
        public ObservableCollection<Department> Departments
        {
            get => _Departments;
            set
            {
                if (_Departments != value)
                {
                    _Departments = value; OnPropertyChanged("Departments");
                }
            }
        }
        private ObservableCollection<UserDepartment> _UserDepartments;
        public ObservableCollection<UserDepartment> UserDepartments
        {
            get => _UserDepartments;
            set
            {
                if (_UserDepartments != value)
                {
                    _UserDepartments = value; OnPropertyChanged("UserDepartments");
                }
            }
        }
        private MessageServiceClient _MyClient;
        private IReadOnlyList<User> iReadOnlyListUser;
        private ConcurrentDictionary<int, byte[]> _ListFileDecrypted = new ConcurrentDictionary<int, byte[]>();

        private bool _IsReadOnlyPermission;
        public bool IsReadOnlyPermission
        {
            get => _IsReadOnlyPermission;
            set
            {
                if (_IsReadOnlyPermission != value)
                {
                    _IsReadOnlyPermission = value; OnPropertyChanged("IsReadOnlyPermission");
                }
            }
        }

        private UserTask _UserTaskSelected;
        public UserTask UserTaskSelected
        {
            get => _UserTaskSelected;
            set
            {
                if (_UserTaskSelected != value)
                {
                    _UserTaskSelected = value; OnPropertyChanged("UserTaskSelected");
                }
            }
        }
        private Task _TaskSelected;
        public Task TaskSelected
        {
            get => _TaskSelected;
            set
            {
                if (_TaskSelected != value)
                {
                    _TaskSelected = value; OnPropertyChanged("TaskSelected");
                }
            }
        }

        private ObservableCollection<Task> _ListTaskOfUser;
        public ObservableCollection<Task> ListTaskOfUser
        {
            get => _ListTaskOfUser;
            set
            {
                if (_ListTaskOfUser != value)
                {
                    _ListTaskOfUser = value; OnPropertyChanged("ListTaskOfUser");
                }
            }
        }
        private ObservableCollection<User> _UsersInTask;
        public ObservableCollection<User> UsersInTask
        {
            get => _UsersInTask;
            set
            {
                if (_UsersInTask != value)
                {
                    _UsersInTask = value; OnPropertyChanged("UsersInTask");
                }
            }
        }
        private ObservableCollection<UserTask> _ListUserTaskOfTask;
        public ObservableCollection<UserTask> ListUserTaskOfTask
        {
            get => _ListUserTaskOfTask;
            set
            {
                if (_ListUserTaskOfTask != value)
                {
                    _ListUserTaskOfTask = value; OnPropertyChanged("ListUserTaskOfTask");

                }
            }
        }
        #endregion

        #region "Command"
        public ICommand OpenReceiveUserManagerCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand TaskSelectedCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand RequirePermisionCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }
        public ICommand RevokeTaskCommand { get; set; }
        public ICommand AddUserOfMyDepartmentToTaskCommand { get; set; }
        public ICommand OpenReceiveDepartmentManagerCommand { get; set; }
        #endregion

        public DocumentSendedViewModel(int productId)
        {            
            IsReadOnlyPermission = true;
            IsExpander = false;
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                iReadOnlyListUser = _MyClient.GetUserContacts(SectionLogin.Ins.CurrentUser.UserName);
                Departments = _MyClient.GetDepartments().ToObservableCollection();
                UserDepartments = _MyClient.LoadUserDepartments().ToObservableCollection();
                _MyClient.Close();
                foreach (var ud in _UserDepartments)
                {
                    ud.Department = _Departments.Where(x => x.Id == ud.DepartmentId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _MyClient.Abort();
                System.Windows.MessageBox.Show(ex.Message);

            }

            UsersInTask = new ObservableCollection<User>();
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                IsReadOnlyPermission = !SectionLogin.Ins.Permissions.HasFlag(PermissionType.CHANGE_PERMISSION);
                ListTaskOfUser = LoadTasksOfProduct(productId);
            });
            OpenFileCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null) return true; else return false; }, (p) =>
            {
                OpenFilePdf();
               
            });
            EditTaskCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskEditTask") && _TaskSelected != null) return true; else return false; }, (p) =>
            {
                EditTaskWindow editTaskWindow = new EditTaskWindow(_TaskSelected);
                editTaskWindow.ShowDialog();
                ListTaskOfUser = LoadTasksOfProduct(productId);
               
            });
            RequirePermisionCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null) return true; else return false; }, (p) =>
            {
                RequestPermissionDocumentWindow requestPermissionDocumentWindow = new RequestPermissionDocumentWindow(_TaskSelected.Id);
                requestPermissionDocumentWindow.ShowDialog();
            });
            OpenReceiveUserManagerCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.Permissions.HasFlag(PermissionType.ADD_USER_TO_TASK) && _TaskSelected != null) return true; else return false; }, (p) =>
            {
                ReceiveUserManagerWD receiveUserManagerWD = new ReceiveUserManagerWD();
                ReceiveUserManagerViewModel receiveUserManagerViewModel = new ReceiveUserManagerViewModel(_TaskSelected);
                receiveUserManagerWD.DataContext = receiveUserManagerViewModel;
                receiveUserManagerWD.ShowDialog();
                ListTaskOfUser = LoadTasksOfProduct(productId);
            });
            TaskSelectedCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null && _IsExpander) return true; else return false; }, (p) =>
            {
                ListUserTaskOfTask = new ObservableCollection<UserTask>();
                try
                {
                    ListUserTaskOfTask = GetUserTasksOfTask(_TaskSelected.Id);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    if (ex.InnerException != null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.StackTrace);
                    }
                }
            });
            RevokeTaskCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null && (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskRevokeTask") || (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskRevokeTaskByOwner") && _UserTaskSelected.Task.OwnerUserId == SectionLogin.Ins.CurrentUser.Id))) return true; else return false; }, (p) =>
            {
                try
                {
                    MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Bạn có muốn thu hồi tài liệu này? Thao tác này sẽ xóa toàn bộ dữ liệu liên quan", "Cảnh báo !", MessageBoxButton.OKCancel);
                    if (dialogResult == MessageBoxResult.OK)
                    {
                        _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                        _MyClient.Open();

                        _MyClient.RevokeTaskByCurrentUser(_TaskSelected.Id);
                        _MyClient.Close();
                        
                        
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    if (ex.InnerException != null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.Message);
                    }
                    _MyClient.Abort();
                }
            });
            AddUserOfMyDepartmentToTaskCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskAddUserOfMyDepartment") && _TaskSelected != null) return true; else return false; }, (p) =>
            {
                ReceiveUserOfMyDepartmentManagerWD receiveUserOfMyDepartmentManagerWD = new ReceiveUserOfMyDepartmentManagerWD();
                ReceiveUserOfMyDepartmentManagerViewModel receiveUserOfMyDepartmentManagerViewModel = new ReceiveUserOfMyDepartmentManagerViewModel(_TaskSelected);
                receiveUserOfMyDepartmentManagerWD.DataContext = receiveUserOfMyDepartmentManagerViewModel;
                receiveUserOfMyDepartmentManagerWD.ShowDialog();
                ListTaskOfUser = LoadTasksOfProduct(productId);
            });
            OpenReceiveDepartmentManagerCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null && SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskAddDepartmentToTask")) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    _UserTaskSelected = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, _TaskSelected.Id);
                    _UserTaskSelected.Task = _TaskSelected;
                    _MyClient.Close();

                    ReceiveDepartmentManagerWD receiveDepartmentManagerWD = new ReceiveDepartmentManagerWD();
                    
                    ReceiveDepartmentManagerViewModel receiveDepartmentManagerViewModel = new ReceiveDepartmentManagerViewModel(_UserTaskSelected);
                    receiveDepartmentManagerViewModel.WindowTitle = _TaskSelected.Subject;
                    receiveDepartmentManagerWD.DataContext = receiveDepartmentManagerViewModel;
                    receiveDepartmentManagerWD.ShowDialog();
                    receiveDepartmentManagerViewModel.Dispose();
                    ListTaskOfUser = LoadTasksOfProduct(productId);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + " - OpenReceiveDepartmentManagerCommand");
                    if (ex.InnerException != null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.StackTrace);
                    }
                }
            });
        }

       
        
        private void OpenFilePdf()
        {
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();

                _UserTaskSelected = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, _TaskSelected.Id);

                if (_UserTaskSelected != null && _UserTaskSelected.CanViewAttachedFile == true)
                {
                    _UserTaskSelected.Task = _TaskSelected;
                    PermissionType taskPermissions = _MyClient.GetTaskPermissions(SectionLogin.Ins.CurrentUser.Id, _UserTaskSelected.TaskId);
                    bool signable = SectionLogin.Ins.Permissions.HasFlag(PermissionType.REVIEW_DOCUMENT | PermissionType.SIGN_DOCUMENT);
                    bool printable = signable | taskPermissions.HasFlag(PermissionType.PRINT_DOCUMENT) | (_TaskSelected.OwnerUserId == SectionLogin.Ins.CurrentUser.Id);
                    bool saveable = _UserTaskSelected.CanSave.HasValue ? _UserTaskSelected.CanSave.Value : false;

                    var taskAttachedFileDTOs = _MyClient.GetTaskDocuments(_UserTaskSelected.TaskId); //get all file PDF in task
                    if (taskAttachedFileDTOs != null && taskAttachedFileDTOs.Length > 0)
                    {
                        TaskAttackFileViewerViewModel taskAttackFileViewerViewModel = new TaskAttackFileViewerViewModel(taskAttachedFileDTOs[0], printable, saveable, _UserTaskSelected);
                        taskAttackFileViewerViewModel.FileName = taskAttachedFileDTOs[0].FileName;
                        taskAttackFileViewerViewModel.TaskName = _UserTaskSelected.Task.Subject;

                        TaskAttackFileViewerWindow taskAttackFileViewerWindow = new TaskAttackFileViewerWindow();
                        taskAttackFileViewerWindow.DataContext = taskAttackFileViewerViewModel;
                        taskAttackFileViewerWindow.Show();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Không tìm thấy file đính kèm");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Bạn chưa có quyền xem tài liệu này, vui lòng liên hệ quản trị viên!");
                }
                _MyClient.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                if (ex.InnerException != null)
                {
                    System.Windows.MessageBox.Show(ex.InnerException.Message);
                }
                _MyClient.Abort();
            }
        }
        public void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO, UserTask userTask)
        {
            FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            byte[] orAdd = _ListFileDecrypted.GetOrAdd(taskAttachedFileDTO.TaskId, (int int_0) => fileHelper.GetKeyDecryptOfTask(userTask));
            if (orAdd != null)
            {
                taskAttachedFileDTO.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO.Content);
            }
        }
        public ObservableCollection<Task> LoadTasksOfProduct(int productId)
        {
            ObservableCollection<Task> ketqua = new ObservableCollection<Task>();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                ketqua = _MyClient.GetTaskOfProduct(productId).OrderByDescending(x => x.StartDate).ToObservableCollection();
                _MyClient.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                if (ex.InnerException != null)
                {
                    System.Windows.MessageBox.Show(ex.InnerException.Message);
                }
                _MyClient.Abort();
            }
            return ketqua;
        }
        public ObservableCollection<UserTask> GetUserTasksOfTask(int taskId)
        {
            ObservableCollection<UserTask> ketqua = new ObservableCollection<UserTask>();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                UsersInTask = _MyClient.GetUserInTask(_TaskSelected.Id).ToObservableCollection();
                ketqua = _MyClient.GetAllUserTaskOfTask(_TaskSelected.Id).ToObservableCollection();
                foreach (var ut in ketqua)
                {
                    ut.User = _UsersInTask.Where(x => x.Id == ut.UserId).FirstOrDefault();
                    ut.User.UserDepartments = _UserDepartments.Where(x => x.UserId == ut.User.Id).ToArray();
                }
                _MyClient.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                if (ex.InnerException != null)
                {
                    System.Windows.MessageBox.Show(ex.InnerException.Message);
                }
                _MyClient.Abort();
            }
            return ketqua;
        }
    }
}
