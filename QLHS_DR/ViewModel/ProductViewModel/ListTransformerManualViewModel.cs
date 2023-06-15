using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.HosoView;
using QLHS_DR.View.ProductView;
using QLHS_DR.View.TransformerManualView;
using QLHS_DR.ViewModel.HoSoViewModel;
using QLHS_DR.ViewModel.TransformerManualViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    class ListTransformerManualViewModel : BaseViewModel
    {
        #region "Properties and Field"

        MessageServiceClient _Proxy;

        private bool _CanOpenFile;
        public bool CanOpenFile
        {
            get => _CanOpenFile;
            set
            {
                if (_CanOpenFile != value)
                {
                    _CanOpenFile = value;
                    OnPropertyChanged("CanOpenFileElectrical");
                }
            }
        }
        private bool _CanUploadFile;
        public bool CanUploadFile
        {
            get => _CanUploadFile;
            set
            {
                if (_CanUploadFile != value)
                {
                    _CanUploadFile = value;
                    OnPropertyChanged("CanUploadFileElectrical");
                }
            }
        }

        private bool _CanRemoveFile;
        public bool CanRemoveFile
        {
            get => _CanRemoveFile;
            set
            {
                if (_CanRemoveFile != value)
                {
                    _CanRemoveFile = value;
                    OnPropertyChanged("CanRemoveFile");
                }
            }
        }
        private bool _CanRemoveOwnerFile;
        public bool CanRemoveOwnerFile
        {
            get => _CanRemoveOwnerFile;
            set
            {
                if (_CanRemoveOwnerFile != value)
                {
                    _CanRemoveOwnerFile = value;
                    OnPropertyChanged("CanRemoveOwnerFile");
                }
            }
        }
        private Product _Product;
        private TransformerManualDTO _SelectedTransformerManualDTO;
        public TransformerManualDTO SelectedTransformerManualDTO
        {
            get => _SelectedTransformerManualDTO;
            set
            {
                if (_SelectedTransformerManualDTO != value)
                {
                    _SelectedTransformerManualDTO = value;
                    OnPropertyChanged("SelectedTransformerManualDTO");
                }
            }
        }

        private ObservableCollection<TransformerManualDTO> _TransformerManualDTOs;
        public ObservableCollection<TransformerManualDTO> TransformerManualDTOs { get => _TransformerManualDTOs; set { _TransformerManualDTOs = value; OnPropertyChanged("TransformerManualDTOs"); } }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand OpenPDFCommand { get; set; }
        public ICommand UploadFileHoSoCommand { get; set; }
        public ICommand RemoveFileHoSoCommand { get; set; }
        public ICommand ChangeFileHoSoCommand { get; set; }

        #endregion
        public ListTransformerManualViewModel(Product product)
        {
            _Product = product;

            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CanOpenFile = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productOpenTransformerManualFile");
                CanUploadFile = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productUploadTransformerManualFile");
                CanRemoveFile = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productRemoveTransformerManual");
                CanRemoveOwnerFile = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productRemoveTransformerManualOfOwner");
                TransformerManualDTOs = LoadTransformerManual();
            });
            OpenPDFCommand = new RelayCommand<Object>((p) => { if (_SelectedTransformerManualDTO != null && CanOpenFile) return true; else return false; }, (p) =>
            {
                TransformerManualViewPdf pdfViewer = new TransformerManualViewPdf(true, true, _SelectedTransformerManualDTO.FileName, _SelectedTransformerManualDTO);
                pdfViewer.Show();
            });

            UploadFileHoSoCommand = new RelayCommand<Object>((p) => { if (_CanUploadFile) return true; else return false; }, (p) =>
            {
                UploadHoSoViewModel uploadTransformerManualViewModel = new UploadHoSoViewModel(_Product.ProductCode);
                UploadHoSoWindow uploadTransformerManualWindow = new UploadHoSoWindow() { DataContext = uploadTransformerManualViewModel };
                uploadTransformerManualWindow.ShowDialog();
                TransformerManualDTOs = LoadTransformerManual();
            });
            RemoveFileHoSoCommand = new RelayCommand<Object>((p) => { if (_SelectedTransformerManualDTO != null && (CanRemoveFile || (CanRemoveOwnerFile && _SelectedTransformerManualDTO.UserCreateId == SectionLogin.Ins.CurrentUser.Id))) return true; else return false; }, (p) =>
            {
                try
                {
                    if (MessageBox.Show("Bạn có muốn xóa file: " + _SelectedTransformerManualDTO.FileName, "Cảnh báo", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        _Proxy = ServiceHelper.NewMessageServiceClient();
                        _Proxy.Open();
                        _Proxy.SetDeletedTransformerManual(_SelectedTransformerManualDTO.TransformerManualId);
                        _Proxy.Close();
                        MessageBox.Show("Xóa thành công");
                    }
                    TransformerManualDTOs = LoadTransformerManual();
                }
                catch (Exception ex)
                {
                    _Proxy.Abort();
                    MessageBox.Show(ex.Message);
                }
            });
            ChangeFileHoSoCommand = new RelayCommand<Object>((p) => { if (_SelectedTransformerManualDTO != null && (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productUploadTransformerManualFile") || (_SelectedTransformerManualDTO.UserCreateId == SectionLogin.Ins.CurrentUser.Id))) return true; else return false; }, (p) =>
            {
                EditTransformerManualViewModel editTransformerManualViewModel = new EditTransformerManualViewModel(_SelectedTransformerManualDTO, _Product.ProductCode);
                EditTransformerManualWindow editTransformerManualWindow = new EditTransformerManualWindow() { DataContext = editTransformerManualViewModel };
                editTransformerManualWindow.ShowDialog();
                TransformerManualDTOs = LoadTransformerManual();
            });
        }
        private ObservableCollection<TransformerManualDTO> LoadTransformerManual()
        {
            ObservableCollection<TransformerManualDTO> ketqua = new ObservableCollection<TransformerManualDTO>();
            try
            {
                _Proxy = ServiceHelper.NewMessageServiceClient();
                _Proxy.Open();
                ketqua = _Proxy.LoadTransformerManual(_Product.Id).ToObservableCollection();
                _Proxy.Close();
            }
            catch (CommunicationException ex)
            {
                _Proxy.Abort();
                MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
    }
}
