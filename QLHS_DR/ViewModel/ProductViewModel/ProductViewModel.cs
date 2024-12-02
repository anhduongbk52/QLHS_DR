using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.ContractView;
using QLHS_DR.View.HosoView;
using QLHS_DR.View.ProductView;
using QLHS_DR.ViewModel.ContractViewModel;
using QLHS_DR.ViewModel.HoSoViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    internal class ProductViewModel : BaseViewModel
    {
        #region "Field and properties"
        ServiceFactory _ServiceFactory;
        private bool _IsFirtLoad;
        private Product _Product;
        public Product Product
        {
            get => _Product;
            set
            {
                if (_Product != value)
                {
                    _Product = value;
                    OnPropertyChanged("Product");
                }
            }
        }
        private ProductTypeNew _ProductTypeNew;
        public ProductTypeNew ProductTypeNew
        {
            get => _ProductTypeNew;
            set
            {
                if (_ProductTypeNew != value)
                {
                    _ProductTypeNew = value;
                    OnPropertyChanged("ProductTypeNew");
                }
            }
        }
        private ObservableCollection<ProductTypeNew> _ProductTypeNews;
        public ObservableCollection<ProductTypeNew> ProductTypeNews
        {
            get => _ProductTypeNews;
            set
            {
                if (_ProductTypeNews != value)
                {
                    _ProductTypeNews = value; OnPropertyChanged("ProductTypeNews");
                }
            }
        }
        public UserControl _LoadUC;
        public UserControl LoadUC
        {
            get => _LoadUC; set { if (_LoadUC != value) { _LoadUC = value; NotifyPropertyChanged("LoadUC"); } }
        } //UserControl hien thoi

        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand HomeCommand { get; set; }
        public ICommand OpenListContractUCCommand { get; set; }
        public ICommand OpenListLsxOfProductUCCommand { get; set; }
        public ICommand OpenListFileDesignCommand { get; set; }
        public ICommand OpenListFileHoSoCommand { get; set; }
        public ICommand OpenListApprovalDocumentProductCommand { get; set; }
        public ICommand OpenListSendedDocumentCommand { get; set; }
        public ICommand OpenListRevokedDocumentCommand { get; set; }
        #endregion
        internal ProductViewModel(Product product)
        {
            _ServiceFactory = new ServiceFactory();
            Product = product;
            if (product.ProductTypeNewId != null) ProductTypeNew = _ServiceFactory.GetProductTypeNew(product.ProductTypeNewId.Value);
            _IsFirtLoad = true; //Khoi tao lan dau
            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (_IsFirtLoad)
                {
                    if (_ProductTypeNew.TypeCode == "PowerTransformer" || _ProductTypeNew.TypeCode == "DistributionTransformer")
                    {
                        TransformerTDViewModel transformerTDViewModel = new TransformerTDViewModel(product);
                        TransformerTDUC transformerTDUC = new TransformerTDUC() { DataContext = transformerTDViewModel };
                        LoadUC = transformerTDUC;
                    }
                }
                _IsFirtLoad = false;
            });
            HomeCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (_ProductTypeNew.TypeCode == "PowerTransformer" || _ProductTypeNew.TypeCode == "DistributionTransformer")
                {
                    TransformerTDViewModel transformerTDViewModel = new TransformerTDViewModel(product);
                    TransformerTDUC transformerTDUC = new TransformerTDUC() { DataContext = transformerTDViewModel };
                    LoadUC = transformerTDUC;
                }
            });
            OpenListContractUCCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ListContractViewModel listContractViewModel = new ListContractViewModel(_Product);
                ListContractUC listConstractUC = new ListContractUC() { DataContext = listContractViewModel };
                LoadUC = listConstractUC;
            });
            OpenListLsxOfProductUCCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ListLsxOfProductUC view = new ListLsxOfProductUC();
                ListLsxOfProductViewModel vm = new ListLsxOfProductViewModel(_Product.Id);
                view.DataContext = vm;
                LoadUC = view;
            });
            OpenListFileDesignCommand = new RelayCommand<DevExpress.Xpf.NavBar.NavBarItem>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                //int typeFilePdf;
                //switch (p.Name)
                //{
                //    case "BtnBvDien":
                //        typeFilePdf = 1;
                //        break;
                //    case "BtnBvCo":
                //        typeFilePdf = 2;
                //        break;
                //    case "BtnBvNt":
                //        typeFilePdf = 3;
                //        break;
                //    default:
                //        typeFilePdf = 1;
                //        break;
                //}
                //ListFileScanViewModel vm = new ListFileScanViewModel(_Product) { TypeFilePdf = typeFilePdf };
                //ListFileScanUC listFileScanUC = new ListFileScanUC() { DataContext = vm };
                //LoadUC = listFileScanUC;
            });
            OpenListFileHoSoCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ListTransformerManualViewModel listFileHoSoViewModel = new ListTransformerManualViewModel(_Product);
                ListTransformerManualUC listFileHoSoUC = new ListTransformerManualUC() { DataContext = listFileHoSoViewModel };
                LoadUC = listFileHoSoUC;
            });
            OpenListApprovalDocumentProductCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ListApprovalDocumentOfProductViewModel listApprovalDocumentOfProductViewModel = new ListApprovalDocumentOfProductViewModel(_Product);
                ListApprovalDocumentOfProductUC listApprovalDocumentOfProductUC = new ListApprovalDocumentOfProductUC() { DataContext = listApprovalDocumentOfProductViewModel };
                LoadUC = listApprovalDocumentOfProductUC;
            });
            OpenListSendedDocumentCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                DocumentSendedViewModel documentSendedViewModel = new DocumentSendedViewModel(Product.Id);
                DocumentSendedUC documentSendedUC = new DocumentSendedUC() { DataContext = documentSendedViewModel };
                LoadUC = documentSendedUC;
            });
            OpenListRevokedDocumentCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                DocumentSendedViewModel documentSendedViewModel = new DocumentSendedViewModel(Product.Id);
                DocumentSendedUC documentSendedUC = new DocumentSendedUC() { DataContext = documentSendedViewModel };
                LoadUC = documentSendedUC;
            });
        }
    }
}
