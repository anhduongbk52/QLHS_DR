using EofficeCommonLibrary.Common.Util;
using QLHS_DR.Core;
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
using QLHS_DR.ChatAppServiceReference;
using EofficeClient.Core;

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
                MessageServiceClient client = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                try
                {
                    client.Open();
                    string hashOldPass = Convert.ToBase64String(CryptoUtil.HashPassword(Encoding.UTF8.GetBytes(_CurrentPassword), CryptoUtil.GetSalt(SectionLogin.Ins.CurrentUser.UserName)));
                    string hashNewPass = Convert.ToBase64String(CryptoUtil.HashPassword(Encoding.UTF8.GetBytes(_NewPassword), CryptoUtil.GetSalt(SectionLogin.Ins.CurrentUser.UserName)));
                    if (client.ChangePassword(SectionLogin.Ins.CurrentUser.UserName, hashOldPass, hashNewPass) == LoginStatusType.SUCCESS)
                    {
                        SectionLogin.Ins.CurrentUser.Password = CryptoUtil.HashPassword(Encoding.UTF8.GetBytes(_CurrentPassword));
                        SectionLogin.Ins.Token = hashNewPass;
                        MessageBox.Show("Đổi mật khẩu thành công");
                    }
                    else MessageBox.Show("Thao tác thất bại, vui lòng thử lại!");
                    p.Close();
                }
              
                catch (Exception exception)
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
