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
using QLHS_DR.EOfficeServiceReference;
using DevExpress.Mvvm.Native;
using EofficeClient.ViewModel.DocumentViewModel;
using DevExpress.Pdf.Native.BouncyCastle.Utilities.Net;
using AutoUpdaterDotNET;
using System.Windows.Forms;
using QLHS_DR.Properties;
using TaskStatus = QLHS_DR.EOfficeServiceReference.TaskStatus;
using QLHS_DR.ViewModel.DocumentViewModel;

namespace QLHS_DR.ViewModel
{
    class MainViewModel:BaseViewModel
    {
        #region "Field and properties"
        private Window window;
        
        private EofficeMainServiceClient _MyClient;
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
       

        private User _CurrentUser;
        public User CurrentUser { get => _CurrentUser; set { _CurrentUser = value; OnPropertyChanged("CurrentUser"); } }

        public bool Isloaded = false;
        private string _TileApplication;
        public string TileApplication
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

        private ListNewDocumentUC listNewDocumentUC;
        private UserTaskFinishUC userTaskFinishUC;
        private UserTaskRevokedUC userTaskRevokedUC;
        private AllTaskUC allTaskUC;

        ListNewDocumentViewModel listNewDocumentViewModel;
        UserTaskFinishViewModel userTaskFinishViewModel;
        UserTaskRevokedViewModel userTaskRevokedViewModel;
        AllTaskViewModel allTaskViewModel; 
        public MainViewModel()
        {
            //Settings for update
            string addressUpdateInfo = Settings.Default.AddressUpdateInfo;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start(addressUpdateInfo);

            Workspaces = new ObservableCollection<TabContainer>();

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
                        }
                        
                        p.Show();
                        LoadDefaultTab();
                    }
                    else
                    { p.Close(); }
                }
            });
            LogoutCommand = new RelayCommand<Window>((p) => { if (p == null) return false; else return true; }, (p) =>
            {              
                Isloaded = false;
                ConfigurationUtil.RemoveCreditalData(AppInfo.FolderPath);                
                SectionLogin.Ins = null;
               

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
                        LoadDefaultTab();
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
        private void LoadDefaultTab()
        {
            listNewDocumentUC = new ListNewDocumentUC();
            userTaskFinishUC = new UserTaskFinishUC();
            userTaskRevokedUC = new UserTaskRevokedUC();
            allTaskUC = new AllTaskUC();

            listNewDocumentViewModel = new ListNewDocumentViewModel();
            userTaskFinishViewModel = new UserTaskFinishViewModel();
            userTaskRevokedViewModel = new UserTaskRevokedViewModel();
            allTaskViewModel = new AllTaskViewModel();

            Workspaces.Clear();

            listNewDocumentUC.DataContext = listNewDocumentViewModel;
            userTaskFinishUC.DataContext = userTaskFinishViewModel;
            userTaskRevokedUC.DataContext = userTaskRevokedViewModel;
            allTaskUC.DataContext = allTaskViewModel;

            TabContainer tabItemNew = new TabContainer
            {
                Header = "Tài liệu chưa xử lý",
                AllowHide = "true",
                IsSelected = true,
                IsVisible = true,
                Content = listNewDocumentUC
            };
            TabContainer tabItemCompleted = new TabContainer
            {
                Header = "Tài liệu đã xử lý",
                AllowHide = "true",
                IsSelected = false,
                IsVisible = true,
                Content = userTaskFinishUC
            };
            TabContainer tabItemRevoke = new TabContainer
            {
                Header = "Tài liệu đã thu hồi",
                AllowHide = "true",
                IsSelected = false,
                IsVisible = true,
                Content = userTaskRevokedUC
            };
            TabContainer tabItemAll = new TabContainer
            {
                Header = "All",
                AllowHide = "true",
                IsSelected = false,
                IsVisible = true,
                Content = allTaskUC
            };
            Workspaces.Add(tabItemNew);
            Workspaces.Add(tabItemCompleted);
            Workspaces.Add(tabItemRevoke);
            Workspaces.Add(tabItemAll);
        }
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
    }
}
