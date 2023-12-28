using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.Products
{
    class NewProductViewModel : BaseViewModel, IDataErrorInfo
    {
        #region "Properties and Filed"
        readonly ServiceFactory _ServiceFactory;
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set
            {
                _IsBusy = value; OnPropertyChanged("IsBusy");
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
                    OnPropertyChanged("SelectedProductTypeNew");
                    if (_ProductCodes.Count > 0 && _SelectedProductTypeNew != null)
                    {
                        AllowNext = true;
                    }
                    else AllowNext = false;
                    switch (_SelectedProductTypeNew.TypeCode)
                    {
                        case "General":
                            IsTransformer = false;
                            ShowFinish1 = true;
                            ShowNext = false;
                            break;

                        case "OutdoorOilImmersedCurrentTransformer":
                            IsTransformer = false;
                            ShowFinish1 = true;
                            ShowNext = false;
                            break;
                        case "PowerTransformer":
                            IsTransformer = true;
                            ShowFinish1 = false;
                            ShowNext = true;
                            UnitPower = "MVA";
                            CoolingMethod = "ONAN/ONAF";
                            break;
                        case "DistributionTransformer":
                            IsTransformer = true;
                            ShowFinish1 = false;
                            ShowNext = true;
                            UnitPower = "kVA";
                            CoolingMethod = "ONAN";
                            break;
                        case "BushingCurrentTransformer":
                            IsTransformer = false;
                            ShowFinish1 = true;
                            ShowNext = false;
                            break;
                        case "OutdoorOilImmersedInductiveVoltageTransformer":
                            IsTransformer = false;
                            ShowFinish1 = true;
                            ShowNext = false;
                            break;
                        case "OutdoorOilImmersedCapacitorVoltageTransformer":
                            IsTransformer = false;
                            ShowFinish1 = true;
                            ShowNext = false;
                            break;
                        case "MOF1":
                            IsTransformer = false;
                            ShowFinish1 = true;
                            ShowNext = false;
                            break;
                        case "MOF3":
                            IsTransformer = false;
                            ShowFinish1 = true;
                            ShowNext = false;
                            break;
                        case "BA":
                            IsTransformer = true;
                            UnitPower = "VA";
                            CoolingMethod = "ONAN";
                            ShowFinish1 = false;
                            ShowNext = true;
                            break;
                        default:
                            IsTransformer = false;
                            ShowFinish1 = false;
                            ShowNext = false;
                            break;
                    }
                }
            }
        }
        private QLHS_DR.ChatAppServiceReference.Standard _SelectedStandard;
        public QLHS_DR.ChatAppServiceReference.Standard SelectedStandard
        {
            get => _SelectedStandard;
            set
            {
                if (_SelectedStandard != value)
                {
                    _SelectedStandard = value;
                    OnPropertyChanged("SelectedStandard");
                }
            }
        }
        private ObservableCollection<QLHS_DR.ChatAppServiceReference.Standard> _ListStandards;
        public ObservableCollection<QLHS_DR.ChatAppServiceReference.Standard> ListStandards
        {
            get => _ListStandards;
            set
            {
                if (_ListStandards != value)
                {
                    _ListStandards = value;
                    OnPropertyChanged("ListStandards");
                }
            }
        }
        private string _ConnectionSymbol;
        public string ConnectionSymbol
        {
            get => _ConnectionSymbol;
            set
            {
                if (_ConnectionSymbol != value)
                {
                    _ConnectionSymbol = value;
                    OnPropertyChanged("_ConnectionSymbol");
                }
            }
        }
        private string _UnitPower;
        public string UnitPower
        {
            get => _UnitPower;
            set
            {
                if (_UnitPower != value)
                {
                    _UnitPower = value;
                    OnPropertyChanged("UnitPower");
                }
            }
        }
        private string _RatedPower;
        public string RatedPower
        {
            get => _RatedPower;
            set
            {
                if (_RatedPower != value)
                {
                    _RatedPower = value;
                    OnPropertyChanged("RatedPower");
                }
            }
        }
        private int _NumberOfPhase;
        public int NumberOfPhase
        {
            get => _NumberOfPhase;
            set
            {
                if (_NumberOfPhase != value)
                {
                    _NumberOfPhase = value;
                    OnPropertyChanged("NumberOfPhase");
                }
            }
        }
        private string _CoolingMethod;
        public string CoolingMethod
        {
            get => _CoolingMethod;
            set
            {
                if (_CoolingMethod != value)
                {
                    _CoolingMethod = value;
                    OnPropertyChanged("CoolingMethod");
                }
            }
        }

        private string _Code;
        public string Code
        {
            get => _Code;
            set
            {
                if (_Code != value)
                {
                    _Code = value;
                    ProductCodes.Clear();
                    try
                    {
                        if (_Code.StartsWith("BA"))
                        {
                            SelectedProductTypeNew = _ProductTypeNews.Where(x => x.TypeCode == "BA").FirstOrDefault();

                            if (_Code.StartsWith("BA2")) NumberOfPhase = 2; else if (_Code.StartsWith("BA1")) NumberOfPhase = 1;
                        }
                        else if (_Code.StartsWith("BU"))
                        {
                            SelectedProductTypeNew = _ProductTypeNews.Where(x => x.TypeCode == "OutdoorOilImmersedInductiveVoltageTransformer").FirstOrDefault();
                            if (_Code.StartsWith("BU1")) NumberOfPhase = 1;
                        }
                        else if (_Code.StartsWith("BI"))
                        {
                            SelectedProductTypeNew = _ProductTypeNews.Where(x => x.TypeCode == "OutdoorOilImmersedCurrentTransformer").FirstOrDefault();
                            NumberOfPhase = 1;
                        }
                        else if (_Code.StartsWith("MOF1"))
                        {
                            SelectedProductTypeNew = _ProductTypeNews.Where(x => x.TypeCode == "MOF1").FirstOrDefault();
                            NumberOfPhase = 1;
                        }
                        else if (_Code.StartsWith("MOF3"))
                        {
                            SelectedProductTypeNew = _ProductTypeNews.Where(x => x.TypeCode == "MOF3").FirstOrDefault();
                            NumberOfPhase = 3;
                        }

                        List<string> singleCodes = DocScan.GetProductCodeSingle(value); // Lấy về tập hợp các mã số có trong mã số đầy đủ
                        if (singleCodes != null)
                        {
                            foreach (var code in singleCodes)
                            {
                                var trans = _ServiceFactory.GetProductByProductCode(code);

                                if (trans == null && !_ProductCodes.Contains(code))
                                {
                                    ProductCodes.Add(code);
                                }
                            }
                        }
                        if (_ProductCodes.Count > 0 && _SelectedProductTypeNew != null)
                        {
                            AllowNext = true;
                            AllowFinish1 = true;
                        }
                        else
                        {
                            AllowNext = false;
                            AllowFinish1 = false;
                        }
                        OnPropertyChanged("ProductCodes");
                        OnPropertyChanged("Code");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
        }

        private int? _YearCreate;
        public int? YearCreate
        {
            get => _YearCreate;
            set
            {
                if (_YearCreate != value)
                {
                    _YearCreate = value;
                    OnPropertyChanged("YearCreate");
                }
            }
        }
        private string _Note;
        public string Note
        {
            get => _Note;
            set
            {
                if (_Note != value)
                {
                    _Note = value;
                    OnPropertyChanged("Note");
                }
            }
        }
        //Parameter of BA


        private string _ErrorMessage;
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set
            {
                if (_ErrorMessage != value)
                {
                    _ErrorMessage = value;
                    OnPropertyChanged("ErrorMessage");
                }
            }
        }
        private string _RatedFrequency;
        public string RatedFrequency
        {
            get => _RatedFrequency;
            set
            {
                if (_RatedFrequency != value)
                {
                    _RatedFrequency = value;
                    OnPropertyChanged("RatedFrequency");
                }
            }
        }

        private ObservableCollection<string> _ProductCodes;
        public ObservableCollection<string> ProductCodes
        {
            get => _ProductCodes;
            set
            {
                if (_ProductCodes != value)
                {
                    _ProductCodes = value;
                    if (_ProductCodes.Count > 0 && _SelectedProductTypeNew != null)
                    {
                        AllowNext = true;
                    }
                    else AllowNext = false;
                    OnPropertyChanged("ProductCodes");
                }
            }
        }
        private string _RatedVoltage;
        public string RatedVoltage
        {
            get => _RatedVoltage;
            set
            {
                if (_RatedVoltage != value)
                {
                    _RatedVoltage = value;
                    OnPropertyChanged("RatedVoltage");
                }
            }
        }
        private string _VoltageUnit;
        public string VoltageUnit
        {
            get => _VoltageUnit;
            set
            {
                if (_VoltageUnit != value)
                {
                    _VoltageUnit = value;
                    OnPropertyChanged("VoltageUnit");
                }
            }
        }
        private string _Station;
        public string Station
        {
            get => _Station;
            set
            {
                if (_Station != value)
                {
                    _Station = value;
                    OnPropertyChanged("Station");
                }
            }
        }
        private bool _IsTransformer;
        public bool IsTransformer
        {
            get => _IsTransformer;
            set
            {
                if (_IsTransformer != value)
                {
                    _IsTransformer = value;
                    OnPropertyChanged("IsTransformer");
                }
            }
        }
        private bool _AllowNext;
        public bool AllowNext
        {
            get => _AllowNext;
            set
            {
                if (_AllowNext != value)
                {
                    _AllowNext = value;
                    OnPropertyChanged("AllowNext");
                }
            }
        }
        private bool _ShowNext;
        public bool ShowNext
        {
            get => _ShowNext;
            set
            {
                if (_ShowNext != value)
                {
                    _ShowNext = value;
                    OnPropertyChanged("ShowNext");
                }
            }
        }

        private bool _AllowFinish1;
        public bool AllowFinish1
        {
            get => _AllowFinish1;
            set
            {
                if (_AllowFinish1 != value)
                {
                    _AllowFinish1 = value;
                    OnPropertyChanged("AllowFinish1");
                }
            }
        }
        private bool _ShowFinish1;
        public bool ShowFinish1
        {
            get => _ShowFinish1;
            set
            {
                if (_ShowFinish1 != value)
                {
                    _ShowFinish1 = value;
                    OnPropertyChanged("ShowFinish1");
                }
            }
        }

        #endregion
        #region "Command"
        public ICommand FinishCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ClearProductCodesCommand { get; set; }
        public ICommand DeleteProductCodeCommand { get; set; }
        #endregion


        public string Error { get { return null; } }

        public string this[string columnName]
        {
            get
            {
                string error = null;

                var regexVoltage = new Regex("([0-9]*[.])?[0-9]+");
                decimal value;
                switch (columnName)
                {

                }
                return error;
            }
        }

        public NewProductViewModel()
        {
            _ServiceFactory = new ServiceFactory();
            ErrorMessage = null;
            ProductCodes = new ObservableCollection<string>();
            RatedFrequency = "50";
            NumberOfPhase = 3;
            ListStandards = ServiceHelper.LoadStandards().ToObservableCollection();
            SelectedStandard = ListStandards.Where(x => x.Name == "NONE").FirstOrDefault();
            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ProductTypeNews = _ServiceFactory.LoadProducTypeNews();
                try
                {
                    YearCreate = DateTime.Now.Year;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

            ClearProductCodesCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ProductCodes.Clear();
            });
            DeleteProductCodeCommand = new RelayCommand<string>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                ProductCodes.Remove(p);
            });
            FinishCommand = new RelayCommand<Window>((p) => { if (!string.IsNullOrEmpty(_Code) && _SelectedProductTypeNew != null && p != null) return true; else return false; }, (p) =>
            {
                switch (_SelectedProductTypeNew.TypeCode)
                {
                    case "General":
                        DefaultNewProduct();
                        break;
                    case "DistributionTransformer":
                    case "PowerTransformer":
                        NewTransformer();
                        break;

                    case "OutdoorOilImmersedCurrentTransformer":
                        DefaultNewProduct();
                        break;
                    case "BushingCurrentTransformer":
                        DefaultNewProduct();
                        break;
                    case "OutdoorOilImmersedInductiveVoltageTransformer":
                        DefaultNewProduct();
                        break;
                    case "OutdoorOilImmersedCapacitorVoltageTransformer":
                        DefaultNewProduct();
                        break;
                    case "MOF1":
                        DefaultNewProduct();
                        break;
                    case "MOF3":
                        DefaultNewProduct();
                        break;
                    case "BA":
                        DefaultNewProduct();
                        break;
                    default:
                        DefaultNewProduct();
                        break;
                }
            });
        }
        private void NewTransformer()
        {
            MessageServiceClient _Client = ServiceHelper.NewMessageServiceClient();
            try
            {
                IsBusy = true;
                _Client.Open();
                foreach (string codeItem in _ProductCodes)
                {
                    TransformerDTO transformerDTO = new()
                    {
                        CoolingMethod = _CoolingMethod,
                        ConnectionSymbol = _ConnectionSymbol,
                        IsLocked = false,
                        NumberOfPhase = _NumberOfPhase,
                        ProductCode = codeItem,
                        RatedPower = _RatedPower,
                        UnitPower = _UnitPower,
                        RatedVoltage = _RatedVoltage + _VoltageUnit,
                        RatedFrequency = _RatedFrequency,
                        ProductName = ("MBA " + _RatedPower + _UnitPower + " " + _RatedVoltage + _VoltageUnit + " " + _Station).Trim(),
                        Note = _Note,
                        YearOfManufacture = _YearCreate.Value,
                        ProductType = _SelectedProductTypeNew.TypeCode == "DistributionTransformer" ? ProductType.DistributionTransformer : ProductType.PowerTransformer,
                        Station = _Station,
                        StandardId = _SelectedStandard == null ? 10 : _SelectedStandard.Id
                    };
                    _Client.NewTransformer(transformerDTO);
                }
                _Client.Close();
                MessageBox.Show("Thêm mới sản phẩm thành công");
                Code = string.Empty;
                RatedPower = string.Empty;
                RatedVoltage = string.Empty;
                Station = string.Empty;
                Note = string.Empty;
            }
            catch (Exception ex)
            {
                _Client.Abort();
                if (ex.InnerException != null) MessageBox.Show(ex.InnerException.Message);
                else MessageBox.Show(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
        private void DefaultNewProduct()
        {
            bool status = true;
            foreach (var item in _ProductCodes)
            {
                Product product = new Product()
                {
                    IsLocked = false,
                    Note = _Note,
                    ProductCode = item,
                    ProductTypeNewId = _SelectedProductTypeNew.Id,
                    YearOfManufacture = _YearCreate
                };
                if (_ServiceFactory.NewProduct(product) == 0)
                {
                    status = false;
                }
            }
            if (status) MessageBox.Show("Thêm mới thành công");
            else MessageBox.Show("Thao tác thất bại, vui lòng thử lại!");
        }
    }
}