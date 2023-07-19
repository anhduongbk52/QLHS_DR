using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using Prism.Events;
using QLHS_DR;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.DocumentView;
using QLHS_DR.ViewModel;
using QLHS_DR.ViewModel.DocumentViewModel;
using QLHS_DR.ViewModel.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace EofficeClient.ViewModel.DocumentViewModel
{

    class ListNewDocumentViewModel : BaseViewModel
    {
        #region "Properties and Field"

        private readonly IEventAggregator _eventAggregator;
        private MessageServiceClient _MyClient;
        private IReadOnlyList<User> iReadOnlyListUser;
        private ConcurrentDictionary<int, byte[]> _ListFileDecrypted = new ConcurrentDictionary<int, byte[]>();
        private bool _TrackChange;
      
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
        private ObservableCollection<UserTask> _UserTaskSelecteds;
        public ObservableCollection<UserTask> UserTaskSelecteds
        {
            get => _UserTaskSelecteds;
            set
            {
                if (_UserTaskSelecteds != value)
                {
                    _UserTaskSelecteds = value; OnPropertyChanged("UserTaskSelecteds");
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
        private ObservableCollection<UserTask> _ListUserTaskOfTaskOrigin;
        private ObservableCollection<UserTask> _ListUserTaskOfTask;
        public ObservableCollection<UserTask> ListUserTaskOfTask
        {
            get => _ListUserTaskOfTask;
            set
            {
                if (_ListUserTaskOfTask != value)
                {
                    _ListUserTaskOfTask = value; OnPropertyChanged("ListUserTaskOfTask");
                    _TrackChange = true;
                }
            }
        }
        private ObservableCollection<UserTask> _ListUserTaskOfUser;
        public ObservableCollection<UserTask> ListUserTaskOfUser
        {
            get => _ListUserTaskOfUser;
            set
            {
                if (_ListUserTaskOfUser != value)
                {
                    _ListUserTaskOfUser = value; OnPropertyChanged("ListUserTaskOfUser");
                }
            }
        }
        #endregion

        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand UserTaskSelectedCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand FinishUserTaskCommand { get; set; }
        public ICommand SavePermissionCommand { get; set; }
        public ICommand OpenReceiveDepartmentManagerCommand { get; set; }
        public ICommand OpenReceiveUserManagerCommand { get; set; }
        public ICommand RevokeTaskCommand { get; set; }
        public ICommand AddUserOfMyDepartmentToTaskCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }
        #endregion

        public ListNewDocumentViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<QLHS_DR.ViewModel.Message.ReloadNewTasksTabEvent>().Subscribe(OnLoadUserControl);
            _TrackChange = false;
            IsExpander = false;
            IsReadOnlyPermission = !SectionLogin.Ins.Permissions.HasFlag(PermissionType.CHANGE_PERMISSION);
            MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            try
            {
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
                UserTaskSelecteds = new ObservableCollection<UserTask>();
                IsReadOnlyPermission = !SectionLogin.Ins.Permissions.HasFlag(PermissionType.CHANGE_PERMISSION);
                ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                OnLoadUserControl(new object());
            });
            OpenReceiveDepartmentManagerCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null && SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskAddDepartmentToTask")) return true; else return false; }, (p) =>
            {
                try
                {
                    ReceiveDepartmentManagerWD receiveDepartmentManagerWD = new ReceiveDepartmentManagerWD();
                    ReceiveDepartmentManagerViewModel receiveDepartmentManagerViewModel = new ReceiveDepartmentManagerViewModel(_UserTaskSelected);
                    receiveDepartmentManagerViewModel.WindowTitle = _UserTaskSelected.Task.Subject;
                    receiveDepartmentManagerWD.DataContext = receiveDepartmentManagerViewModel;
                    receiveDepartmentManagerWD.ShowDialog();
                    receiveDepartmentManagerViewModel.Dispose();
                    ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                    UpdateHeaderTabControl();
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
            OpenReceiveUserManagerCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskAddUserToTask") && _UserTaskSelected != null) return true; else return false; }, (p) =>
            {
                ReceiveUserManagerWD receiveUserManagerWD = new ReceiveUserManagerWD();
                ReceiveUserManagerViewModel receiveUserManagerViewModel = new ReceiveUserManagerViewModel(_UserTaskSelected.Task);
                receiveUserManagerWD.DataContext = receiveUserManagerViewModel;
                receiveUserManagerWD.ShowDialog();
                ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                UpdateHeaderTabControl();
            });
            AddUserOfMyDepartmentToTaskCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskAddUserOfMyDepartment") && _UserTaskSelected != null) return true; else return false; }, (p) =>
            {
                ReceiveUserOfMyDepartmentManagerWD receiveUserOfMyDepartmentManagerWD = new ReceiveUserOfMyDepartmentManagerWD();
                ReceiveUserOfMyDepartmentManagerViewModel receiveUserOfMyDepartmentManagerViewModel = new ReceiveUserOfMyDepartmentManagerViewModel(_UserTaskSelected.Task);
                receiveUserOfMyDepartmentManagerWD.DataContext = receiveUserOfMyDepartmentManagerViewModel;
                receiveUserOfMyDepartmentManagerWD.ShowDialog();
                ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                UpdateHeaderTabControl();
            });
            SavePermissionCommand = new RelayCommand<Object>((p) => { if (_IsReadOnlyPermission != true && _TrackChange == true) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();

                    if (_MyClient.UpdateUserTasks(_ListUserTaskOfTask.ToArray(), SectionLogin.Ins.CurrentUser.Id))
                    {
                        System.Windows.MessageBox.Show("Cập nhật thành công");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Cập nhật thất bại");
                    }
                    _MyClient.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + " - SavePermissionCommand");
                    if (ex.InnerException != null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.StackTrace);
                    }
                    _MyClient.Abort();
                }
            });
            EditTaskCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskEditTask") && _UserTaskSelected != null) return true; else return false; }, (p) =>
            {
                EditTaskWindow editTaskWindow = new EditTaskWindow();
                //ReceiveUserOfMyDepartmentManagerViewModel receiveUserOfMyDepartmentManagerViewModel = new ReceiveUserOfMyDepartmentManagerViewModel(_UserTaskSelected.Task);
                //editTaskWindow.DataContext = receiveUserOfMyDepartmentManagerViewModel;
                editTaskWindow.ShowDialog();
                ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                UpdateHeaderTabControl();
            });
            UserTaskSelectedCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null && _IsExpander) return true; else return false; }, (p) =>
            {
                ListUserTaskOfTask = new ObservableCollection<UserTask>();
                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                try
                {
                    _MyClient.Open();
                    var temp = _MyClient.GetAllUserTaskOfTask(_UserTaskSelected.TaskId).ToObservableCollection();
                    UsersInTask = _MyClient.GetUserInTask(_UserTaskSelected.TaskId).ToObservableCollection();

                    _MyClient.Close();
                    foreach (var userTask in temp)
                    {
                        userTask.User = _UsersInTask.Where(x => x.Id == userTask.UserId).FirstOrDefault();
                        userTask.User.UserDepartments = _UserDepartments.Where(x => x.UserId == userTask.User.Id).ToArray();
                        userTask.PropertyChanged += OnItemPropertyChanged;
                    }
                    _TrackChange = false;
                    ListUserTaskOfTask = temp;
                    _ListUserTaskOfTaskOrigin = new ObservableCollection<UserTask>(_ListUserTaskOfTask);
                }
                catch (Exception ex)
                {
                    _MyClient.Abort();
                    System.Windows.MessageBox.Show(ex.Message + " Function: UserTaskSelectedCommand");
                    if (ex.InnerException != null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.StackTrace);
                    }
                }
            });
            OpenFileCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null) return true; else return false; }, (p) =>
            {
                OpenFilePdf();
                //Thread thread5 = new Thread(new ThreadStart(OpenFilePdf));
                //thread5.SetApartmentState(ApartmentState.STA);
                //thread5.IsBackground = true;
                //thread5.Start();
            });
            FinishUserTaskCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelecteds.Count() > 0) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    foreach (var item in _UserTaskSelecteds)
                    {
                        _MyClient.SetUserTaskFinish(item.TaskId, SectionLogin.Ins.CurrentUser.Id, true);
                        //ListUserTaskOfUser.Remove(item);
                    }
                    _MyClient.Close();
                    ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                    UpdateHeaderTabControl();
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
            RevokeTaskCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null && (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskRevokeTask") || (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "taskRevokeTaskByOwner") && _UserTaskSelected.Task.OwnerUserId == SectionLogin.Ins.CurrentUser.Id))) return true; else return false; }, (p) =>
            {
                try
                {
                    MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Bạn có muốn thu hồi tài liệu này? Thao tác này sẽ xóa toàn bộ dữ liệu liên quan", "Cảnh báo !", MessageBoxButton.OKCancel);
                    if (dialogResult == MessageBoxResult.OK)
                    {
                        _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                        _MyClient.Open();

                        _MyClient.RevokeTaskByCurrentUser(_UserTaskSelected.TaskId);
                        _MyClient.Close();
                        ListUserTaskOfUser.Remove(_UserTaskSelected);
                        UpdateHeaderTabControl();
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
        }
        private void OpenFilePdf()
        {
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                PermissionType taskPermissions = _MyClient.GetTaskPermissions(SectionLogin.Ins.CurrentUser.Id, _UserTaskSelected.TaskId);
                bool signable = SectionLogin.Ins.Permissions.HasFlag(PermissionType.REVIEW_DOCUMENT | PermissionType.SIGN_DOCUMENT);
                bool printable = signable | taskPermissions.HasFlag(PermissionType.PRINT_DOCUMENT) | (_UserTaskSelected.Task.OwnerUserId == SectionLogin.Ins.CurrentUser.Id);
                bool saveable = _UserTaskSelected.CanSave.HasValue ? _UserTaskSelected.CanSave.Value : false;
                //if (_UserTaskSelected.CanViewAttachedFile == true)
                if (true)
                {
                    var taskAttachedFileDTOs = _MyClient.GetTaskDocuments(_UserTaskSelected.TaskId); //get all file PDF in task
                    if (taskAttachedFileDTOs != null && taskAttachedFileDTOs.Length > 0)
                    {
                       // TaskAttackFileViewerViewModel1 taskAttackFileViewerViewModel = new TaskAttackFileViewerViewModel1(taskAttachedFileDTOs[0], printable, saveable,  _UserTaskSelected);

                        TaskAttackFileViewerViewModel taskAttackFileViewerViewModel = new TaskAttackFileViewerViewModel(taskAttachedFileDTOs[0], printable, saveable, _UserTaskSelected);

                        taskAttackFileViewerViewModel.FileName = taskAttachedFileDTOs[0].FileName;
                        taskAttackFileViewerViewModel.TaskName = _UserTaskSelected.Task.Subject;

                        TaskAttackFileViewerWindow taskAttackFileViewerWindow = new TaskAttackFileViewerWindow();
                        taskAttackFileViewerWindow.DataContext = taskAttackFileViewerViewModel;
                        taskAttackFileViewerWindow.ShowDialog();
                        
                        //System.Windows.Threading.Dispatcher.Run();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy file đính kèm");
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa có quyền xem tài liệu này, vui lòng liên hệ quản trị viên!");
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

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _TrackChange = true;
        }
        private void OnLoadUserControl(object obj)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                UpdateHeaderTabControl();
            }));
        }
        private void UpdateHeaderTabControl()
        {
            var titletabControl = new TitletabControlMessage() { Title = "Tài liệu chưa xử lý ( " + _ListUserTaskOfUser.Count() + " )" };
            _eventAggregator.GetEvent<NewTasksTabTitleChangedEvent>().Publish(titletabControl);
        }
        public ObservableCollection<Task> GetAllTaskNotFinishOfUser(int userId)
        {
            ObservableCollection<Task> ketqua = new ObservableCollection<Task>();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                ketqua = _MyClient.LoadTasksNotFinish(userId).OrderByDescending(x => x.StartDate).ToObservableCollection();
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
        public ObservableCollection<UserTask> GetAllUserTaskNotFinishOfUser(int userId)
        {
            ObservableCollection<UserTask> ketqua = new ObservableCollection<UserTask>();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();

                ketqua = _MyClient.GetUserTaskNotFinish(userId).OrderByDescending(x => x.TimeCreate).ToObservableCollection();
                var tasks = _MyClient.LoadTasksNotFinish(userId);
                _MyClient.Close();
                foreach (var usertask in ketqua)
                {
                    usertask.Task = tasks.Where(x => x.Id == usertask.TaskId).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + " - Function: GetAllUserTaskNotFinishOfUser");
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
