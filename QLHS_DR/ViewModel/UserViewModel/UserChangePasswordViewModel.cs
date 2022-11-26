using EofficeCommonLibrary.Common.Util;
using QLHS_DR.Core;
using QLHS_DR.ServiceReference1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace QLHS_DR.ViewModel.UserViewModel
{
    class UserChangePasswordViewModel : BaseViewModel, IDataErrorInfo
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
                        break;
                }
                return res;
            }
        }
        private string _CurrentPassword;
        public string CurrentPassword { get => _CurrentPassword; set { _CurrentPassword = value; OnPropertyChanged("CurrentPassword"); } }
        private string _NewPassword;
        public string NewPassword { get => _NewPassword; set { _NewPassword = value; OnPropertyChanged("NewPassword"); } }
        private string _ConfirmPassword;
        public string ConfirmPassword { get => _ConfirmPassword; set { _ConfirmPassword = value; OnPropertyChanged("ConfirmPassword"); } }

        public ICommand CloseCommand { get; set; }
        public ICommand UpdatePasswordCommand { get; set; }
        public ICommand CurrentPasswordChangedCommand { get; set; }
        public ICommand NewPasswordChangedCommand { get; set; }
        public ICommand ConfirmNewPasswordChangedCommand { get; set; }

        public UserChangePasswordViewModel()
        {
            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });
            UpdatePasswordCommand = new RelayCommand<Window>((p) => { if (CurrentPassword != null & NewPassword != null & ConfirmPassword != null & ConfirmPassword == NewPassword) return true; else return false; }, (p) =>
            {
                ServiceReference1.EEMCDRWcfServiceClient client = new ServiceReference1.EEMCDRWcfServiceClient();
                try
                {
                    client.ClientCredentials.UserName.UserName = SectionLogin.Ins.CurrentUser.UserName;
                    client.ClientCredentials.UserName.Password = System.Text.Encoding.UTF8.GetString(SectionLogin.Ins.CurrentUser.Password);
                    client.Open();
                    string hashOldPass = Convert.ToBase64String(CryptoUtil.HashPassword(Encoding.UTF8.GetBytes(_CurrentPassword), CryptoUtil.GetSalt(SectionLogin.Ins.CurrentUser.UserName)));
                    string hashNewPass = Convert.ToBase64String(CryptoUtil.HashPassword(Encoding.UTF8.GetBytes(_NewPassword), CryptoUtil.GetSalt(SectionLogin.Ins.CurrentUser.UserName)));
                    if (client.ChangePassword(SectionLogin.Ins.CurrentUser.UserName, hashOldPass, hashNewPass) == LoginStatusType.SUCCESS)
                    {
                        SectionLogin.Ins.CurrentUser.Password = CryptoUtil.HashPassword(Encoding.UTF8.GetBytes(_CurrentPassword));
                        MessageBox.Show("Đổi mật khẩu thành công");
                        p.Close();
                    }
                }
                catch (TimeoutException exception)
                {
                    MessageBox.Show(exception.Message);
                    client.Abort();
                }
                catch (MessageSecurityException exception)
                {
                    MessageBox.Show(exception.InnerException.Message);
                    client.Abort();
                }
                catch (CommunicationException exception)
                {
                    MessageBox.Show(exception.Message);
                    client.Abort();
                }
            });
            CurrentPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { CurrentPassword = p?.Password; });
            NewPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { NewPassword = p?.Password; });
            ConfirmNewPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { ConfirmPassword = p?.Password; });
        }
    }
}
