﻿using DevExpress.Data.Utils;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using Prism.Events;
using QLHS_DR.Core;
using QLHS_DR.EOfficeServiceReference;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using DevExpress.Mvvm.POCO;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    class TransformerManagerViewModel : BaseViewModel
    {
        IEofficeMainService _Client;
        private string _CodeKeyWord;
        public string CodeKeyWord
        {
            get => _CodeKeyWord;
            set
            {
                _CodeKeyWord = value;
                OnPropertyChanged("CodeKeyWord");
            }
        }
        private string _RatedPowerKeyWord;
        public string RatedPowerKeyWord
        {
            get => _RatedPowerKeyWord;
            set
            {
                _RatedPowerKeyWord = value;
                OnPropertyChanged("RatedPowerKeyWord");
            }
        }
        private string _RatedVoltageKeyWord;
        public string RatedVoltageKeyWord
        {
            get => _RatedVoltageKeyWord;
            set
            {
                _RatedVoltageKeyWord = value;
                OnPropertyChanged("RatedVoltageKeyWord");
            }
        }
        private int _YearCreateKeyWord;
        public int YearCreateKeyWord
        {
            get => _YearCreateKeyWord;
            set
            {
                _YearCreateKeyWord = value;
                OnPropertyChanged("YearCreateKeyWord");
            }
        }
        private string _NoteKeyWord;
        public string NoteKeyWord
        {
            get => _NoteKeyWord;
            set
            {
                _NoteKeyWord = value;
                OnPropertyChanged("NoteKeyWord");
            }
        }
        private string _StationKeyWord;
        public string StationKeyWord
        {
            get => _StationKeyWord;
            set
            {
                _StationKeyWord = value;
                OnPropertyChanged("StationKeyWord");
            }
        }
        private ProductTypeWrapper _SelectedProductTypeWrapper;
        public ProductTypeWrapper SelectedProductTypeWrapper
        {
            get => _SelectedProductTypeWrapper;
            set
            {
                _SelectedProductTypeWrapper = value;
                OnPropertyChanged("SelectedProductTypeWrapper");
            }
        }
        private Product _SelectedProduct;
        public Product SelectedProduct
        {
            get => _SelectedProduct;
            set
            {
                _SelectedProduct = value;
                OnPropertyChanged("SelectedProduct");
            }
        }
        private QLHS_DR.EOfficeServiceReference.Standard _SelectedStandard;
        public QLHS_DR.EOfficeServiceReference.Standard SelectedStandard
        {
            get => _SelectedStandard;
            set
            {
                _SelectedStandard = value;
                OnPropertyChanged("StandardSelected");
            }
        }
        private ObservableCollection<TransformerDTO> _Products;
        public ObservableCollection<TransformerDTO> Products
        {
            get => _Products;
            set
            {
                _Products = value; OnPropertyChanged("Products");
            }
        }
        private ObservableCollection<QLHS_DR.EOfficeServiceReference.Standard> _Standards;
        public ObservableCollection<QLHS_DR.EOfficeServiceReference.Standard> Standards
        {
            get => _Standards;
            set
            {
                _Standards = value; OnPropertyChanged("Standards");
            }
        }
        private ObservableCollection<ProductTypeWrapper> _ProductTypeWrappers;
        public ObservableCollection<ProductTypeWrapper> ProductTypeWrappers
        {
            get => _ProductTypeWrappers;
            set
            {
                _ProductTypeWrappers = value; OnPropertyChanged("ProductTypeWrappers");
            }
        }

        public ICommand LoadedWindowCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        public ICommand LockProductCommand { get; set; }
        public ICommand RemoveProductCommand { get; set; }
        public ICommand OpenProductCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public TransformerManagerViewModel(IEventAggregator eventAggregator)
        {
            try
            {
                CertificateValidation();
                ProductTypeWrappers = new ObservableCollection<ProductTypeWrapper>()
                {
                   new ProductTypeWrapper(ProductType.PowerTransformer),
                   new ProductTypeWrapper(ProductType.DistributionTransformer)
                };
                SelectedProductTypeWrapper = ProductTypeWrappers[0];
                _Client = App.Container.GetService<IEofficeMainService>();
                if(_Client==null)
                {
                    _Client = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    App.Container.AddService(typeof(IEofficeMainService), _Client); //save client to Container
                }                
                Standards = _Client.LoadStandards().ToObservableCollection();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
            ClearCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CodeKeyWord = null;
                YearCreateKeyWord = 0;
                RatedPowerKeyWord = null;
                RatedVoltageKeyWord = null;
                NoteKeyWord = null;
            });
            SearchCommand = new RelayCommand<object>((p) => { return true;}, (p) =>
            {
                Products = SearchTransformer();
            });
            LoadedWindowCommand = new RelayCommand<object>((p) => {return true;}, (p) =>
            {
               // Products = SearchProducts();
            });
            //OpenProductCommand = new RelayCommand<DependencyObject>((p) => { if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productOpenPowerTransformer")) return true; else return false; }, (p) =>
            //{
            //    FrameworkElement window = System.Windows.Window.GetWindow(p);
            //    MainViewModel dataOfMainWindow = new MainViewModel();
            //    dataOfMainWindow = (MainViewModel)window.DataContext;

            //    DetailProductViewModel tabDetailProductVM = new DetailProductViewModel
            //    {
            //        Transformer = _SelectedProduct
            //    };
            //    VIEW.Products.DetailProductUC detailPowertransformerUC = new VIEW.Products.DetailProductUC();
            //    detailPowertransformerUC.DataContext = tabDetailProductVM;

            //    var item = dataOfMainWindow.Workspaces.Where(x => x.Header == _SelectedProduct.Code).FirstOrDefault();
            //    if (item != null) //Kiểm tra xem đã tồn tại tab có mã số trên chưa
            //    {
            //        item.IsVisible = true;
            //        item.IsSelected = true;
            //        item.Content = detailPowertransformerUC;
            //    }
            //    else
            //    {
            //        TabContainer detailPowertransformer = new TabContainer()
            //        {
            //            Header = _SelectedProduct.Code,
            //            IsSelected = true,
            //            IsVisible = true,
            //            Content = detailPowertransformerUC
            //        };
            //        dataOfMainWindow.Workspaces.Add(detailPowertransformer);
            //    }
            //});
            //EditProductCommand = new RelayCommand<PowerTransformer>((p) => { if (p != null && SectionLogin.Ins.CurrentPermission._Permission["productEditProduct"] && SectionLogin.Ins.CurrentPermission._Permission.ContainsKey("productEditProduct")) return true; else return false; }, (p) =>
            //{
            //    ViewModel.Products.EditProductViewModel editProductViewModel = new EditProductViewModel(p);
            //    VIEW.Products.EditProductWindow editProductWindow = new VIEW.Products.EditProductWindow() { DataContext = editProductViewModel };
            //    editProductWindow.ShowDialog();
            //});
            //RemoveProductCommand = new RelayCommand<PowerTransformer>((p) => { if (p != null && SectionLogin.Ins.CurrentPermission._Permission["productDelProduct"] == true && SectionLogin.Ins.CurrentPermission._Permission.ContainsKey("productDelProduct")) return true; else return false; }, (p) =>
            //{
            //    if (MessageBox.Show("Bạn có muốn xóa máy biến áp mã số: " + p.Code + " ?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            //    {
            //        var powertrans = DataProvider.Ins.DB.PowerTransformers.Where(x => x.TransformerID == p.TransformerID).FirstOrDefault();
            //        if (powertrans != null)
            //            DataProvider.Ins.DB.PowerTransformers.Remove(powertrans);
            //        if (DataProvider.Ins.DB.SaveChanges() > 0)
            //        {
            //            MessageBox.Show("Xóa thành công");
            //            if (Products.Contains(p)) Products.Remove(p);
            //        }
            //        else
            //        {
            //            MessageBox.Show("Thao tác thất bại, vui lòng thử lại");
            //            DataProvider.Ins.Refresh();
            //        }
            //    }
            //});
            //LockProductCommand = new RelayCommand<PowerTransformer>((p) => { if (p != null && SectionLogin.Ins.CurrentPermission._Permission["fileLockFile"] && SectionLogin.Ins.CurrentPermission._Permission.ContainsKey("fileLockFile")) return true; else return false; }, (p) =>
            //{
            //    if (MessageBox.Show("Bạn có muốn khóa máy biến áp mã số: " + p.Code + " ?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            //    {
            //        var powertrans = DataProvider.Ins.DB.PowerTransformers.Where(x => x.TransformerID == p.TransformerID).FirstOrDefault();

            //        if (powertrans != null)
            //            powertrans.Lock = 1;
            //        if (DataProvider.Ins.DB.SaveChanges() > 0)
            //        {
            //            MessageBox.Show("Khóa thành công");
            //            if (Products.Contains(p)) Products.Remove(p);
            //        }
            //        else
            //        {
            //            MessageBox.Show("Thao tác thất bại, vui lòng thử lại");
            //            DataProvider.Ins.Refresh();
            //        }
            //    }
            //});

        }

        private ObservableCollection<TransformerDTO> SearchTransformer()
        {
            ObservableCollection<TransformerDTO> ketqua = new ObservableCollection<TransformerDTO>();
            try
            {    
                if(_Client== null)
                {
                    _Client = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    App.Container.AddService(typeof(IEofficeMainService), _Client); //save client to Container
                }                
                ketqua = _Client.SearchTransformer(_CodeKeyWord, _RatedPowerKeyWord, _RatedVoltageKeyWord, _YearCreateKeyWord, _NoteKeyWord, _StationKeyWord, _SelectedProductTypeWrapper?.EnumValue ?? 0, _SelectedStandard?.Id ?? 0).ToObservableCollection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);                
            }
            return ketqua;
        }
        private void CertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(IsSSL));
        }
        private static bool IsSSL(object A_0, X509Certificate A_1, X509Chain A_2, SslPolicyErrors A_3)
        {
            return true;
        }

    }
}
