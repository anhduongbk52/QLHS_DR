using DevExpress.Xpf.WindowsUI.Internal;
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
    internal class LogViewModel:BaseViewModel
    {
        #region "Properties and Field"
        ServiceFactory _ServiceFactory;
        private int _MaxResult;
        public int MaxResult
        {
            get => _MaxResult;
            set
            {
                if (_MaxResult != value)
                {
                    _MaxResult = value;
                    NotifyPropertyChanged("MaxResult");
                }
            }
        }
      
        private ObservableCollection<Log> _Logs;
        public ObservableCollection<Log> Logs
        {
            get => _Logs;
            set
            {
                if (_Logs != value)
                {
                    _Logs = value;
                    NotifyPropertyChanged("Logs");
                }
            }
        }
        private ObservableCollection<User> _Users;
        public ObservableCollection<User> Users
        {
            get => _Users;
            set
            {
                if (_Users != value)
                {
                    _Users = value;
                    NotifyPropertyChanged("Users");
                }
            }
        }
        private User _UserSelected;
        public User UserSelected
        {
            get => _UserSelected;
            set
            {
                if (_UserSelected != value)
                {
                    _UserSelected = value; OnPropertyChanged("UserSelected");
                }
            }
        }
        private ObservableCollection<LogType> _LogTypes;
        public ObservableCollection<LogType> LogTypes
        {
            get => _LogTypes;
            set
            {
                if (_LogTypes != value)
                {
                    _LogTypes = value; OnPropertyChanged("LogTypes");
                }
            }
        }
        private LogType _LogTypeSelected;
        public LogType LogTypeSelected
        {
            get => _LogTypeSelected;
            set
            {
                if (_LogTypeSelected != value)
                {
                    _LogTypeSelected = value; OnPropertyChanged("LogTypeSelected");
                }
            }
        }
        private DateTime _FromDate;
        public DateTime FromDate
        {
            get => _FromDate;
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value; OnPropertyChanged("FromDate");
                }
            }
        }
        private DateTime _ToDate;
        public DateTime ToDate
        {
            get => _ToDate;
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value; OnPropertyChanged("ToDate");
                }
            }
        }
        #endregion

        #region "Command"
        public ICommand SearchCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        #endregion

        internal LogViewModel() 
        {
            _ServiceFactory = new ServiceFactory();
            try
            {
                LogTypes = new ObservableCollection<LogType>() { LogType.NONE, LogType.LOGIN,LogType.LOGOUT,LogType.CREATE_USER, LogType.EDIT_USER, LogType.DELETE_USER,LogType.DELETE_USER, LogType.LOCK_USER, LogType.UNLOCK_USER, LogType.CHANGE_PERMISSION, LogType.PRINT_DOCUMENT, LogType.SIGN_DOCUMENT, LogType.CREATE_TASK, LogType.MODIFY_TASK, LogType.DELETE_TASK, LogType.ADD_USER_TO_TASK, LogType.ADD_DOCUMENT_TO_TASK, LogType.DELETE_DOCUMENT, LogType.START_SERVICE, LogType.STOP_SERVICE, LogType.RESTART_SERVICE, LogType.SETUP_SSL, LogType.SETUP_USER_DATA_DIR, LogType.REVOKE_TASK, LogType.CHANGE_DEFAULT_PASSWORD, LogType.CHANGE_USER_PASSWORD };
                Users = _ServiceFactory.LoadUsers();
                MaxResult = 50;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            SearchCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                try
                {
                    Logs = _ServiceFactory.GetLogs(_FromDate,_ToDate,_LogTypeSelected,_UserSelected!=null? _UserSelected.Id:-1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            ClearCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                Logs.Clear();
                try
                {
                    UserSelected = null;
                    LogTypeSelected = LogType.NONE;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
