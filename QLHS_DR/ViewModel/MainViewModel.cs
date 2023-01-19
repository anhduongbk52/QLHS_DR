using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Deployment.Application;
using QLHS_DR.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.View;
using EofficeClient.ViewModel;
using QLHS_DR.View.DocumentView;
using EofficeClient.Core;
using QLHS_DR.ServiceReference1;
using DevExpress.Mvvm.Native;
using EofficeClient.ViewModel.DocumentViewModel;
using DevExpress.Pdf.Native.BouncyCastle.Utilities.Net;
using AutoUpdaterDotNET;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using QLHS_DR.Properties;

namespace QLHS_DR.ViewModel
{
    class MainViewModel:BaseViewModel
    {
        #region "Field and properties"
        private Window window;

        private ObservableCollection<TabContainer> _Workspaces;
        public ObservableCollection<TabContainer> Workspaces
        {
            get => _Workspaces;
            set
            {
                if (_Workspaces != value)
                {
                    _Workspaces = value;
                    NotifyPropertyChanged("Workspaces");
                }
            }
        }
        private ServiceReference1.User _CurrentUser;
        public ServiceReference1.User CurrentUser { get => _CurrentUser; set { _CurrentUser = value; OnPropertyChanged("CurrentUser"); } }

        public bool Isloaded = false;
        private string? _TileApplication;
        public string? TileApplication
        {
            get => _TileApplication;
            set
            {
                if (_TileApplication != value)
                {
                    _TileApplication = value;
                    NotifyPropertyChanged("TileApplication");
                }
            }
        }

        private string _ContentButtonNotCompleted;
        public string ContentButtonNotCompleted
        {
            get => _ContentButtonNotCompleted;
            set
            {
                if (_ContentButtonNotCompleted != value)
                {
                    _ContentButtonNotCompleted = value;
                    NotifyPropertyChanged("ContentButtonNotCompleted");
                }
            }
        }
        private string _ContentButtonCompleted;
        public string ContentButtonCompleted
        {
            get => _ContentButtonCompleted;
            set
            {
                if (_ContentButtonCompleted != value)
                {
                    _ContentButtonCompleted = value;
                    NotifyPropertyChanged("ContentButtonCompleted");
                }
            }
        }
        private string _ContentButtonRevoke;
        public string ContentButtonRevoke
        {
            get => _ContentButtonRevoke;
            set
            {
                if (_ContentButtonRevoke != value)
                {
                    _ContentButtonRevoke = value;
                    NotifyPropertyChanged("ContentButtonRevoke");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand LoadListNewDocument { get; set; }
        public ICommand LoadListCompltetedDocument { get; set; }
        public ICommand LoadListRevokeDocument { get; set; }
        public ICommand OpenChangePassWordWindowCommand { get; set; }
        #endregion
        public MainViewModel()
        {
            //Settings for update
            string addressUpdateInfo = Settings.Default.AddressUpdateInfo;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start(addressUpdateInfo);


            Workspaces = new ObservableCollection<TabContainer>();

            ObservableCollection<UserTask> _ListUserTaskIsProcessing = new ObservableCollection<UserTask>();
            ObservableCollection<UserTask> _ListUserTaskCompleted=new ObservableCollection<UserTask>();
            ObservableCollection<UserTask> _ListUserTaskRevoked=new ObservableCollection<UserTask>();
            ObservableCollection<UserTask>  _ListAllUserTask = new ObservableCollection<UserTask>();
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                window = p;
                //Load current version
                Version version = GetRunningVersion();
                TileApplication = "EEMC Office - " + version?.Major + "." + version?.Minor + "." + version?.Build + "." + version?.Revision;

                //Login process
                Isloaded = true;
                if (p == null) return;
                CertificateValidation();
                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                if (loginWindow.DataContext == null) return;
                var loginVM = loginWindow.DataContext as LoginViewModel;
                if (loginVM != null)
                {
                    if (loginVM.IsLogin)
                    {
                        if (loginVM.User != null)
                        {
                           
                            CurrentUser = loginVM.User;
                            //Group currentGroup = SectionLogin.Ins.CurrentUser.Groups.FirstOrDefault();
                            //if (currentGroup != null) SectionLogin.Ins.CurrentPermission.GetPermissionByGroup(currentGroup.GroupId);
                            //else SectionLogin.Ins.CurrentPermission.GetDefaultPermission();
                        }
                        
                        p.Show();

                        _ListUserTaskIsProcessing = GetAllUserTaskOfUser(CurrentUser.Id).Where(x => x.Task.Status != ServiceReference1.TaskStatus.Revoked && x.IsProcessing == false).OrderByDescending(x=>x.Task.StartDate).ToObservableCollection();
                        _ListUserTaskCompleted = GetAllUserTaskOfUser(CurrentUser.Id).Where(x => x.Task.Status != ServiceReference1.TaskStatus.Revoked && x.IsProcessing == true).OrderByDescending(x => x.Task.StartDate).ToObservableCollection();
                        _ListUserTaskRevoked = GetAllUserTaskOfUser(CurrentUser.Id).Where(x => x.Task.Status == ServiceReference1.TaskStatus.Revoked).OrderByDescending(x => x.Task.StartDate).ToObservableCollection();
                        //_ListAllUserTask = GetAllUserTaskOfUser(CurrentUser.Id).Where(x => x.Task.Status == ServiceReference1.TaskStatus.Revoked).OrderByDescending(x => x.Task.StartDate).ToObservableCollection();
                        Workspaces.Clear();

                        ListNewDocumentUC listNewDocumentUC = new ListNewDocumentUC();
                        ListNewDocumentUC listCompletedDocumentUC = new ListNewDocumentUC();
                        ListNewDocumentUC listRevokeDocumentUC = new ListNewDocumentUC();
                        ListNewDocumentUC listAllDocumentUC = new ListNewDocumentUC();

                        ListNewDocumentViewModel listNewDocumentViewModel = new ListNewDocumentViewModel(_ListUserTaskIsProcessing);                        
                        ListNewDocumentViewModel listCompletedDocumentViewModel = new ListNewDocumentViewModel(_ListUserTaskCompleted);
                        ListNewDocumentViewModel listRevokeDocumentViewModel = new ListNewDocumentViewModel(_ListUserTaskRevoked);
                        ListNewDocumentViewModel listAllDocumentViewModel = new ListNewDocumentViewModel(_ListUserTaskRevoked);

                        listNewDocumentUC.DataContext = listNewDocumentViewModel;
                        listCompletedDocumentUC.DataContext = listCompletedDocumentViewModel;
                        listRevokeDocumentUC.DataContext = listRevokeDocumentViewModel;
                        listAllDocumentUC.DataContext = listRevokeDocumentViewModel;

                        TabContainer tabItemNew = new TabContainer
                        {
                            Header = "Tài liệu chưa xử lý ( "+ _ListUserTaskIsProcessing.Count()+" )",
                            AllowHide = "true",
                            IsSelected = true,
                            IsVisible = true,
                            Content = listNewDocumentUC
                        };
                        TabContainer tabItemCompleted = new TabContainer
                        {
                            Header = "Tài liệu đã xử lý ( " + _ListUserTaskCompleted.Count() + " )",
                            AllowHide = "true",
                            IsSelected = false,
                            IsVisible = true,
                            Content = listCompletedDocumentUC
                        };
                        TabContainer tabItemRevoke = new TabContainer
                        {
                            Header = "Tài liệu đã thu hồi ( " + _ListUserTaskRevoked.Count() + " )",
                            AllowHide = "true",
                            IsSelected = false,
                            IsVisible = true,
                            Content = listRevokeDocumentUC
                        };
                        TabContainer tabItemAll = new TabContainer
                        {
                            Header = "All ( " + _ListUserTaskRevoked.Count() + " )",
                            AllowHide = "true",
                            IsSelected = false,
                            IsVisible = true,
                            Content = listAllDocumentUC
                        };

                        Workspaces.Add(tabItemNew);
                        Workspaces.Add(tabItemCompleted);
                        Workspaces.Add(tabItemRevoke);
                        Workspaces.Add(tabItemAll);

                    }
                    else
                    { p.Close(); }
                }
            });
            LogoutCommand = new RelayCommand<Window>((p) => { if (p == null) return false; else return true; }, (p) =>
            {              
                Isloaded = false;
                ConfigurationUtil.RemoveCreditalData(AppInfo.FolderPath);

                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                if (loginWindow.DataContext == null) return;
                var loginVM = loginWindow.DataContext as LoginViewModel;
                if (loginVM != null)
                {
                    if (loginVM.IsLogin)
                    {
                        if (loginVM.User != null)
                        {
                            SectionLogin.Ins.CurrentUser = loginVM.User;
                            CurrentUser = loginVM.User;
                        }
                        p.Show();
                    }
                    else
                    { p.Close(); }
                }
            });
            LoadListNewDocument = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                

            });
            LoadListCompltetedDocument = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
               
                

            });
            LoadListRevokeDocument = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
               

            });
            OpenChangePassWordWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                View.UserView.UserChangePasswordWindow userChangePasswordWindow = new View.UserView.UserChangePasswordWindow();
                ViewModel.UserViewModel.UserChangePasswordViewModel userChangePasswordViewModel = new UserViewModel.UserChangePasswordViewModel();
                userChangePasswordWindow.DataContext = userChangePasswordViewModel;
                userChangePasswordWindow.ShowDialog();
            });


        }
        #region "Function"
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.IsUpdateAvailable)
            {
                DialogResult dialogResult;
                dialogResult = System.Windows.Forms.MessageBox.Show($@"Bạn ơi, phần mềm của bạn có phiên bản mới {args.CurrentVersion}. Phiên bản bạn đang sử dụng hiện tại  {args.InstalledVersion}. Bạn có muốn cập nhật phần mềm không?", @"Cập nhật phần mềm",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
                {
                    try
                    {
                        if (AutoUpdater.DownloadUpdate(args))
                        {
                            window.Close();
                        }
                    }
                    catch (Exception exception)
                    {
                        System.Windows.Forms.MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            //else
            //{
            //    System.Windows.Forms.MessageBox.Show(@"Phiên bản bạn đang sử dụng đã được cập nhật mới nhất.", @"Cập nhật phần mềm",
            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }
        private Version GetRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
        private void CertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(IsSSL));
        }
        #endregion
        private static bool IsSSL(object A_0, X509Certificate A_1, X509Chain A_2, SslPolicyErrors A_3)
        {
            return true;
        }
        private ObservableCollection<UserTask> GetAllUserTaskOfUser(int userId)
        {
            ObservableCollection<UserTask> ListUserTaskOfUser =new ObservableCollection<UserTask>();
            ObservableCollection<ServiceReference1.Task> ListTaskOfUser = ServiceProxy.Ins.LoadTasks(userId).ToObservableCollection();
           
            foreach (var task in ListTaskOfUser)
            {
                UserTask userTask = ServiceProxy.Ins.GetUserTask(userId, task.Id);
                if(userTask != null)
                {
                    userTask.Task = task;
                    ListUserTaskOfUser.Add(userTask);
                }    
              
            }
            return ListUserTaskOfUser;



        }
    }
}
