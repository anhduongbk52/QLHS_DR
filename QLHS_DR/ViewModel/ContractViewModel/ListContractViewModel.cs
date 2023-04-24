using DevExpress.Mvvm.Native;
using QLHS_DR.Core;
using QLHS_DR.EOfficeServiceReference;
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

namespace QLHS_DR.ViewModel.ContractViewModel
{
    internal class ListContractViewModel : BaseViewModel
    {

        #region "Properties and Field"
        private bool _CanUploadContract;
        private bool _CanRemoveContract;
        private bool _CanRemoveOwnContract;
        private bool _CanOpenContract;
        ChannelFactory<IEofficeMainService> _ChannelFactory;
        IEofficeMainService _Proxy;
        private ObservableCollection<EOfficeServiceReference.Contract> _Contracts;
        public ObservableCollection<EOfficeServiceReference.Contract> Contracts { get => _Contracts; set { _Contracts = value; OnPropertyChanged("Contracts"); } }
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

                _ChannelFactory = new ChannelFactory<IEofficeMainService>("WSHttpBinding_IEofficeMainService");
                _ChannelFactory.Credentials.UserName.UserName = SectionLogin.Ins.CurrentUser.UserName;
                _ChannelFactory.Credentials.UserName.Password = SectionLogin.Ins.Token;
                LoadAllContracts();
            });
            OpenContractCommand = new RelayCommand<EOfficeServiceReference.Contract>((p) => { if (p != null && _CanOpenContract) return true; else return false; }, (p) =>
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
            RemoveContractCommand = new RelayCommand<EOfficeServiceReference.Contract>((p) => { if (p != null && _CanRemoveContract) return true; else return false; }, (p) =>
            {
                if (System.Windows.MessageBox.Show("Bạn có muốn xóa file: " + p.ContractName, "Cảnh báo", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        if (_ChannelFactory == null || _ChannelFactory.State == CommunicationState.Faulted || _ChannelFactory.State != CommunicationState.Opened)
                        {
                            _ChannelFactory = new ChannelFactory<IEofficeMainService>("WSHttpBinding_IEofficeMainService");
                            _ChannelFactory.Credentials.UserName.UserName = SectionLogin.Ins.CurrentUser.UserName;
                            _ChannelFactory.Credentials.UserName.Password = SectionLogin.Ins.Token;
                        }
                        else if (_ChannelFactory.State == CommunicationState.Closed)
                        {
                            _ChannelFactory.Open();
                        }
                        _Proxy = _ChannelFactory.CreateChannel();
                        ((IClientChannel)_Proxy).Open();
                        _Proxy.SetDeletedContract(p.id);
                        ((IClientChannel)_Proxy).Close();
                        MessageBox.Show("Xóa thành công");
                        LoadAllContracts();
                    }
                    catch (Exception ex)
                    {
                        ((IClientChannel)_Proxy).Abort();
                        _ChannelFactory.Abort();
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
            ObservableCollection<EOfficeServiceReference.Contract> ketqua = new ObservableCollection<EOfficeServiceReference.Contract>();
            try
            {
                if (_ChannelFactory == null || _ChannelFactory.State == CommunicationState.Faulted || _ChannelFactory.State != CommunicationState.Opened)
                {
                    _ChannelFactory = new ChannelFactory<IEofficeMainService>("WSHttpBinding_IEofficeMainService");
                    _ChannelFactory.Credentials.UserName.UserName = SectionLogin.Ins.CurrentUser.UserName;
                    _ChannelFactory.Credentials.UserName.Password = SectionLogin.Ins.Token;
                }
                else if (_ChannelFactory.State == CommunicationState.Closed)
                {
                    _ChannelFactory.Open();
                }
                _Proxy = _ChannelFactory.CreateChannel();
                ((IClientChannel)_Proxy).Open();
                if (_TransformerDTO != null)
                {
                    Contracts = _Proxy.LoadContracts(_TransformerDTO.Id).ToObservableCollection();
                }
                ((IClientChannel)_Proxy).Close();
            }
            catch (CommunicationException ex)
            {
                ((IClientChannel)_Proxy).Abort();
                _ChannelFactory.Abort();
                if (ex.InnerException != null)
                {
                    MessageBox.Show(ex.InnerException.Message);
                }
                else MessageBox.Show(ex.Message);
            }
        }
    }
}
