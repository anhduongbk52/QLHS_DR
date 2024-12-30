using System;
using System.Windows.Controls;

namespace QLHS_DR.ViewModel
{
    class TabContainer : BaseViewModel,IDisposable
    {
        private string _Header;
        private string _AllowHide;
        private bool _IsSelected;
        private bool _IsVisible;
        private bool _IsEnabled;
        private UserControl _Content;
        public string Header
        {
            get => _Header;
            set
            {
                _Header = value;
                OnPropertyChanged("Header");
            }
        }
        public string AllowHide
        {
            get => _AllowHide;
            set
            {
                _AllowHide = value;
                OnPropertyChanged("AllowHide");
            }
        }
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                _IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        public bool IsVisible
        {
            get => _IsVisible;
            set
            {
                _IsVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        public TabContainer() // Constructor
        {
            AllowHide = "true"; // Gán giá trị mặc định
        }
        public UserControl Content
        {
            get => _Content;
            set
            {
                _Content = value;
                OnPropertyChanged("Content");
            }
        }

        public void Dispose()
        {
            Content = null;
        }
    }
}
