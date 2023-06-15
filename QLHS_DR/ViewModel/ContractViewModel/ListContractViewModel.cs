using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.ContractView;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ContractViewModel
{
    class ListContractViewModel : BaseViewModel
    {
        #region "Properties and Field"

        MessageServiceClient _Proxy;
        private ObservableCollection<Contract> _Contracts;
        public ObservableCollection<Contract> Contracts { get => _Contracts; set { _Contracts = value; OnPropertyChanged("Contracts"); } }
        private Product _Product;
        public Product Product { get => _Product; set { _Product = value; OnPropertyChanged("Product"); } }

        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand OpenContractCommand { get; set; }
        public ICommand NewContractCommand { get; set; }
        public ICommand RemoveContractCommand { get; set; }

        #endregion
        public ListContractViewModel(Product product)
        {
            Product = product;
            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                LoadAllContracts();
            });
            OpenContractCommand = new RelayCommand<Contract>((p) => { if (p != null && SectionLogin.Ins.CanOpenContract) return true; else return false; }, (p) =>
            {
                ContractViewPdf pdfViewer = new ContractViewPdf(true, true, p);
                pdfViewer.Show();
            });
            NewContractCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.CanUploadContract) return true; else return false; }, (p) =>
            {
                AddContractViewModel uploadTransformerManualViewModel = new ViewModel.ContractViewModel.AddContractViewModel(_Product);
                AddContractWindow uploadTransformerManualWindow = new AddContractWindow() { DataContext = uploadTransformerManualViewModel };
                uploadTransformerManualWindow.ShowDialog();
                LoadAllContracts();
            });
            RemoveContractCommand = new RelayCommand<Contract>((p) => { if (p != null && SectionLogin.Ins.CanRemoveContract) return true; else return false; }, (p) =>
            {
                if (MessageBox.Show("Bạn có muốn xóa file: " + p.ContractName, "Cảnh báo", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        _Proxy = ServiceHelper.NewMessageServiceClient();
                        _Proxy.Open();
                        _Proxy.SetDeletedContract(p.id);
                        _Proxy.Close();
                        MessageBox.Show("Xóa thành công");
                        LoadAllContracts();
                    }
                    catch (Exception ex)
                    {
                        _Proxy.Abort();
                        MessageBox.Show(ex.Message);
                    }
                }
            });
        }
        private void LoadAllContracts()
        {
            try
            {
                _Proxy = ServiceHelper.NewMessageServiceClient();
                _Proxy.Open();
                if (_Product != null)
                {
                    Contracts = _Proxy.LoadContracts(_Product.Id).ToObservableCollection();
                }
                _Proxy.Close();
            }
            catch (CommunicationException ex)
            {
                _Proxy.Abort();

                if (ex.InnerException != null)
                {
                    MessageBox.Show(ex.InnerException.Message);
                }
                else MessageBox.Show(ex.Message);
            }
        }
    }
}
