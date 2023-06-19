using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using EofficeCommonLibrary.Model.DataType;
using Microsoft.Win32;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.ViewModel;
using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EofficeClient.ViewModel
{

    class LoginViewModel : BaseViewModel, IDataErrorInfo
    {
        private MessageServiceClient _MyClient;

        public string Error { get { return null; } }
        public string this[string columnName]
        {
            get
            {
                string res = null;
                switch (columnName)
                {
                    case "UserName":
                        if (string.IsNullOrEmpty(UserName))
                        {
                            res = "UserName cannot be empty";
                        }

                        else if (UserName.Contains("@"))

                            res = "UserName cannot contains @";
                        break;
                }
                return res;
            }
        }
        public bool IsLogin { get; set; }
        private string _UserName;
        public string UserName
        {
            get => _UserName;
            set
            {
                _UserName = value; OnPropertyChanged("UserName");
            }
        }
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set
            {
                _IsBusy = value; OnPropertyChanged("IsBusy");
            }
        }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        private bool _SaveLogin;
        public bool SaveLogin { get => _SaveLogin; set { _SaveLogin = value; OnPropertyChanged("SaveLogin"); } }
        private int _UserId;
        public int UserId { get => _UserId; set { _UserId = value; OnPropertyChanged(); } }
        public User _User;
        public User User { get => _User; set { _User = value; OnPropertyChanged("User"); } }
        public ICommand CloseCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public LoginViewModel()
        {
            //MainViewModel.CertificateValidation();
            IsLogin = false;
            Password = "";
            UserName = "";
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Password = "";
                UserName = "";
                try
                {
                    SaveLogin = QLHS_DR.Properties.Settings.Default.StatusSavePass;
                    CredentialData credentialData = ConfigurationUtil.LoadCredentialData(AppInfo.FolderPath);
                    if (credentialData != null)
                    {
                        UserName = credentialData.UserId;
                        Password = credentialData.Password;
                        Login(p);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exceptiton: " + ex.Message);
                }
            });
            LoginCommand = new RelayCommand<Window>((p) => { if (_UserName != null && _Password != null && _Password != "") return true; else return false; }, (p) =>
            {
                Window window = p;
                Login(window);
            });
            CloseCommand = new RelayCommand<Window>((w) => { return true; }, (p) => { CheckBoxSave(_SaveLogin); p.Close(); IsLogin = false; });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) =>
            {
                try
                {
                    Password = p.Password;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            });
        }
        public static Version GetRunningVersion()
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
        void Login(Window p)
        {
            IsLogin = false;
            IsBusy = true;
            if (p == null) return;
            string passEncode = Convert.ToBase64String(CryptoUtil.HashPassword(Encoding.UTF8.GetBytes(_Password), CryptoUtil.GetSalt(_UserName)));

            _MyClient = ServiceHelper.NewMessageServiceClient(_UserName, passEncode);

            try
            {
                _MyClient.Open();
                User = _MyClient.GetUserByName(_UserName);
                LoginManager loginManager = new LoginManager()
                {
                    ComputerName = Environment.MachineName,
                    LoginIp = EofficeCommonLibrary.Common.MyCommon.GetLocalIPAddress(),
                    LogType = LoginType.Login,
                    ApplicationVersion = GetRunningVersion().ToString(),
                    ApplicationName = AppDomain.CurrentDomain.FriendlyName
                };
                _MyClient.RecordLogin(loginManager);
                SectionLogin.Ins.CurrentUser = User;
                SectionLogin.Ins.Permissions = _MyClient.GetPermissions(_User.Id);
                SectionLogin.Ins.ListPermissions = _MyClient.GetPermissionOfUser(_User.UserName).ToList();
                _MyClient.Close();
                SectionLogin.Ins.Token = passEncode;
                IsLogin = true;

                CheckBoxSave(_SaveLogin);
                if (_SaveLogin && IsLogin)
                {
                    try
                    {
                        ConfigurationUtil.SaveCredentialData(new CredentialData
                        {
                            Password = _Password,
                            UserId = _UserName
                        }, AppInfo.FolderPath);

                        string keyName = "QLHS_DR"; // Tên khóa đăng ký của ứng dụng của bạn 
                        string appPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                        RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                        if (rk == null)
                        {
                            rk = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                        }
                        rk.SetValue(keyName, appPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exceptiton: " + ex.Message);
                    }
                    //ConfigurationUtil.SaveCredentialData(new CredentialData
                    //{
                    //    Password = _Password,
                    //    UserId = _UserName
                    //}, AppInfo.FolderPath);
                }
                else
                {
                    try
                    {
                        ConfigurationUtil.RemoveCreditalData(AppInfo.FolderPath);
                        string keyName = "QLHS_DR"; // Tên khóa đăng ký của ứng dụng của bạn

                        RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                        if (rk != null)
                        {
                            rk.DeleteValue(keyName, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exceptiton: " + ex.Message);
                    }
                }
                //ChatApp Login
                //var uri = "net.tcp://192.168.11.12:8080/EofficeService/Service";
                //var callBack = new InstanceContext(new MessageServiceCallBack());
                //var binding = new NetTcpBinding(SecurityMode.None);
                //var channel = new DuplexChannelFactory<IMessageService>(callBack, binding);
                //var endPoint = new EndpointAddress(uri);
                //var proxy = channel.CreateChannel(endPoint);
                //proxy?.Connect(User.Id);              
                p.Close();
            }
            catch (Exception ex)
            {
                _MyClient.Abort();
                IsLogin = false;
                MessageBox.Show("Exceptiton: " + ex.Message);
            }

            finally
            { IsBusy = false; }
        }
        public void CheckBoxSave(bool status)
        {
            QLHS_DR.Properties.Settings.Default.StatusSavePass = status;
            QLHS_DR.Properties.Settings.Default.Save();
        }
    }
}