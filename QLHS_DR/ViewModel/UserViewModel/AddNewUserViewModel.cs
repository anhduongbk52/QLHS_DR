using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.UserViewModel
{
    class AddNewUserViewModel : BaseViewModel, IDataErrorInfo
    {

        public string Error { get { return null; } }
        public string this[string columnName]
        {
            get
            {
                string res = null;
                var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
                switch (columnName)
                {
                    case "UserName":
                        if (string.IsNullOrEmpty(_UserName))
                        {
                            res = "UserName cannot be empty";
                        }
                        else if (!regexItem.IsMatch(_UserName))
                            res = "UserName cannot contains special character";
                        //else if (BtkModel.DataProvider.Ins.DB.Users.Where(x => x.UserName == _UserName).Count() >= 0)
                        //    res = "Username already exists";
                        break;
                    case "PassWord":
                        if (_PassWord?.Length < 6)
                        {
                            res = "Minimum password length to at least a value of 6";
                        }
                        break;
                }
                return res;
            }
        }
        private string _UserName;
        public string UserName
        {
            get => _UserName;
            set { _UserName = value; OnPropertyChanged("UserName"); }
        }
        private string _PassWord;
        public string PassWord
        {
            get => _PassWord;
            set { _PassWord = value; OnPropertyChanged("PassWord"); }
        }
        private string _FullName;
        public string FullName
        {
            get => _FullName;
            set { _FullName = value; OnPropertyChanged("FullName"); }
        }
        private string _Email;
        public string Email
        {
            get => _Email;
            set { _Email = value; OnPropertyChanged("Email"); }
        }
        private string _Phone;
        public string Phone
        {
            get => _Phone;
            set { _Phone = value; OnPropertyChanged("Phone"); }
        }
        private DateTime? _Birday;
        public DateTime? Birday
        {
            get => _Birday;
            set { _Birday = value; OnPropertyChanged("_Birday"); }
        }
        private bool _Gender;
        public bool Gender
        {
            get => _Gender;
            set { _Gender = value; OnPropertyChanged("Gender"); }
        }
        private string _Address;
        public string Address
        {
            get => _Address;
            set { _Address = value; OnPropertyChanged("Address"); }
        }
        private string _Note;
        public string Note
        {
            get => _Note;
            set { _Note = value; OnPropertyChanged("Note"); }
        }
        private byte[] _Photo;
        public byte[] Photo
        {
            get => _Photo;
            set { _Photo = value; OnPropertyChanged("Photo"); }
        }
        public ICommand AddNewUserCommand { get; set; }
        public AddNewUserViewModel()
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            AddNewUserCommand = new RelayCommand<Object>((p) => { if (string.IsNullOrEmpty(_Phone) || string.IsNullOrEmpty(_Email) || string.IsNullOrEmpty(_FullName) || string.IsNullOrEmpty(_PassWord)) return false; else return true; }, (p) =>
            {
                User user = new User()
                {
                    UserName = _UserName,
                    Password = Encoding.UTF8.GetBytes(_PassWord.Trim()),
                    AttemptCount = 0,
                    FullName = _FullName,
                    BirthDate = _Birday,
                    HomeTown = _Address,
                    MobiphoneNumber = _Phone,
                    Email = _Email,
                    Avatar = _Photo
                };
                serviceFactory.NewUser(user, false);
                MessageBox.Show("Thêm mới thành công");
            });

        }
    }
}
