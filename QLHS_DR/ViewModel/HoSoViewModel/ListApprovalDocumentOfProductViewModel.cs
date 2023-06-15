using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.HosoView;
using QLHS_DR.View.PdfView;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.HoSoViewModel
{
    internal class ListApprovalDocumentOfProductViewModel : BaseViewModel
    {
        #region "Properties and Field"

        ServiceFactory _ServiceFactory;
        private Product _Product;
        private ApprovalDocumentProduct _SelectedApprovalDocumentProduct;
        public ApprovalDocumentProduct SelectedApprovalDocumentProduct
        {
            get => _SelectedApprovalDocumentProduct;
            set
            {
                if (_SelectedApprovalDocumentProduct != value)
                {
                    _SelectedApprovalDocumentProduct = value;
                    OnPropertyChanged("SelectedApprovalDocumentProduct");
                }
            }
        }

        private ObservableCollection<ApprovalDocumentProduct> _ApprovalDocumentProducts;
        public ObservableCollection<ApprovalDocumentProduct> ApprovalDocumentProducts
        {
            get => _ApprovalDocumentProducts;
            set
            {
                if (_ApprovalDocumentProducts != value)
                {
                    _ApprovalDocumentProducts = value;
                    OnPropertyChanged("ApprovalDocumentProducts");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand OpenPDFCommand { get; set; }
        public ICommand UploadApprovalDocumentCommand { get; set; }
        public ICommand RemoveApprovalDocumentCommand { get; set; }
        public ICommand EditApprovalDocumentCommand { get; set; }
        public ICommand AddAppDocToOtherProductCommand { get; set; }
        #endregion
        public ListApprovalDocumentOfProductViewModel(Product product)
        {
            _Product = product;
            _ServiceFactory = new ServiceFactory();

            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ApprovalDocumentProducts = _ServiceFactory.GetApprovalDocumentProducts(_Product.Id, false);
            });
            OpenPDFCommand = new RelayCommand<Object>((p) => { if (_SelectedApprovalDocumentProduct != null && SectionLogin.Ins.CanViewApprovalDocumentProduct) return true; else return false; }, (p) =>
            {
                byte[] temp = _ServiceFactory.DownloadApprovalDocumentProduct(_SelectedApprovalDocumentProduct.Id);
                using (MemoryStream stream = new MemoryStream(temp))
                {
                    CommonPdfViewer commonPdfViewer = new CommonPdfViewer(stream);
                    commonPdfViewer.ShowDialog();
                }
            });
            UploadApprovalDocumentCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                UploadApprovalDocumentProductViewModel uploadApprovalDocumentProductViewModel = new UploadApprovalDocumentProductViewModel(_Product);
                UploadApprovalDocumentProductWindow uploadApprovalDocumentProductWindow = new UploadApprovalDocumentProductWindow() { DataContext = uploadApprovalDocumentProductViewModel };
                uploadApprovalDocumentProductWindow.ShowDialog();
                ApprovalDocumentProducts = _ServiceFactory.GetApprovalDocumentProducts(_Product.Id, false);
            });
            RemoveApprovalDocumentCommand = new RelayCommand<Object>((p) => { if (_SelectedApprovalDocumentProduct != null && (SectionLogin.Ins.CanRemoveApprovalDocumentProduct || (_SelectedApprovalDocumentProduct.UserCreateId == SectionLogin.Ins.CurrentUser.Id))) return true; else return false; }, (p) =>
            {
                if (System.Windows.MessageBox.Show("Bạn có muốn xóa tài liệu: " + _SelectedApprovalDocumentProduct.DocumentName, "Cảnh báo", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    if (_ServiceFactory.SetExpiredApprovalDocumentProduct(_SelectedApprovalDocumentProduct.Id) > 0)
                    {
                        ApprovalDocumentProducts = _ServiceFactory.GetApprovalDocumentProducts(_Product.Id, false);
                    }
                }
            });
            EditApprovalDocumentCommand = new RelayCommand<Object>((p) => { if (_SelectedApprovalDocumentProduct != null && (SectionLogin.Ins.CanUpdateApprovalDocumentProduct || (_SelectedApprovalDocumentProduct.UserCreateId == SectionLogin.Ins.CurrentUser.Id))) return true; else return false; }, (p) =>
            {
                EditApprovalDocumentProductViewModel editApprovalDocumentProductView = new EditApprovalDocumentProductViewModel(_SelectedApprovalDocumentProduct);
                EditApprovalDocumentProductWindow editApprovalDocumentProductWindow = new EditApprovalDocumentProductWindow() { DataContext = editApprovalDocumentProductView };
                editApprovalDocumentProductWindow.ShowDialog();
                ApprovalDocumentProducts = _ServiceFactory.GetApprovalDocumentProducts(_Product.Id, false);
            });
            AddAppDocToOtherProductCommand = new RelayCommand<Object>((p) => { if (_SelectedApprovalDocumentProduct != null) return true; else return false; }, (p) =>
            {
                try
                {
                    ObservableCollection<Object> observableCollection = (ObservableCollection<Object>)p;
                    ObservableCollection<ApprovalDocumentProduct> __approvalDocumentProducts = new ObservableCollection<ApprovalDocumentProduct>();
                    foreach (var item in observableCollection)
                    {
                        __approvalDocumentProducts.Add((ApprovalDocumentProduct)item);
                    }
                    if (__approvalDocumentProducts.Count > 0)
                    {
                        AddApprovalDocToOtherProductViewModel model = new AddApprovalDocToOtherProductViewModel(__approvalDocumentProducts);
                        AddApprovalDocToOtherProductWindow window = new AddApprovalDocToOtherProductWindow() { DataContext = model };
                        window.ShowDialog();
                    }
                }
                catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
            });
        }

    }
}
