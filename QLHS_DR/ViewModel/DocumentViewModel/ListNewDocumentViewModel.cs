using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.Core;
using QLHS_DR.EOfficeServiceReference;
using QLHS_DR.View.DocumentView;
using QLHS_DR.ViewModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace EofficeClient.ViewModel.DocumentViewModel
{

    class ListNewDocumentViewModel : BaseViewModel
    {
        //        
        #region "Properties and Field"
        private EofficeMainServiceClient _MyClient;
        private IReadOnlyList<User> iReadOnlyListUser;
        private ConcurrentDictionary<int, byte[]> _ListFileDecrypted = new ConcurrentDictionary<int, byte[]>();

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
        private List<Task> _ListTaskOfUser;
        public List<Task> ListTaskOfUser
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
        #endregion
        public ListNewDocumentViewModel(ObservableCollection<UserTask> userTask) 
        {
            if(userTask!=null)
            {
                ListUserTaskOfUser = userTask;
            }
            try 
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                iReadOnlyListUser = _MyClient.GetUserContacts(SectionLogin.Ins.CurrentUser.UserName);
                _MyClient.Close();
            }
            catch(Exception ex) 
            { 
                MessageBox.Show(ex.Message);
                _MyClient.Abort();
            }    
            
            ListTaskOfUser = new List<Task>();
            UsersInTask = new ObservableCollection<User>();
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ListTaskOfUser = GetAllTaskOfUser(SectionLogin.Ins.CurrentUser.Id).OrderByDescending(x => x.StartDate).ToList();
            });
            TaskSelectedCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null) return true; else return false; }, (p) =>
            {                
                
                ListUserTaskOfTask = new ObservableCollection<UserTask>();
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    UsersInTask = _MyClient.GetUserInTask(_UserTaskSelected.TaskId).ToObservableCollection();
                    foreach (var user in _UsersInTask)
                    {
                        UserTask userTask = _MyClient.GetUserTask(user.Id, _UserTaskSelected.TaskId);

                        if (userTask != null)
                        {
                            userTask.User = user;
                            ListUserTaskOfTask.Add(userTask);
                        }
                    }
                    _MyClient.Close();
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message);
                    _MyClient.Abort();
                }
            });
            OpenFileCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    PermissionType taskPermissions = _MyClient.GetTaskPermissions(SectionLogin.Ins.CurrentUser.Id, _UserTaskSelected.TaskId);

                    bool signable = SectionLogin.Ins.Permissions.HasFlag(PermissionType.REVIEW_DOCUMENT | PermissionType.SIGN_DOCUMENT);
                    bool printable = signable | taskPermissions.HasFlag(PermissionType.PRINT_DOCUMENT) | (_UserTaskSelected.Task.OwnerUserId == SectionLogin.Ins.CurrentUser.Id);
                    bool saveable =_UserTaskSelected.CanSave.HasValue ? _UserTaskSelected.CanSave.Value : false;

                    var taskAttachedFileDTOs = _MyClient.GetTaskDocuments(_UserTaskSelected.TaskId); //get all file PDF in task
                    _MyClient.SetSeenUserInTask(_UserTaskSelected.TaskId, SectionLogin.Ins.CurrentUser.Id);
                    _MyClient.Close();
                    if (taskAttachedFileDTOs.Length == 1)
                    {
                        DecryptTaskAttachedFile(taskAttachedFileDTOs[0]);
                        PdfViewerWindow pdfViewer = new PdfViewerWindow(taskAttachedFileDTOs[0].Content, printable,saveable);
                        pdfViewer.FileName = taskAttachedFileDTOs[0].FileName;
                        pdfViewer.TaskName = _UserTaskSelected.Task.Subject;
                        pdfViewer.UserTaskPrint = _UserTaskSelected;
                        pdfViewer.Show();
                    }
                }
                catch(Exception ex)
                { MessageBox.Show(ex.Message); }
                
            });
            FinishTaskCommand = new RelayCommand<Object>((p) => { if (_UserTaskSelected != null && _UserTaskSelected.IsFinish!=true) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    _MyClient.SetUserTaskFinish(_UserTaskSelected.Id);
                    ListUserTaskOfUser.Remove(_UserTaskSelected);
                    _MyClient.Close();
                    
                }
                catch (Exception ex)
                { MessageBox.Show(ex.Message); }

            });
        }      
        public ObservableCollection<User> GetAllUser()
        {
            ObservableCollection<User> ketqua = new ObservableCollection<User>();
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();                
                var Users = _MyClient.GetUserContacts("duongda");
                _MyClient.Close();
                foreach (var user in Users)
                {
                    ketqua.Add(user);
                }                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                _MyClient.Abort();
            }
            return ketqua;
        }
        public List<Task> GetAllTaskOfUser(int userId)
        {
             List<Task> ketqua = new List<Task>();
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                var Tasks = _MyClient.LoadTasks(userId);
                _MyClient.Close();
                foreach (var taks in Tasks)
                {
                    ketqua.Add(taks);
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                _MyClient.Abort();
            }
            return ketqua;
        }
        public void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO) 
        {
            byte[] orAdd = _ListFileDecrypted.GetOrAdd(taskAttachedFileDTO.TaskId, (int int_0) => GetKeyDecryptOfTask(taskAttachedFileDTO.TaskId));
            if (orAdd != null)
            {
                taskAttachedFileDTO.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO.Content);
            }
        }
        private byte[] GetHashPasword()
        {
            byte[] password=null;
            byte[] ketqua = null;
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                password = Convert.FromBase64String(_MyClient.ChannelFactory.Endpoint.Behaviors.Find<ClientCredentials>().UserName.Password);
                _MyClient.Close();
                ketqua = CryptoUtil.HashPassword(CryptoUtil.GetKeyFromPassword(password), CryptoUtil.GetSaltFromPassword(password));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                _MyClient.Abort();
            }
            return ketqua;
        }
        private byte[] GetKeyDecryptOfTask(int taskId)
        {
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                UserTask userTask_0 = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, taskId);
                _MyClient.Close();
                if (userTask_0 != null)
                {
                    byte[] hassPasword = GetHashPasword();
                    if (userTask_0.AssignedBy == SectionLogin.Ins.CurrentUser.Id) //Nếu luồng công việc được tạo bởi _CurrentUser
                    {
                        return CryptoUtil.DecryptByDerivedPassword(hassPasword, userTask_0.TaskKey); //lấy key giải mã là của TaskKey của userTask
                    }
                    //Nếu luồng công việc không được tạo bởi _CurrentUser
                    User user = iReadOnlyListUser.FirstOrDefault((User u) => u.Id == userTask_0.AssignedBy); //Lấy về user chủ nhân của luồng.
                    if (user.ECPrKeyForFile == null)  //Nếu user không có ECPrKeyForFile
                    {
                        byte[] byte_ = method_20(hassPasword);
                        SetECPrKeyForFile(byte_, user);
                    }
                    return CryptoUtil.DecryptWithoutIV(user.ECPrKeyForFile, userTask_0.TaskKey);
                }
                return null;
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
                _MyClient.Abort();
            }
            return null;
        }
        private byte[] method_20(byte[] byte_0)
        {
            byte[] ketqua = null;
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                QLHS_DR.EOfficeServiceReference.User user = SectionLogin.Ins.CurrentUser ?? (SectionLogin.Ins.CurrentUser = _MyClient.GetUserByName(_MyClient.ClientCredentials.UserName.UserName));
                if (user.ECPrKeyForFile == null)
                {
                    user.ECPrKeyForFile = _MyClient.GetUserECPrKeyFor(user.Id, _MyClient.ChannelFactory.Endpoint.Behaviors.Find<ClientCredentials>().UserName.Password, ECKeyPurpose.FILE);
                }
               ketqua= CryptoUtil.DecryptByDerivedPassword(byte_0, user.ECPrKeyForFile);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                _MyClient.Abort();
            }
            return ketqua;
        }
        private void SetECPrKeyForFile(byte[] byte_0, User user_1)
        {
            if (user_1.ECPrKeyForFile == null)
            {
                user_1.ECPrKeyForFile = CryptoUtil.GetECSessionKey(byte_0, user_1.ECPuKeyForFile);
            }
        }
    }
}
