using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.UserViewModel
{
    class NewGroupsUserViewModel : BaseViewModel
    {
        private string _GroupName;
        public string GroupName
        {
            get => _GroupName;
            set { _GroupName = value; OnPropertyChanged("GroupName"); }
        }
        private string _Description;
        public string Description
        {
            get => _Description;
            set { _Description = value; OnPropertyChanged("Description"); }
        }
        public ICommand NewGroupsUserCommand { get; set; }
        public NewGroupsUserViewModel()
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            NewGroupsUserCommand = new RelayCommand<UserControl>((p) => { if (string.IsNullOrEmpty(_GroupName) || string.IsNullOrEmpty(_Description)) return false; else return true; }, (p) =>
            {
                serviceFactory.NewRole(_GroupName, _Description);
            });
        }
    }
}
