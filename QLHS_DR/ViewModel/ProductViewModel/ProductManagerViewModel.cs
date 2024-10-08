﻿using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.ProductView;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    class ProductManagerViewModel : BaseViewModel
    {
        private MessageServiceClient _Proxy;
        private ServiceFactory _ServiceFactory;

        private bool _IsVisibleRatedPowerKeyWord;
        public bool IsVisibleRatedPowerKeyWord
        {
            get => _IsVisibleRatedPowerKeyWord;
            set
            {
                _IsVisibleRatedPowerKeyWord = value;
                OnPropertyChanged("IsVisibleRatedPowerKeyWord");
            }
        }
        private bool _IsVisibleRatedVoltageKeyWordd;
        public bool IsVisibleRatedVoltageKeyWord
        {
            get => _IsVisibleRatedVoltageKeyWordd;
            set
            {
                _IsVisibleRatedVoltageKeyWordd = value;
                OnPropertyChanged("IsVisibleRatedVoltageKeyWord");
            }
        }
        private bool _IsVisibleStandardsSelection;
        public bool IsVisibleStandardsSelection
        {
            get => _IsVisibleStandardsSelection;
            set
            {
                _IsVisibleStandardsSelection = value;
                OnPropertyChanged("IsVisibleStandardsSelection");
            }
        }
        private bool _IsVisibleTankType;
        public bool IsVisibleTankType
        {
            get => _IsVisibleTankType;
            set
            {
                _IsVisibleTankType = value;
                OnPropertyChanged("IsVisibleTankType");
            }
        }
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
        private string _TankType;
        public string TankType
        {
            get => _TankType;
            set
            {
                if (_TankType != value)
                {
                    _TankType = value;
                    OnPropertyChanged("TankType");
                }
            }
        }
        private string _SaleOrderNumber;
        public string SaleOrderNumber
        {
            get => _SaleOrderNumber;
            set
            {
                _SaleOrderNumber = value;
                OnPropertyChanged("SaleOrderNumber");
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
        private string _ProductNameKeyword;
        public string ProductNameKeyword
        {
            get => _ProductNameKeyword;
            set
            {
                _ProductNameKeyword = value;
                OnPropertyChanged("ProductNameKeyword");
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
        private ChatAppServiceReference.Standard _SelectedStandard;
        public ChatAppServiceReference.Standard SelectedStandard
        {
            get => _SelectedStandard;
            set
            {
                _SelectedStandard = value;
                OnPropertyChanged("StandardSelected");
            }
        }
        private ObservableCollection<Product> _Products;
        public ObservableCollection<Product> Products
        {
            get => _Products;
            set
            {
                _Products = value; OnPropertyChanged("Products");
            }
        }
        private ObservableCollection<ChatAppServiceReference.Standard> _Standards;
        public ObservableCollection<ChatAppServiceReference.Standard> Standards
        {
            get => _Standards;
            set
            {
                _Standards = value; OnPropertyChanged("Standards");
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
        private ProductTypeNew _SelectedProductTypeNew;
        public ProductTypeNew SelectedProductTypeNew
        {
            get => _SelectedProductTypeNew;
            set
            {
                if (_SelectedProductTypeNew != value)
                {
                    _SelectedProductTypeNew = value;

                    if (_SelectedProductTypeNew != null)
                    {
                        switch (_SelectedProductTypeNew.TypeCode)
                        {
                            case "General":
                                IsVisibleRatedPowerKeyWord = false;
                                IsVisibleRatedVoltageKeyWord = false;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                            case "OutdoorOilImmersedCurrentTransformer":
                                IsVisibleRatedPowerKeyWord = false;
                                IsVisibleRatedVoltageKeyWord = false;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                            case "PowerTransformer":
                                IsVisibleRatedPowerKeyWord = true;
                                IsVisibleRatedVoltageKeyWord = true;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                            case "DistributionTransformer":
                                IsVisibleRatedPowerKeyWord = true;
                                IsVisibleRatedVoltageKeyWord = true;
                                IsVisibleStandardsSelection = true;
                                IsVisibleTankType = true;
                                break;
                            case "BushingCurrentTransformer":
                                IsVisibleRatedPowerKeyWord = false;
                                IsVisibleRatedVoltageKeyWord = false;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                            case "OutdoorOilImmersedInductiveVoltageTransformer":
                                IsVisibleRatedPowerKeyWord = false;
                                IsVisibleRatedVoltageKeyWord = false;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                            case "OutdoorOilImmersedCapacitorVoltageTransformer":
                                IsVisibleRatedPowerKeyWord = false;
                                IsVisibleRatedVoltageKeyWord = false;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                            case "MOF1":
                                IsVisibleRatedPowerKeyWord = false;
                                IsVisibleRatedVoltageKeyWord = false;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                            case "MOF3":
                                IsVisibleRatedPowerKeyWord = false;
                                IsVisibleRatedVoltageKeyWord = false;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                            case "BA":
                                IsVisibleRatedPowerKeyWord = false;
                                IsVisibleRatedVoltageKeyWord = false;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                            default:
                                IsVisibleRatedPowerKeyWord = false;
                                IsVisibleRatedVoltageKeyWord = false;
                                IsVisibleStandardsSelection = false;
                                IsVisibleTankType = false;
                                break;
                        }
                    }
                    OnPropertyChanged("SelectedProductTypeNew");
                }
            }
        }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand LockProductCommand { get; set; }
        public ICommand UnLockProductCommand { get; set; }
        public ICommand RemoveProductCommand { get; set; }
        public ICommand OpenProductCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ProductManagerViewModel()
        {
            try
            {
                _ServiceFactory = new ServiceFactory();
                _Proxy = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _Proxy.Open();
                Standards = _Proxy.LoadStandards().ToObservableCollection();
                SelectedStandard = Standards.Where(x => x.Name == "NONE").FirstOrDefault();

                _ProductTypeNews = _ServiceFactory.LoadProducTypeNews();
                _ProductTypeNews.Add(new ProductTypeNew() { Id = 0, TypeCode = "All", TypeName = "All" });
                ProductTypeNews = _ProductTypeNews.OrderBy(x => x.Id).ToObservableCollection();
                SelectedProductTypeNew = _ProductTypeNews.Where(x => x.TypeCode == "All").FirstOrDefault();

                _Proxy.Close();
            }
            catch (Exception ex)
            {
                _Proxy.Abort();
                MessageBox.Show(ex.Message);
            }
            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
               
            });
            RemoveProductCommand = new RelayCommand<Product>((p) => { if (SectionLogin.Ins.CanRemoveProduct) return true; else return false; }, (p) =>
            {
                try
                {
                    if (MessageBox.Show("Bạn có muốn xóa sản phẩm mã số: " + p.ProductCode + " ?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        _Proxy = ServiceHelper.NewMessageServiceClient();
                        _Proxy.Open();
                        if (_Proxy.RemoveProduct(p.Id))
                        {
                            MessageBox.Show("Xóa thành công");
                            if (Products.Contains(p)) Products.Remove(p);
                        }
                        else
                        {
                            MessageBox.Show("Thao tác thất bại, vui lòng thử lại");
                        }
                        _Proxy.Close();
                    }
                }
                catch (Exception ex)
                {
                    _Proxy.Abort();
                    MessageBox.Show(ex.Message);
                }
            });
            ClearCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CodeKeyWord = null;
                YearCreateKeyWord = 0;
                RatedPowerKeyWord = null;
                RatedVoltageKeyWord = null;
                NoteKeyWord = null;
            });
            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Products = SearchProducts();
            });

            OpenProductCommand = new RelayCommand<DependencyObject>((p) => { if (_SelectedProduct != null) return true; else return false; }, (p) =>
            {
                bool canOpen = false;
                FrameworkElement window = System.Windows.Window.GetWindow(p);
                MainViewModel dataOfMainWindow = new MainViewModel();
                dataOfMainWindow = (MainViewModel)window.DataContext;
                TransformerDTO transformerDTO = new TransformerDTO();
                TabContainer item = new TabContainer();
                ProductTypeNew productTypeNew = _ServiceFactory.GetProductTypeNew(_SelectedProduct.ProductTypeNewId.Value);
                switch (productTypeNew.TypeCode)
                {
                    case "General":
                        canOpen = true;
                        break;
                    case "OutdoorOilImmersedCurrentTransformer":
                        if (SectionLogin.Ins.CanOpenInstrumentTransformer) canOpen = true;
                        break;
                    case "PowerTransformer":
                        if (SectionLogin.Ins.CanOpenPowerTransformer) canOpen = true;
                        break;
                    case "DistributionTransformer":
                        if (SectionLogin.Ins.CanOpenDistributionTransformer) canOpen = true;
                        break;
                    case "BushingCurrentTransformer":
                        if (SectionLogin.Ins.CanOpenInstrumentTransformer) canOpen = true;
                        break;
                    case "OutdoorOilImmersedInductiveVoltageTransformer":
                        if (SectionLogin.Ins.CanOpenInstrumentTransformer) canOpen = true;
                        break;
                    case "OutdoorOilImmersedCapacitorVoltageTransformer":
                        if (SectionLogin.Ins.CanOpenInstrumentTransformer) canOpen = true;
                        break;
                    case "MOF1":
                        if (SectionLogin.Ins.CanOpenInstrumentTransformer) canOpen = true;
                        break;
                    case "MOF3":
                        if (SectionLogin.Ins.CanOpenInstrumentTransformer) canOpen = true;
                        break;
                    case "BA":
                        if (SectionLogin.Ins.CanOpenInstrumentTransformer) canOpen = true;
                        break;
                    default:
                        break;
                }
                if (canOpen)
                {
                    ProductViewModel tabVM = new ProductViewModel(_SelectedProduct);
                    ProductUC tabView = new ProductUC();
                    tabView.DataContext = tabVM;
                    item = dataOfMainWindow.Workspaces.Where(x => x.Header == _SelectedProduct.ProductCode).FirstOrDefault();
                    if (item != null)
                    {
                        item.IsVisible = true;
                        item.IsSelected = true;
                        item.Content = tabView;
                        item.AllowHide = "true";
                    }
                    else
                    {
                        TabContainer tabContainer = new TabContainer()
                        {
                            Header = _SelectedProduct.ProductCode,
                            IsSelected = true,
                            IsVisible = true,
                            Content = tabView,
                            AllowHide = "true"
                        };
                        dataOfMainWindow.Workspaces.Add(tabContainer);
                    }
                }
                else MessageBox.Show("Bạn chưa được cấp quyền xem sản phẩm này.");
            });

            LockProductCommand = new RelayCommand<Product>((p) => { if (SectionLogin.Ins.CanLockProduct && p != null && p.IsLocked == false) return true; else return false; }, (p) =>
            {
                try
                {
                    if (MessageBox.Show("Bạn có muốn khóa máy biến áp mã số: " + p.ProductCode + " ?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        _Proxy = ServiceHelper.NewMessageServiceClient();
                        _Proxy.Open();
                        if (_Proxy.LockProduct(p.Id, true))
                        {
                            MessageBox.Show("Khóa thành công");
                            p.IsLocked = true;
                        }
                        else
                        {
                            MessageBox.Show("Thao tác thất bại, vui lòng thử lại");
                        }
                        _Proxy.Close();
                    }
                }
                catch (Exception ex)
                {
                    _Proxy.Abort();
                    MessageBox.Show(ex.Message);
                }
            });
            UnLockProductCommand = new RelayCommand<Product>((p) => { if (SectionLogin.Ins.CanLockProduct && p != null && p.IsLocked == true) return true; else return false; }, (p) =>
            {
                try
                {
                    if (MessageBox.Show("Bạn có muốn mở khóa máy biến áp mã số: " + p.ProductCode + " ?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        _Proxy = ServiceHelper.NewMessageServiceClient();
                        _Proxy.Open();
                        if (_Proxy.LockProduct(p.Id, false))
                        {
                            MessageBox.Show("Mở khóa thành công");
                            p.IsLocked = false;
                        }
                        else
                        {
                            MessageBox.Show("Thao tác thất bại, vui lòng thử lại");
                        }
                        _Proxy.Close();
                    }
                }
                catch (Exception ex)
                {
                    _Proxy.Abort();
                    MessageBox.Show(ex.Message);
                }
            });
        }
        private ObservableCollection<Product> SearchProducts()
        {
            ObservableCollection<Product> ketqua = new ObservableCollection<Product>();
            try
            {
                _Proxy = ServiceHelper.NewMessageServiceClient();
                _Proxy.Open();
                 
                switch (_SelectedProductTypeNew.TypeCode)
                {
                    case "General":
                        if (!String.IsNullOrEmpty(_SaleOrderNumber))
                            ketqua = _Proxy.SearchAllProductsWithSO(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id, _SaleOrderNumber).ToObservableCollection();
                        else
                            ketqua = _Proxy.SearchAllProducts(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id).ToObservableCollection();
                        break;
                    case "OutdoorOilImmersedCurrentTransformer":
                        if (!String.IsNullOrEmpty(_SaleOrderNumber))
                            ketqua = _Proxy.SearchAllProductsWithSO(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id, _SaleOrderNumber).ToObservableCollection();
                        else
                            ketqua = _Proxy.SearchAllProducts(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id).ToObservableCollection();

                        break;
                    case "PowerTransformer":
                        if (!String.IsNullOrEmpty(_SaleOrderNumber))
                            ketqua = _Proxy.SearchTransformersWithSO(_CodeKeyWord, _RatedPowerKeyWord, _RatedVoltageKeyWord, _YearCreateKeyWord, _NoteKeyWord, _StationKeyWord, _SelectedProductTypeNew.Id, _SelectedStandard.Id, _SaleOrderNumber).ToObservableCollection();
                        else
                            ketqua = _Proxy.SearchTransformers(_CodeKeyWord, _RatedPowerKeyWord, _RatedVoltageKeyWord, _YearCreateKeyWord, _NoteKeyWord, _StationKeyWord, _SelectedProductTypeNew.Id, _SelectedStandard.Id).ToObservableCollection();
                        break;
                    case "DistributionTransformer":
                        ketqua = _Proxy.SearchDistributionTransformers(_CodeKeyWord, _RatedPowerKeyWord, _RatedVoltageKeyWord, _YearCreateKeyWord, _NoteKeyWord, _StationKeyWord, _SelectedProductTypeNew.Id, _SelectedStandard.Id, _SaleOrderNumber, _TankType).ToObservableCollection();
                        break;
                    case "BushingCurrentTransformer":
                        if (!String.IsNullOrEmpty(_SaleOrderNumber))
                            ketqua = _Proxy.SearchAllProductsWithSO(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id, _SaleOrderNumber).ToObservableCollection();
                        else
                            ketqua = _Proxy.SearchAllProducts(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id).ToObservableCollection();
                        break;
                    case "OutdoorOilImmersedInductiveVoltageTransformer":
                        ketqua = _Proxy.SearchAllProducts(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id).ToObservableCollection();

                        break;
                    case "OutdoorOilImmersedCapacitorVoltageTransformer":
                        if (!String.IsNullOrEmpty(_SaleOrderNumber))
                            ketqua = _Proxy.SearchAllProductsWithSO(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id, _SaleOrderNumber).ToObservableCollection();
                        else
                            ketqua = _Proxy.SearchAllProducts(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id).ToObservableCollection();
                        break;
                    case "MOF1":
                        if (!String.IsNullOrEmpty(_SaleOrderNumber))
                            ketqua = _Proxy.SearchAllProductsWithSO(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id, _SaleOrderNumber).ToObservableCollection();
                        else
                            ketqua = _Proxy.SearchAllProducts(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id).ToObservableCollection();
                        break;
                    case "MOF3":
                        if (!String.IsNullOrEmpty(_SaleOrderNumber))
                            ketqua = _Proxy.SearchAllProductsWithSO(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id, _SaleOrderNumber).ToObservableCollection();
                        else
                            ketqua = _Proxy.SearchAllProducts(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id).ToObservableCollection();
                        break;
                    case "BA":
                        if (!String.IsNullOrEmpty(_SaleOrderNumber))
                            ketqua = _Proxy.SearchAllProductsWithSO(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id, _SaleOrderNumber).ToObservableCollection();
                        else
                            ketqua = _Proxy.SearchAllProducts(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id).ToObservableCollection();
                        break;
                    default:
                        if (!String.IsNullOrEmpty(_SaleOrderNumber))
                            ketqua = _Proxy.SearchAllProductsWithSO(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id, _SaleOrderNumber).ToObservableCollection();
                        else
                            ketqua = _Proxy.SearchAllProducts(_CodeKeyWord, _YearCreateKeyWord, _NoteKeyWord, _ProductNameKeyword, _SelectedProductTypeNew.Id).ToObservableCollection();
                        break;
                }
                _Proxy.Close();
            }

            catch (Exception ex)
            {
                _Proxy.Abort();
                MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
    }
}
