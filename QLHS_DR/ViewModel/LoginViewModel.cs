using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EofficeCommonLibrary;
using EofficeCommonLibrary.Common.Util;
using EofficeCommonLibrary.Model.DataType;
using DevExpress.Mvvm;
using QLHS_DR.Core;
using QLHS_DR.ViewModel;
using EofficeClient.Core;
using DevExpress.XtraExport.Xls;
using QLHS_DR.EOfficeServiceReference;
using QLHS_DR.ViewModel.ChatAppViewModel;
using QLHS_DR.ChatAppServiceReference;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace EofficeClient.ViewModel
{

    class LoginViewModel : BaseViewModel, IDataErrorInfo
    {
        private EofficeMainServiceClient _MyClient;
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
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        private bool _SaveLogin;
        public bool SaveLogin { get => _SaveLogin; set { _SaveLogin = value; OnPropertyChanged("SaveLogin"); } }
        private int _UserId;
        public int UserId { get => _UserId; set { _UserId = value; OnPropertyChanged(); } }
        public QLHS_DR.EOfficeServiceReference.User _User;
        public QLHS_DR.EOfficeServiceReference.User User { get => _User; set { _User = value; OnPropertyChanged("User"); } }
        public ICommand CloseCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public LoginViewModel()
        {
            IsLogin = false;
            Password = "";
            UserName = "";
            LoadedWindowCommand = new RelayCommand<Window>((w) => { return true; }, (p) => {
                Password = "";
                UserName = "";
                try
                {
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
            LoginCommand = new RelayCommand<Window>((w) => { if (_UserName != null && _Password != null && _Password != "") return true; else return false; }, (p) =>
            {
                Login(p);
                if (_SaveLogin)
                {
                    ConfigurationUtil.SaveCredentialData(new CredentialData
                    {
                        Password = _Password,
                        UserId = _UserName
                    }, AppInfo.FolderPath);
                }
                else
                {
                    ConfigurationUtil.RemoveCreditalData(AppInfo.FolderPath);
                }
            });
            CloseCommand = new RelayCommand<Window>((w) => { return true; }, (p) => { CheckBoxSave(_SaveLogin); p.Close(); IsLogin = false; });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
        }
        void Login(Window p)
        {
            IsLogin = false;
            if (p == null) return;
            string passEncode = Convert.ToBase64String(CryptoUtil.HashPassword(Encoding.UTF8.GetBytes(_Password), CryptoUtil.GetSalt(_UserName)));
            
            _MyClient = ServiceHelper.NewEofficeMainServiceClient(_UserName, passEncode);
            try
            {
                // System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => { return true; }; //Igno SSL
                _MyClient.Open();               
                User = _MyClient.GetUserByName(_UserName);                
                SectionLogin.Ins.CurrentUser = User;
                SectionLogin.Ins.Permissions = _MyClient.GetPermissions(_User.Id);
                SectionLogin.Ins.ListPermissions = _MyClient.GetPermissionOfUser(_User.UserName).ToList();
                _MyClient.Close();
                SectionLogin.Ins.Token = passEncode;
                IsLogin = true;
                CheckBoxSave(_SaveLogin);
                if (_SaveLogin && IsLogin)
                {
                    ConfigurationUtil.SaveCredentialData(new CredentialData
                    {
                        Password = _Password,
                        UserId = _UserName
                    }, AppInfo.FolderPath);
                }
                else
                {
                    ConfigurationUtil.RemoveCreditalData(AppInfo.FolderPath);
                }
                //ChatApp Login

                //var uri = "net.tcp://192.168.11.12:8080/EofficeService/Service";
                //var callBack = new InstanceContext(new MessageServiceCallBack());
                //var binding = new NetTcpBinding(SecurityMode.None);
                //var channel = new DuplexChannelFactory<IMessageService>(callBack, binding);
                //var endPoint = new EndpointAddress(uri);
                //var proxy = channel.CreateChannel(endPoint);
                //proxy?.Connect(User.Id);

                //Uri baseAddress = new Uri("net.tcp://192.168.11.12:8080/EofficeService/Service");
                //NetTcpBinding wsd = new NetTcpBinding();
                ////wsd.Security = SecurityMode.None;
                //EndpointAddress ea = new EndpointAddress(baseAddress, EndpointIdentity.CreateDnsIdentity("192.168.11.12:8080"));
                //MessageServiceClient _ChatClient = new MessageServiceClient(callBack, wsd,ea);

                //////_ChatClient.ClientCredentials.UserName.UserName = _UserName;
                //////_ChatClient.ClientCredentials.UserName.Password = passEncode;
                //_ChatClient.Open(); 
                //_ChatClient.Connect(User.Id);
                //var enpoint = _ChatClient.Endpoint;
                              
                p.Close();
                
            }
            catch (TimeoutException exception)
            {
                IsLogin = false;
                MessageBox.Show("Exceptiton: " + exception.Message);
                _MyClient.Abort();
            }
            catch (MessageSecurityException exception)
            {
                IsLogin = false;
                MessageBox.Show(exception.InnerException.Message);
                _MyClient.Abort();
            }
            catch (CommunicationException exception)
            {
                IsLogin = false;
                MessageBox.Show("Exceptiton: " + exception.Message);
                _MyClient.Abort();
            }
        }
        public void CheckBoxSave(bool status)
        {
            QLHS_DR.Properties.Settings.Default.StatusSavePass = status;           
            QLHS_DR.Properties.Settings.Default.Save();
        }
    }
}