using DevExpress.Mvvm;
using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.Core;
using QLHS_DR.ServiceReference1;
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
     
        private IReadOnlyList<User> iReadOnlyListUser;
        private ConcurrentDictionary<int, byte[]> _ListFileDecrypted = new ConcurrentDictionary<int, byte[]>();

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
        #endregion



        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand TaskSelectedCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        #endregion
        public ListNewDocumentViewModel()
        {
            iReadOnlyListUser = ServiceProxy.Ins.GetUserContacts(SectionLogin.Ins.CurrentUser.UserName);

           


            ListTaskOfUser = new List<Task>();
            UsersInTask = new ObservableCollection<User>();
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ListTaskOfUser = GetAllTaskOfUser(SectionLogin.Ins.CurrentUser.Id);
            });
            TaskSelectedCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null) return true; else return false; }, (p) =>
            {
                UsersInTask = new ObservableCollection<User>();
                var taskusers = ServiceProxy.Ins.GetUserInTask(_TaskSelected.Id);
                foreach (var item in taskusers)
                {
                    UsersInTask.Add(item);
                }
            });
            OpenFileCommand = new RelayCommand<Object>((p) => { if (_TaskSelected != null) return true; else return false; }, (p) =>
            {
                PermissionType taskPermissions = ServiceProxy.Ins.GetTaskPermissions(SectionLogin.Ins.CurrentUser.Id,_TaskSelected.Id);

                bool signable = SectionLogin.Ins.Permissions.HasFlag(PermissionType.REVIEW_DOCUMENT | PermissionType.SIGN_DOCUMENT);
                bool printable = signable | taskPermissions.HasFlag(PermissionType.PRINT_DOCUMENT) | (_TaskSelected.OwnerUserId == SectionLogin.Ins.CurrentUser.Id);


                var taskAttachedFileDTOs = ServiceProxy.Ins.GetTaskDocuments(_TaskSelected.Id); //get all file PDF in task
                if(taskAttachedFileDTOs.Length==1)
                {
                    DecryptTaskAttachedFile(taskAttachedFileDTOs[0]);
                    PdfViewerWindow pdfViewer = new PdfViewerWindow(taskAttachedFileDTOs[0].Content);
                    pdfViewer.Show();
                }           

              
            });
        }

      
        public ObservableCollection<User> GetAllUser()
        {
            ObservableCollection<User> ketqua = new ObservableCollection<User>();
            var Users = ServiceProxy.Ins.GetUserContacts("duongda");

            foreach (var user in Users)
            {
                ketqua.Add(user);

            }
            return ketqua;
        }
        public List<Task> GetAllTaskOfUser(int userId)
        {
            List<Task> ketqua = new List<Task>();
            var Tasks = ServiceProxy.Ins.LoadTasks(userId);

            foreach (var taks in Tasks)
            {
                ketqua.Add(taks);

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
            byte[] password = Convert.FromBase64String(ServiceProxy.Ins.ChannelFactory.Endpoint.Behaviors.Find<ClientCredentials>().UserName.Password);
            return CryptoUtil.HashPassword(CryptoUtil.GetKeyFromPassword(password), CryptoUtil.GetSaltFromPassword(password));
        }
        private byte[] GetKeyDecryptOfTask(int taskId)
        {
            UserTask userTask_0 = ServiceProxy.Ins.GetUserTask(SectionLogin.Ins.CurrentUser.Id, taskId);
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
        private byte[] method_20(byte[] byte_0)
        {
            User user = SectionLogin.Ins.CurrentUser ?? (SectionLogin.Ins.CurrentUser = ServiceProxy.Ins.GetUserByName(ServiceProxy.Ins.ClientCredentials.UserName.UserName));
            if (user.ECPrKeyForFile == null)
            {
                user.ECPrKeyForFile = ServiceProxy.Ins.GetUserECPrKeyFor(user.Id, ServiceProxy.Ins.ChannelFactory.Endpoint.Behaviors.Find<ClientCredentials>().UserName.Password, ECKeyPurpose.FILE);
            }
            return CryptoUtil.DecryptByDerivedPassword(byte_0, user.ECPrKeyForFile);
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
