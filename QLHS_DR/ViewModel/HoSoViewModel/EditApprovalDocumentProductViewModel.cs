using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.HoSoViewModel
{
    class EditApprovalDocumentProductViewModel : BaseViewModel
    {
        #region "Field and Properties"
        ServiceFactory _ServiceFactory;
        private string _TittleWindow;
        public string TittleWindow { get => _TittleWindow; set { _TittleWindow = value; OnPropertyChanged("TittleWindow"); } }
        private string _DocumentName;
        public string DocumentName { get => _DocumentName; set { _DocumentName = value; OnPropertyChanged("DocumentName"); } }
        private string _Description;
        public string Description { get => _Description; set { _Description = value; OnPropertyChanged("Description"); } }
        private int _ApprovalNumber;
        public int ApprovalNumber
        {
            get => _ApprovalNumber;
            set
            {
                if (_ApprovalNumber != value)
                {
                    _ApprovalNumber = value;
                    NotifyPropertyChanged("ApprovalNumber");
                }
            }
        }
        #endregion

        #region "Commands"
        public ICommand ExitCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        #endregion

        public EditApprovalDocumentProductViewModel(ApprovalDocumentProduct approvalDocumentProduct)
        {
            _ServiceFactory = new ServiceFactory();

            TittleWindow = "Sửa đổi thông tin tài liệu phê duyệt: " + approvalDocumentProduct.DocumentName;
            DocumentName = approvalDocumentProduct.DocumentName;
            ApprovalNumber = approvalDocumentProduct.ApprovalNumber;
            Description = approvalDocumentProduct.Description;

            LoadedWindowCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                //Gettransformer               
            });
            UpdateCommand = new RelayCommand<Window>((p) => { if (!String.IsNullOrEmpty(_DocumentName) && _ApprovalNumber != 0) return true; else return false; }, (p) =>
            {
                approvalDocumentProduct.ApprovalNumber = _ApprovalNumber;
                approvalDocumentProduct.DocumentName = _DocumentName;
                approvalDocumentProduct.Description = _Description;
                _ServiceFactory.UpdateApprovalDocumentProduct(approvalDocumentProduct);
                p.Close();
            });
            ExitCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                p.Close();
            });
        }


    }
}
