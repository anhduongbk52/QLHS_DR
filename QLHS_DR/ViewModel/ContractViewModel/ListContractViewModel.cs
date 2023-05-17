using DevExpress.Mvvm.Native;
using QLHS_DR.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.View.ContractView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EofficeClient.Core;

namespace QLHS_DR.ViewModel.ContractViewModel
{
    internal class ListContractViewModel : BaseViewModel
    {

        #region "Properties and Field"
        private bool _CanUploadContract;
        private bool _CanRemoveContract;
        private bool _CanRemoveOwnContract;
        private bool _CanOpenContract;
        private MessageServiceClient _MyClient;
        private ObservableCollection<Contract> _Contracts;
        public ObservableCollection<Contract> Contracts { get => _Contracts; set { _Contracts = value; OnPropertyChanged("Contracts"); } }
        private TransformerDTO _TransformerDTO;
        public TransformerDTO TransformerDTO { get => _TransformerDTO; set { _TransformerDTO = value; OnPropertyChanged("TransformerDTO"); } }

        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand OpenContractCommand { get; set; }
        public ICommand NewContractCommand { get; set; }
        public ICommand RemoveContractCommand { get; set; }

        #endregion
        public ListContractViewModel(TransformerDTO transformerDTO)
        {
            TransformerDTO = transformerDTO;
            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                _CanOpenContract = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productOpenContractFile");
                _CanRemoveContract = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productRemoveContract");
                _CanRemoveOwnContract = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productRemoveContractOfOwner");
                _CanUploadContract = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productUploadContract");
                LoadAllContracts();
            });
            OpenContractCommand = new RelayCommand<Contract>((p) => { if (p != null && _CanOpenContract) return true; else return false; }, (p) =>
            {
                ContractViewPdf pdfViewer = new ContractViewPdf(true, true, p);
                pdfViewer.Show();
            });
            NewContractCommand = new RelayCommand<Object>((p) => { if (_CanUploadContract) return true; else return false; }, (p) =>
            {
                ViewModel.ContractViewModel.AddContractViewModel addContractViewModel = new ViewModel.ContractViewModel.AddContractViewModel(_TransformerDTO);
                View.ContractView.AddContractWindow addContractWindow = new View.ContractView.AddContractWindow() { DataContext = addContractViewModel };
                addContractWindow.ShowDialog();
                LoadAllContracts();
            });
            RemoveContractCommand = new RelayCommand<Contract>((p) => { if (p != null && (_CanRemoveContract || (_CanRemoveOwnContract && (p.UserUploadId==SectionLogin.Ins.CurrentUser.Id)))) return true; else return false; }, (p) =>
            {
                if (System.Windows.MessageBox.Show("Bạn có muốn xóa file: " + p.ContractName, "Cảnh báo", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                        _MyClient.Open();
                        _MyClient.SetDeletedContract(p.id);
                        _MyClient.Close();
                        MessageBox.Show("Xóa thành công");
                        LoadAllContracts();
                    }
                    catch (Exception ex)
                    {
                        _MyClient.Abort();                        
                        if (ex.InnerException != null)
                        {
                            MessageBox.Show(ex.InnerException.Message);
                        }
                        else MessageBox.Show(ex.Message);
                    }
                }
            });
        }
        private void LoadAllContracts()
        {
            _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            ObservableCollection<Contract> ketqua = new ObservableCollection<Contract>();
            try
            {
                _MyClient.Open();   
                Contracts = _MyClient.LoadContracts(_TransformerDTO.Id).ToObservableCollection();
                _MyClient.Close();
            }
            catch (CommunicationException ex)
            {
                _MyClient.Abort();               
                if (ex.InnerException != null)
                {
                    MessageBox.Show(ex.InnerException.Message);
                }
                else MessageBox.Show(ex.Message);
            }
        }
    }
}
