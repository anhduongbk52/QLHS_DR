using QLHS_DR.ServiceReference1;
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

namespace EofficeClient.ViewModel
{

    class LoginViewModel : BaseViewModel, IDataErrorInfo
    {
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
        public User _User;
        public User User { get => _User; set { _User = value; OnPropertyChanged("User"); } }
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
                CredentialData credentialData = ConfigurationUtil.LoadCredentialData(AppInfo.FolderPath);
                if (credentialData != null)
                {
                    UserName = credentialData.UserId;
                    Password = credentialData.Password;
                    Login(p);
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
            EEMCDRWcfServiceClient client = Core.ServiceHelper.NewServiceClient(_UserName, passEncode);          

            try
            {
                // System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => { return true; }; //Igno SSL
                client.Open();
                User = client.GetUserByName(_UserName);               
                ServiceProxy.Ins = client;
                SectionLogin.Ins.CurrentUser = User;
                SectionLogin.Ins.Permissions = client.GetPermissions(User.Id);
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
                p.Close();
            }
            catch (TimeoutException exception)
            {
                IsLogin = false;
                MessageBox.Show("Exceptiton: " + exception.Message);
                client.Abort();
            }
            catch (MessageSecurityException exception)
            {
                IsLogin = false;
                MessageBox.Show(exception.InnerException.Message);
                client.Abort();
            }
            catch (CommunicationException exception)
            {
                IsLogin = false;
                MessageBox.Show("Exceptiton: " + exception.Message);
                client.Abort();
            }
        }
        public void CheckBoxSave(bool status)
        {
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings["StatusSavePass"] == null)
                configuration.AppSettings.Settings.Add("StatusSavePass", status.ToString());
            else
                configuration.AppSettings.Settings["StatusSavePass"].Value = status.ToString();
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

    }
}