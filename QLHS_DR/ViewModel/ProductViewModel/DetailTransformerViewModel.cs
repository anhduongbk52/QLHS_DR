using DevExpress.Data.TreeList;
using QLHS_DR.EOfficeServiceReference;
using QLHS_DR.View.ProductView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    internal class DetailTransformerViewModel:BaseViewModel
    {
        #region "Properties and Filed"
        private bool isFirtLoad;
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
            this.Product = product;
            this.TransformerDTO = new TransformerDTO()
            {
                //ConnectionSymbol= product.TransformerInfo.ConnectionSymbol,
                //CoolingMethod = product.TransformerInfo.CoolingMethod,
                Id= product.Id,
            };
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
                    //GeneralInformationDistributionTransformerUC generalInformationDistributionTransformerUC = new GeneralInformationDistributionTransformerUC();

                    //GeneralInformationDistributionTransformerViewModel informationDistributionTransformerViewModel = new GeneralInformationDistributionTransformerViewModel(_Transformer.Id);

                    //generalInformationDistributionTransformerUC.DataContext = informationDistributionTransformerViewModel;
                    //LoadUC = generalInformationDistributionTransformerUC;
                }
                isFirtLoad = false;
            });
            GeneralInformationDistributionTransformerCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                //GeneralInformationDistributionTransformerUC generalInformationDistributionTransformerUC = new GeneralInformationDistributionTransformerUC();

                //GeneralInformationDistributionTransformerViewModel informationDistributionTransformerViewModel = new GeneralInformationDistributionTransformerViewModel(_Transformer.Id);
                //generalInformationDistributionTransformerUC.DataContext = informationDistributionTransformerViewModel;
                //LoadUC = generalInformationDistributionTransformerUC;
            });
            OpenTransformerManuals = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ListTransformerManualUC  listTransformerManualUC = new ListTransformerManualUC();               
                ListTransformerManualViewModel listTransformerViewModel = new ListTransformerManualViewModel(_Product);
                listTransformerManualUC.DataContext = listTransformerViewModel;
                LoadUC = listTransformerManualUC;
            });
            OpenListContractUCCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                _ListContractUC = new View.ContractView.ListContractUC();
                ContractViewModel.ListContractViewModel listContractViewModel = new ContractViewModel.ListContractViewModel(_TransformerDTO);
                _ListContractUC.DataContext = listContractViewModel;
                LoadUC = _ListContractUC;
            });
        }
    }
}
