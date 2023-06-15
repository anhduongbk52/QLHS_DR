using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.ProductView;
using System;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    internal class DetailTransformerViewModel : BaseViewModel
    {
        #region "Properties and Filed"
        private bool isFirtLoad;
        public View.ProductView.GeneralInformationProductUC GeneralInformationProductUC { get; set; }
        private ServiceFactory serviceFactory;
        private Product _Product;
        public Product Product { get => _Product; set { _Product = value; OnPropertyChanged("Product"); } }
        private TransformerDTO _TransformerDTO;
        public TransformerDTO TransformerDTO { get => _TransformerDTO; set { _TransformerDTO = value; OnPropertyChanged("TransformerDTO"); } }
        public System.Windows.Controls.UserControl _LoadUC;
        public System.Windows.Controls.UserControl LoadUC
        {
            get => _LoadUC; set { if (_LoadUC != value) { _LoadUC = value; NotifyPropertyChanged("LoadUC"); } }
        }

        public View.ContractView.ListContractUC _ListContractUC { get; set; }
        #endregion
        #region "Commands"
        public ICommand OpenTransformerManuals { get; set; }
        public ICommand GeneralInformationDistributionTransformerCommand { get; set; }
        public ICommand OpenListContractUCCommand { get; set; }
        public ICommand LoadUCCommand { get; set; }
        #endregion
        public DetailTransformerViewModel(Product product)
        {
            serviceFactory = new ServiceFactory();
            GeneralInformationProductUC = new View.ProductView.GeneralInformationProductUC();
            product.TransformerInfo = serviceFactory.GetTransformerInfo(product.Id);
            this.Product = product;
            this.TransformerDTO = new TransformerDTO()
            {
                ConnectionSymbol = product.TransformerInfo.ConnectionSymbol,
                CoolingMethod = product.TransformerInfo.CoolingMethod,
                Id = product.Id,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Note = product.Note,
                NumberOfPhase = product.TransformerInfo.NumberOfPhase.Value,
                IsLocked = product.IsLocked,
                NumberOfWinding = product.TransformerInfo.NumberOfWinding.Value,
                VoltageRatio = product.TransformerInfo.VoltageRatio,
                ProductType = product.ProductType,
                StandardId = product.TransformerInfo.StandardId.Value
            };

            if (product.TransformerInfo == null)
            {
                this.TransformerDTO = new TransformerDTO()
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    DateCreated = product.DateCreated,
                    IsLocked = product.IsLocked,
                    ProductType = product.ProductType,
                    Note = product.Note,
                    UserCreateId = product.UserCreateId != null ? product.UserCreateId.Value : 0,
                    YearOfManufacture = 0,
                };
            }
            else
            {
                this.TransformerDTO = new TransformerDTO()
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    DateCreated = product.DateCreated,
                    IsLocked = product.IsLocked,
                    ProductType = product.ProductType,
                    Note = product.Note,
                    UserCreateId = product.UserCreateId != null ? product.UserCreateId.Value : 0,
                    YearOfManufacture = product.YearOfManufacture != null ? product.YearOfManufacture.Value : 0,
                    ConnectionSymbol = product.TransformerInfo.ConnectionSymbol,
                    CoolingMethod = product.TransformerInfo.CoolingMethod,
                    RatedFrequency = product.TransformerInfo.RatedFrequency,
                    Station = product.TransformerInfo.Station,
                    RatedPower = product.TransformerInfo.RatedPower,
                    UnitPower = product.TransformerInfo.UnitPower,
                    RatedVoltage = product.TransformerInfo.RatedVoltage,
                    StandardId = product.TransformerInfo.StandardId,
                    VoltageRatio = product.TransformerInfo.VoltageRatio,
                    NumberOfWinding = product.TransformerInfo.NumberOfWinding,
                    PowerTransport = product.TransformerInfo.PowerTransport,
                    RatedPowerAndUnit = (product.TransformerInfo.RatedPower + product.TransformerInfo.UnitPower),
                    NumberOfPhase = product.TransformerInfo.NumberOfPhase != null ? product.TransformerInfo.NumberOfPhase.Value : 3
                    //StandardName = product.TransformerInfo.Standard.Name
                };
            }
            isFirtLoad = true; //Khoi tao lan dau

            //OpenListFileScanUC = new RelayCommand<DevExpress.Xpf.NavBar.NavBarItem>((p) => { if (p != null) return true; else return false; }, (p) =>
            //{
            //    int typeFilePdf;
            //    switch (p.Name)
            //    {
            //        case "BtnBvDien":
            //            typeFilePdf = 1;
            //            break;
            //        case "BtnBvCo":
            //            typeFilePdf = 2;
            //            break;
            //        case "BtnBvNt":
            //            typeFilePdf = 3;
            //            break;
            //        default:
            //            typeFilePdf = 1;
            //            break;
            //    }
            //    ListFileScanDistributionTransformerUC listFileScanUC = new VIEW.PdfFile.ListFileScanDistributionTransformerUC();
            //    ListFileScanDistributionTransformerViewModel vm = new ListFileScanDistributionTransformerViewModel() { Transformer = Transformer, TypeFilePdf = typeFilePdf };
            //    listFileScanUC.DataContext = vm;
            //    LoadUC = listFileScanUC;
            //});
            LoadUCCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (isFirtLoad)
                {
                    GeneralInfomationProductViewModel vmGeneralInfomationProduct = new GeneralInfomationProductViewModel(_TransformerDTO);
                    GeneralInformationProductUC.DataContext = vmGeneralInfomationProduct;
                    LoadUC = GeneralInformationProductUC;
                }
                isFirtLoad = false;
            });
            GeneralInformationDistributionTransformerCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                GeneralInfomationProductViewModel vmGeneralInfomationProduct = new GeneralInfomationProductViewModel(_TransformerDTO);
                GeneralInformationProductUC.DataContext = vmGeneralInfomationProduct;
                LoadUC = GeneralInformationProductUC;
            });
            OpenTransformerManuals = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ListTransformerManualUC listTransformerManualUC = new ListTransformerManualUC();
                ListTransformerManualViewModel listTransformerViewModel = new ListTransformerManualViewModel(_Product);
                listTransformerManualUC.DataContext = listTransformerViewModel;
                LoadUC = listTransformerManualUC;
            });
            //OpenListContractUCCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            //{
            //    _ListContractUC = new View.ContractView.ListContractUC();
            //    ContractViewModel.ListContractViewModel listContractViewModel = new ContractViewModel.ListContractViewModel(_TransformerDTO);
            //    _ListContractUC.DataContext = listContractViewModel;
            //    LoadUC = _ListContractUC;
            //});
        }
    }
}
