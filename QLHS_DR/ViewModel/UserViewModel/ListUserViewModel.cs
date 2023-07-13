using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.UserViewModel
{
    class ListUserViewModel : BaseViewModel
    {
        MessageServiceClient _Client;
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set
            {
                _IsBusy = value; OnPropertyChanged("IsBusy");
            }
        }
        private string _SearchKeyWord;
        public string SearchKeyWord
        {
            get => _SearchKeyWord;
            set
            {
                if (_SearchKeyWord != value)
                {
                    _SearchKeyWord = value;
                    ListUsers = SearchUser(SearchKeyWord);
                    OnPropertyChanged("SearchKeyWord");
                    OnPropertyChanged("ListUsers");
                }
            }
        }
        private User _SelectedUser;
        public User SelectedUser
        {
            get => _SelectedUser;
            set
            {
                if (_SelectedUser != value)
                {
                    _SelectedUser = value;
                    OnPropertyChanged("SelectedUser");
                }
            }
        }
        private ObservableCollection<User> _ListUsers;
        public ObservableCollection<User> ListUsers
        {
            get => _ListUsers;
            set
            {
                if (_ListUsers != value)
                {
                    _ListUsers = value;
                    OnPropertyChanged("ListUsers");
                }
            }
        }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand LockUserCommand { get; set; }
        public ICommand UnLockUserCommand { get; set; }
        public ICommand SaveChangeCommand { get; set; }
        public ICommand ResetPasswordCommand { get; set; }
        public ICommand ResetAttemptCountCommand { get; set; }
        public ListUserViewModel()
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (_SearchKeyWord != null)
                {
                    SearchUser(_SearchKeyWord);
                }
                else
                {
                    LoadAllUser();
                }
            });

            LockUserCommand = new RelayCommand<User>((p) => { if (p == null || p?.IsLocked == true) return false; else if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "userLockUser")) return true; return false; }, (p) =>
            {
                if (MessageBox.Show("Bạn có muốn khóa user: " + p.FullName, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        _Client = ServiceHelper.NewMessageServiceClient();
                        _Client.Open();
                        _Client.LockUser(p.Id);
                        _Client.Close();
                        System.Windows.MessageBox.Show("User " + p.UserName + "đã bị khóa!");
                    }
                    catch (Exception ex)
                    {
                        _Client.Abort();
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
            });
            UnLockUserCommand = new RelayCommand<User>((p) => { if (p == null || (p?.IsLocked) == false) return false; else if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "userUnlockUser")) return true; return false; }, (p) =>
            {
                if (MessageBox.Show("Bạn có muốn mở khóa user: " + p.FullName, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        _Client = ServiceHelper.NewMessageServiceClient();
                        _Client.Open();
                        _Client.UnLockUser(p.Id);
                        _Client.Close();
                        System.Windows.MessageBox.Show("User " + p.UserName + "được mở khóa!");
                    }
                    catch (Exception ex)
                    {
                        _Client.Abort();
                        MessageBox.Show(ex.Message);
                    }
                }
            });
            ResetAttemptCountCommand = new RelayCommand<User>((p) => { if (p != null && p.AttemptCount > 0 && SectionLogin.Ins.ListPermissions.Any(x => x.Code == "userResetAttemptCount")) return true; else return false; }, (p) =>
            {
                if (MessageBox.Show("Bạn có muốn mở reset lại số lần đăng nhập sai của: " + p.FullName, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    serviceFactory.ResetAttemptCount(p.Id);
                    p.AttemptCount = 0;
                }
            });
            SaveChangeCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "userEditUser")) return true; else return false; }, (p) =>
            {
                if (MessageBox.Show("Bạn có muốn lưu thay đổi ?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    serviceFactory.UpdateUserInformation(_SelectedUser);
                }
            });
            ResetPasswordCommand = new RelayCommand<User>((p) => { if (p != null && SectionLogin.Ins.ListPermissions.Any(x => x.Code == "userResetPassword")) return true; return false; }, (p) =>
            {
                if (MessageBox.Show("Bạn có muốn đặt lại mật khẩu mặc định cho " + p.FullName + " ?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    serviceFactory.ResetPassword(p.Id);
                }
            });
        }
        private ObservableCollection<User> SearchUser(string key)
        {
            LoadAllUser();
            ObservableCollection<User> kq = _ListUsers.Where(user => user.FullName != null && user.UserName != null && (user.FullName.Contains(key) || user.UserName.Contains(key))).ToObservableCollection();
            return kq;
        }
        private void LoadAllUser()
        {
            try
            {
                IsBusy = true;
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                ListUsers = _Client.GetUsers().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
