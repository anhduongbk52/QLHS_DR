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

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class UserTaskFinishViewModel : BaseViewModel
    {
        #region "Properties and Field"
        private EofficeMainServiceClient _MyClient;
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
        public ICommand TaskSelectedCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand FinishTaskCommand { get; set; }
        public ICommand SavePermissionCommand { get; set; }
        #endregion
        public void SetLabelMsg(string message)
        {
            ListTaskOfUser = GetAllTaskFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
        }

        public UserTaskFinishViewModel()
        {
            _TrackChange = false;
            IsReadOnlyPermission = !SectionLogin.Ins.Permissions.HasFlag(PermissionType.CHANGE_PERMISSION);
            MessageServiceCallBack.SetDelegate(SetLabelMsg);

            MainViewModel dataOfMainWindow = new MainViewModel();

            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                iReadOnlyListUser = _MyClient.GetUserContacts(SectionLogin.Ins.CurrentUser.UserName);
                _MyClient.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                _MyClient.Abort();
            }

            //ListTaskOfUser = new List<Task>();
            UsersInTask = new ObservableCollection<User>();
            LoadedWindowCommand = new RelayCommand<DependencyObject>((p) => { return true; }, (p) =>
            {
                FrameworkElement window = System.Windows.Window.GetWindow(p);
                dataOfMainWindow = (MainViewModel)window.DataContext;
                IsReadOnlyPermission = !SectionLogin.Ins.Permissions.HasFlag(PermissionType.CHANGE_PERMISSION);
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ListTaskOfUser = GetAllTaskFinishOfUser(SectionLogin.Ins.CurrentUser.Id);
                });

                var tabNotFinnish = dataOfMainWindow.Workspaces.Where(x => x.Header.Contains("Tài liệu đã xử lý")).FirstOrDefault();
                if (tabNotFinnish != null)
                {
                    tabNotFinnish.Header = "Tài liệu đã xử lý ( " + _ListTaskOfUser.Count() + " )";
                }
            });
            SavePermissionCommand = new RelayCommand<Object>((p) => { if (_IsReadOnlyPermission != true && _TrackChange == true) return true; else return false; }, (p) =>
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
                    System.Windows.MessageBox.Show(ex.Message + " - SavePermissionCommand");
                    if (ex.InnerException != null)
                    {
                        System.Windows.MessageBox.Show(ex.InnerException.StackTrace);
                    }
                    _MyClient.Abort();
                }
            });
            TaskSelectedCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null) return true; else return false; }, (p) =>
            {
                ListUserTaskOfTask = new ObservableCollection<UserTask>();
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    UserTaskSelected = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, _TaskSelected.Id);
                    UserTaskSelected.Task = _TaskSelected;
                    UsersInTask = _MyClient.GetUserInTask(_TaskSelected.Id).ToObservableCollection();
                    foreach (var user in _UsersInTask)
                    {
                        UserTask userTask = _MyClient.GetUserTask(user.Id, _TaskSelected.Id);
                        if (userTask != null)
                        {
                            userTask.User = user;
                            userTask.PropertyChanged += OnItemPropertyChanged;
                            ListUserTaskOfTask.Add(userTask);
                        }
                    }
                    _MyClient.Close();
                    _TrackChange = false;
                    _ListUserTaskOfTaskOrigin = new ObservableCollection<UserTask>(ListUserTaskOfTask);
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
            OpenFileCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        PermissionType taskPermissions = _MyClient.GetTaskPermissions(SectionLogin.Ins.CurrentUser.Id, _TaskSelected.Id);
                        bool signable = SectionLogin.Ins.Permissions.HasFlag(PermissionType.REVIEW_DOCUMENT | PermissionType.SIGN_DOCUMENT);
                        bool printable = signable | taskPermissions.HasFlag(PermissionType.PRINT_DOCUMENT) | (_TaskSelected.OwnerUserId == SectionLogin.Ins.CurrentUser.Id);
                        bool saveable = _UserTaskSelected.CanSave.HasValue ? _UserTaskSelected.CanSave.Value : false;
                        if (_UserTaskSelected.CanViewAttachedFile == true)
                        {
                            var taskAttachedFileDTOs = _MyClient.GetTaskDocuments(_TaskSelected.Id); //get all file PDF in task
                            if (taskAttachedFileDTOs.Length == 1)
                            {
                                DecryptTaskAttachedFile(taskAttachedFileDTOs[0]);
                                PdfViewerWindow pdfViewer = new PdfViewerWindow(taskAttachedFileDTOs[0].Content, printable, saveable);
                                pdfViewer.FileName = taskAttachedFileDTOs[0].FileName;
                                pdfViewer.TaskName = _TaskSelected.Subject;
                                pdfViewer.UserTaskPrint = _UserTaskSelected;
                                pdfViewer.Show();
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
            FinishTaskCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    _MyClient.SetUserTaskFinish(_TaskSelected.Id, SectionLogin.Ins.CurrentUser.Id);
                    _MyClient.Close();
                    ListTaskOfUser.Remove(_TaskSelected);
                    var tabNotFinnish = dataOfMainWindow.Workspaces.Where(x => x.Header.Contains("Tài liệu chưa xử lý")).FirstOrDefault();
                    if (tabNotFinnish != null)
                    {
                        tabNotFinnish.Header = "Tài liệu đã xử lý ( " + _ListTaskOfUser.Count() + " )";
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

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _TrackChange = true;
        }

        //public ObservableCollection<User> GetAllUser()
        //{
        //    ObservableCollection<User> ketqua = new ObservableCollection<User>();
        //    try
        //    {
        //        _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
        //        _MyClient.Open();
        //        var Users = _MyClient.GetUserContacts("duongda");
        //        _MyClient.Close();
        //        foreach (var user in Users)
        //        {
        //            ketqua.Add(user);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show(ex.Message);
        //        if (ex.InnerException != null)
        //        {
        //            System.Windows.MessageBox.Show(ex.InnerException.Message);
        //        }
        //        _MyClient.Abort();
        //    }
        //    return ketqua;
        //}

        public ObservableCollection<Task> GetAllTaskFinishOfUser(int userId)
        {
            ObservableCollection<Task> ketqua = new ObservableCollection<Task>();
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                ketqua = _MyClient.LoadTasksFinish(userId).OrderByDescending(x => x.StartDate).ToObservableCollection();
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
