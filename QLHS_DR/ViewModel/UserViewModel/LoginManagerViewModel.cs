using DevExpress.Mvvm.Xpf;
using DevExpress.Mvvm;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using DevExpress.Mvvm.Native;

namespace QLHS_DR.ViewModel.UserViewModel
{
    internal class LoginManagerViewModel : BaseViewModel
    {
        #region "Properties and Field"
        ServiceFactory _ServiceFactory;
        private Dictionary<int, User> _Users;
        private ObservableCollection<LoginManager> _LoginManagers;
        public ObservableCollection<LoginManager> LoginManagers
        {
            get => _LoginManagers;
            set
            {
                if (_LoginManagers != value)
                {
                    _LoginManagers = value;
                    NotifyPropertyChanged("LoginManagers");
                }
            }
        }


        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand AddToTrustedListCommand { get; set; }
        public ICommand UnTrustedCommand { get; set; }
        public ICommand<RowUpdatedArgs> RowUpdatedCommand { get; set; }
        #endregion
        public LoginManagerViewModel()
        {
            _ServiceFactory = new ServiceFactory();
            LoginManagers = new ObservableCollection<LoginManager>();
            _Users = _ServiceFactory.LoadUsers().ToDictionary(user => user.Id);

            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                LoginManagers = LoadLoginManagers();
            });
            AddToTrustedListCommand = new RelayCommand<LoginManager>((p) => { if (p != null && p.Trusted != true) return true; else return false; }, (p) =>
            {
                _ServiceFactory.SetTrustedLogin(p.Id, true);
                p.Trusted = true;
            });
            UnTrustedCommand = new RelayCommand<LoginManager>((p) => { if (p != null && p.Trusted != false) return true; else return false; }, (p) =>
            {
                _ServiceFactory.SetTrustedLogin(p.Id, false);
                p.Trusted = false;
            });
            RowUpdatedCommand = new DelegateCommand<RowUpdatedArgs>((p) =>
            {
                try
                {
                    LoginManager loginManager = (LoginManager)p.Item;
                    _ServiceFactory.UpdateLoginManager(loginManager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
        private ObservableCollection<LoginManager> LoadLoginManagers()
        {
            var temp = _ServiceFactory.LoadLoginManagers().ToObservableCollection();
            if (_Users != null)
            {
                foreach (var item in temp)
                {
                    item.User = _Users[item.LoginUserId];
                }
            }
            return temp;
        }

    }
}
