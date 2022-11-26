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

namespace QLHS_DR.ViewModel
{
    class MainViewModel:BaseViewModel
    {
        #region "Field and properties"
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
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand LoadListNewDocument { get; set; }
        public ICommand OpenChangePassWordWindowCommand { get; set; }
        #endregion
        public MainViewModel()
        {
            Workspaces = new ObservableCollection<TabContainer>();

            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
              
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
                            SectionLogin.Ins.CurrentUser = loginVM.User;
                            CurrentUser = loginVM.User;
                            //Group currentGroup = SectionLogin.Ins.CurrentUser.Groups.FirstOrDefault();
                            //if (currentGroup != null) SectionLogin.Ins.CurrentPermission.GetPermissionByGroup(currentGroup.GroupId);
                            //else SectionLogin.Ins.CurrentPermission.GetDefaultPermission();
                        }
                        p.Show();
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
                ListNewDocumentUC listNewDocumentWindow = new ListNewDocumentUC();
                Workspaces.Clear();

                TabContainer tabItemMain = new TabContainer
                {
                    Header = "Tài liệu chưa xử lý",
                    AllowHide = "true",
                    IsSelected = true,
                    IsVisible = true,
                    Content = listNewDocumentWindow
                };
                Workspaces.Add(tabItemMain);

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
