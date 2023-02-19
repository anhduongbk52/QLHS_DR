using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR;
using QLHS_DR.Core;
using QLHS_DR.EOfficeServiceReference;
using QLHS_DR.View.DocumentView;
using QLHS_DR.ViewModel;
using QLHS_DR.ViewModel.ChatAppViewModel;
using QLHS_DR.ViewModel.DocumentViewModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace EofficeClient.ViewModel.DocumentViewModel
{

    class ListNewDocumentViewModel : BaseViewModel
    {     
        #region "Properties and Field"
        private EofficeMainServiceClient _MyClient;
        MainViewModel dataOfMainWindow;
        private IReadOnlyList<User> iReadOnlyListUser;
        private ConcurrentDictionary<int, byte[]> _ListFileDecrypted = new ConcurrentDictionary<int, byte[]>();
        private bool _TrackChange;
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
        public ICommand AddReceiveDepartmentCommand { get; set; }
        public ICommand OpenReceiveUserManagerCommand { get; set; }
        public ICommand RevokeTaskCommand { get; set; }
        #endregion
        public void SetLabelMsg(string message)
        {
            ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
        }        
        public ListNewDocumentViewModel() 
        {
            _TrackChange = false;
            IsReadOnlyPermission = !SectionLogin.Ins.Permissions.HasFlag(PermissionType.CHANGE_PERMISSION);
            MessageServiceCallBack.SetDelegate(SetLabelMsg);
            
            dataOfMainWindow = new MainViewModel();
            
            try 
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                iReadOnlyListUser = _MyClient.GetUserContacts(SectionLogin.Ins.CurrentUser.UserName);
                _MyClient.Close();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                _MyClient.Abort();
            }
            UsersInTask = new ObservableCollection<User>();
            LoadedWindowCommand = new RelayCommand<DependencyObject>((p) => { return true; }, (p) =>
            {
                FrameworkElement window = System.Windows.Window.GetWindow(p);
                dataOfMainWindow = (MainViewModel)window.DataContext;
                IsReadOnlyPermission = !SectionLogin.Ins.Permissions.HasFlag(PermissionType.CHANGE_PERMISSION);
                ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                UpdateHeaderTabControl();
            });
            AddReceiveDepartmentCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.Permissions.HasFlag(PermissionType.ADD_USER_TO_TASK)) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                   
                    ReceiveDepartmentManagerWD receiveDepartmentManagerWD = new ReceiveDepartmentManagerWD();
                    ReceiveDepartmentManagerViewModel receiveDepartmentManagerViewModel = new ReceiveDepartmentManagerViewModel();
                    receiveDepartmentManagerWD.DataContext= receiveDepartmentManagerViewModel;
                    receiveDepartmentManagerWD.Show();
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
            OpenReceiveUserManagerCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.Permissions.HasFlag(PermissionType.ADD_USER_TO_TASK) && _UserTaskSelected!=null) return true; else return false; }, (p) =>
            {
                ReceiveUserManagerWD receiveUserManagerWD = new ReceiveUserManagerWD();
                ReceiveUserManagerViewModel receiveUserManagerViewModel = new ReceiveUserManagerViewModel(_UserTaskSelected.Task);
                receiveUserManagerWD.DataContext = receiveUserManagerViewModel;
                receiveUserManagerWD.ShowDialog();
                ListUserTaskOfUser = GetAllUserTaskNotFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                UpdateHeaderTabControl();
            });

            SavePermissionCommand = new RelayCommand<Object>((p) => { if (_IsReadOnlyPermission != true && _TrackChange==true) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
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
                    System.Windows.MessageBox.Show(ex.Message+ " - SavePermissionCommand");
                    if (ex.InnerException != null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.StackTrace);
                    }
                    _MyClient.Abort();
                }
            });
            UserTaskSelectedCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null) return true; else return false; }, (p) =>
            {                
                ListUserTaskOfTask = new ObservableCollection<UserTask>();
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    var temp = _MyClient.GetAllUserTaskOfTask(_UserTaskSelected.TaskId).ToObservableCollection();
                    UsersInTask = _MyClient.GetUserInTask(_UserTaskSelected.TaskId).ToObservableCollection();
                    _MyClient.Close();
                    foreach (var userTask in temp)
                    {
                        userTask.User = UsersInTask.Where(x=>x.Id== userTask.UserId).FirstOrDefault();
                        userTask.PropertyChanged += OnItemPropertyChanged;                       
                    }                   
                    _TrackChange = false;
                    _ListUserTaskOfTaskOrigin = new ObservableCollection<UserTask>(ListUserTaskOfTask);
                    ListUserTaskOfTask = temp;
                }
                catch (Exception ex) 
                {
                    System.Windows.MessageBox.Show(ex.Message+ " Function: UserTaskSelectedCommand");
                    if(ex.InnerException!= null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.StackTrace);
                    }
                    _MyClient.Abort();
                }
            });
            OpenFileCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
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
                                DecryptTaskAttachedFile(taskAttachedFileDTOs[0]);
                                PdfViewerWindow pdfViewer = new PdfViewerWindow(taskAttachedFileDTOs[0].Content, printable, saveable);
                                pdfViewer.FileName = taskAttachedFileDTOs[0].FileName;
                                pdfViewer.TaskName = _UserTaskSelected.Task.Subject;
                                pdfViewer.UserTaskPrint = _UserTaskSelected;
                                pdfViewer.Show();
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
                    });                   
                    
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
            FinishUserTaskCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    _MyClient.SetUserTaskFinish(_UserTaskSelected.TaskId, SectionLogin.Ins.CurrentUser.Id, true);
                    _MyClient.Close();
                    ListUserTaskOfUser.Remove(_UserTaskSelected);
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
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _TrackChange = true;
        }

        
        private void UpdateHeaderTabControl()
        {
            var tabNotFinnish = dataOfMainWindow.Workspaces.Where(x => x.Header.Contains("Tài liệu chưa xử lý")).FirstOrDefault();
            if (tabNotFinnish != null)
            {
                tabNotFinnish.Header = "Tài liệu chưa xử lý ( " + _ListUserTaskOfUser.Count() + " )";
            }
        }
        public ObservableCollection<Task> GetAllTaskNotFinishOfUser(int userId)
        {
            ObservableCollection<Task> ketqua = new ObservableCollection<Task>();
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                ketqua = _MyClient.LoadTasksNotFinish(userId).OrderByDescending(x => x.StartDate).ToObservableCollection();                
                _MyClient.Close();
            }
            catch(Exception ex)
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
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                ketqua = _MyClient.GetUserTaskNotFinish(userId).OrderByDescending(x => x.TimeCreate).ToObservableCollection();
                var tasks = _MyClient.LoadTasksNotFinish(userId);
                _MyClient.Close();
                foreach (var usertask in ketqua) 
                {
                    usertask.Task = tasks.Where(x=>x.Id==usertask.TaskId).FirstOrDefault();
                }                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message+ " - Function: GetAllUserTaskNotFinishOfUser");
                if (ex.InnerException != null)
                {
                    System.Windows.MessageBox.Show(ex.InnerException.Message);
                }
                _MyClient.Abort();
            }
            return ketqua;
        }
        public void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO) 
        {
            FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token, iReadOnlyListUser);
            byte[] orAdd = _ListFileDecrypted.GetOrAdd(taskAttachedFileDTO.TaskId, (int int_0) => fileHelper.GetKeyDecryptOfTask(taskAttachedFileDTO.TaskId));
            if (orAdd != null)
            {
                taskAttachedFileDTO.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO.Content);
            }
        }      
       
    }
}
