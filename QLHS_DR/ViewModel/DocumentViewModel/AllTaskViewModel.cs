﻿using DevExpress.Mvvm;
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
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class AllTaskViewModel : BaseViewModel
    {
        #region "Properties and Field"
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
        private EofficeMainServiceClient _MyClient;
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
        public ICommand OpenReceiveUserManagerCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand TaskSelectedCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        #endregion
        public void SetLabelMsg(string message)
        {
            ListTaskOfUser = GetAllTask(SectionLogin.Ins.CurrentUser.Id);
        }
        public AllTaskViewModel()
        {
            //MessageServiceCallBack.SetDelegate(SetLabelMsg);
            IsReadOnlyPermission =  true;
            MainViewModel dataOfMainWindow = new MainViewModel();
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
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
                FrameworkElement window = System.Windows.Window.GetWindow(p);
                dataOfMainWindow = (MainViewModel)window.DataContext;
                IsReadOnlyPermission = !SectionLogin.Ins.Permissions.HasFlag(PermissionType.CHANGE_PERMISSION);
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ListTaskOfUser = GetAllTask(SectionLogin.Ins.CurrentUser.Id);
                });

                var tabNotFinnish = dataOfMainWindow.Workspaces.Where(x => x.Header.Contains("All")).FirstOrDefault();
                if (tabNotFinnish != null)
                {
                    tabNotFinnish.Header = "All ( " + _ListTaskOfUser.Count() + " )";
                }
            });
            OpenFileCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null) return true; else return false; }, (p) =>
            {                
                Thread thread5 = new Thread(new ThreadStart(OpenFilePdf));
                thread5.SetApartmentState(ApartmentState.STA);
                thread5.IsBackground = true;
                thread5.Start();
            });
            OpenReceiveUserManagerCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.Permissions.HasFlag(PermissionType.ADD_USER_TO_TASK) && _TaskSelected != null) return true; else return false; }, (p) =>
            {
                ReceiveUserManagerWD receiveUserManagerWD = new ReceiveUserManagerWD();
                ReceiveUserManagerViewModel receiveUserManagerViewModel = new ReceiveUserManagerViewModel(_TaskSelected);
                receiveUserManagerWD.DataContext = receiveUserManagerViewModel;
                receiveUserManagerWD.ShowDialog();
                ListTaskOfUser = GetAllTask(SectionLogin.Ins.CurrentUser.Id);
            });
            TaskSelectedCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null) return true; else return false; }, (p) =>
            {
                ListUserTaskOfTask = new ObservableCollection<UserTask>();
                try
                {
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
                }
            }); 
        }
        private void OpenFilePdf()
        {
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();

                _UserTaskSelected = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, _TaskSelected.Id);
                _UserTaskSelected.Task = _TaskSelected;
                if (_UserTaskSelected != null)
                {
                    PermissionType taskPermissions = _MyClient.GetTaskPermissions(SectionLogin.Ins.CurrentUser.Id, _UserTaskSelected.TaskId);
                    bool signable = SectionLogin.Ins.Permissions.HasFlag(PermissionType.REVIEW_DOCUMENT | PermissionType.SIGN_DOCUMENT);
                    bool printable = signable | taskPermissions.HasFlag(PermissionType.PRINT_DOCUMENT) | (_TaskSelected.OwnerUserId == SectionLogin.Ins.CurrentUser.Id);
                    bool saveable = _UserTaskSelected.CanSave.HasValue ? _UserTaskSelected.CanSave.Value : false;

                    var taskAttachedFileDTOs = _MyClient.GetTaskDocuments(_UserTaskSelected.TaskId); //get all file PDF in task
                    if (taskAttachedFileDTOs != null && taskAttachedFileDTOs.Length > 0)
                    {                      
                        PdfViewerWindow pdfViewer = new PdfViewerWindow(taskAttachedFileDTOs[0], printable, saveable,iReadOnlyListUser, _UserTaskSelected);                       
                        pdfViewer.Show();
                        System.Windows.Threading.Dispatcher.Run();
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
        public void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO,UserTask userTask)
        {
            FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            byte[] orAdd = _ListFileDecrypted.GetOrAdd(taskAttachedFileDTO.TaskId, (int int_0) => fileHelper.GetKeyDecryptOfTask(taskAttachedFileDTO.TaskId,userTask));
            if (orAdd != null)
            {
                taskAttachedFileDTO.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO.Content);
            }
        }
        public ObservableCollection<Task> GetAllTask(int userId)
        {
            ObservableCollection<Task> ketqua = new ObservableCollection<Task>();
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                ketqua = _MyClient.LoadAllTasks().OrderByDescending(x => x.StartDate).ToObservableCollection();                
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
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
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
