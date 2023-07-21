using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using Prism.Events;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.ViewModel.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class UserTaskRevokedViewModel : BaseViewModel
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
        public ICommand TaskSelectedCommand { get; set; }

        #endregion
        //public void SetLabelMsg(string message)
        //{
        //    ListTaskOfUser = GetAllTaskRevokedOfUser(SectionLogin.Ins.CurrentUser.Id);
        //}

        public UserTaskRevokedViewModel(IEventAggregator eventAggregator)
        {
            //MessageServiceCallBack.SetDelegate(SetLabelMsg);
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ReloadRevokedTasksTabEvent>().Subscribe(OnLoadUserControl);
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
                System.Windows.MessageBox.Show(ex.Message);
                _MyClient.Abort();
            }

            UsersInTask = new ObservableCollection<User>();
            LoadedWindowCommand = new RelayCommand<DependencyObject>((p) => { return true; }, (p) =>
            {
                IsReadOnlyPermission = !SectionLogin.Ins.Permissions.HasFlag(PermissionType.CHANGE_PERMISSION);
                OnLoadUserControl(new object());
            });
            TaskSelectedCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null && _IsExpander) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    UserTaskSelected = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, _TaskSelected.Id);
                    _MyClient.Close();
                    UserTaskSelected.Task = _TaskSelected;
                    ListUserTaskOfTask = GetUserTasksOfTask(_TaskSelected.Id);
                    _ListUserTaskOfTaskOrigin = new ObservableCollection<UserTask>(_ListUserTaskOfTask);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    if (ex.InnerException != null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.StackTrace);
                    }
                    _MyClient.Abort();
                }
            });
        }

        public ObservableCollection<UserTask> GetUserTasksOfTask(int taskId)
        {
            _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            _MyClient.Open();
            UsersInTask = _MyClient.GetUserInTask(taskId).ToObservableCollection();
            var temp = _MyClient.GetAllUserTaskOfTask(_TaskSelected.Id).ToObservableCollection();
            _MyClient.Close();
            foreach (var ut in temp)
            {
                ut.User = _UsersInTask.Where(x => x.Id == ut.UserId).FirstOrDefault();
                ut.User.UserDepartments = _UserDepartments.Where(x => x.UserId == ut.User.Id).ToArray();
            }
            return temp;
        }
        public ObservableCollection<Task> GetAllTaskRevokedOfUser(int userId)
        {
            ObservableCollection<Task> ketqua = new ObservableCollection<Task>();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                ketqua = _MyClient.LoadTasksRevoked(userId).OrderByDescending(x => x.StartDate).ToObservableCollection();
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
        public void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO, UserTask userTask)
        {
            FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);

            byte[] orAdd = _ListFileDecrypted.GetOrAdd(taskAttachedFileDTO.TaskId, (int int_0) => fileHelper.GetKeyDecryptOfTask(userTask));
            if (orAdd != null)
            {
                taskAttachedFileDTO.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO.Content);
            }
        }
        private void OnLoadUserControl(object obj)
        {
            ListTaskOfUser = GetAllTaskRevokedOfUser(SectionLogin.Ins.CurrentUser.Id);
            UpdateHeaderTabControl();
        }
        private void UpdateHeaderTabControl()
        {
            var titletabControl = new TitletabControlMessage() { Title = "Tài liệu đã thu hồi ( " + _ListTaskOfUser.Count() + " )" };
            _eventAggregator.GetEvent<RevokedTasksTabTitleChangedEvent>().Publish(titletabControl);
        }
    }
}
