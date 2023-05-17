using QLHS_DR.Core;
using QLHS_DR.ChatAppServiceReference;
using EofficeClient.Core;
using QLHS_DR.View.ProductView;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using System.ServiceModel;
using System.Windows;
using QLHS_DR.ViewModel.TransformerManualViewModel;
using QLHS_DR.View.TransformerManualView;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    internal class ListTransformerManualViewModel:BaseViewModel
    {
        #region "Properties and Field"
        private Product _Product;
        private TransformerManualDTO _SelectedTransformerManual;
        public TransformerManualDTO SelectedTransformerManual { get => _SelectedTransformerManual; set { _SelectedTransformerManual = value; OnPropertyChanged("SelectedTransformerManual"); } }

        private ObservableCollection<TransformerManualDTO> _TransformerManuals;
        public ObservableCollection<TransformerManualDTO> TransformerManuals { get => _TransformerManuals; set { _TransformerManuals = value; OnPropertyChanged("TransformerManuals"); } }
               
        byte[] _Content;
      
        private bool _IsWaitIndicatorVisible;
        public bool IsWaitIndicatorVisible
        {
            get
            {
                return _IsWaitIndicatorVisible;
            }

            set
            {
                _IsWaitIndicatorVisible = value;
                OnPropertyChanged("IsWaitIndicatorVisible");
            }
        }
         
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand OpenPDFCommand { get; set; }
        public ICommand UploadFileHoSoCommand { get; set; }
        public ICommand RemoveFileHoSoCommand { get; set; }
        public ICommand ChangeFileHoSoCommand { get; set; }

        #endregion
        public ListTransformerManualViewModel(Product product)
        {
            _Product = product;
            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                TransformerManuals = LoadTransformerManual();              
            });
            OpenPDFCommand = new RelayCommand<Object>((p) => { if (_SelectedTransformerManual != null && SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productOpenTransformerManualFile")) return true; else return false; }, async (p) =>
            { 
                TransformerManualViewPdf pdfViewer = new TransformerManualViewPdf(true, true, _SelectedTransformerManual.FileName, _SelectedTransformerManual);
                pdfViewer.Show();
            });
            UploadFileHoSoCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productUploadTransformerManualFile")) return true; else return false; },  (p) =>
            {                
                UploadTransformerManualViewModel uploadTransformerManualViewModel = new UploadTransformerManualViewModel(_Product.ProductCode);
                UploadTransformerManualWindow uploadTransformerManualWindow = new UploadTransformerManualWindow() { DataContext = uploadTransformerManualViewModel };
                uploadTransformerManualWindow.ShowDialog();
                TransformerManuals = LoadTransformerManual();
            });
            RemoveFileHoSoCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productRemoveTransformerManual") || (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productRemoveTransformerManualOfOwner") && _SelectedTransformerManual.UserCreateId==SectionLogin.Ins.CurrentUser.Id)) return true; else return false; }, (p) =>
            {
                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                try
                {
                    _MyClient.Open();
                    _MyClient.SetDeletedTransformerManual(_SelectedTransformerManual.TransformerManualId);
                    _MyClient.Close();
                    MessageBox.Show("Xóa thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _MyClient.Abort();
                }
            });
            ChangeFileHoSoCommand = new RelayCommand<Object>((p) => { if (_SelectedTransformerManual!=null && (SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productUploadTransformerManualFile") || ( _SelectedTransformerManual.UserCreateId == SectionLogin.Ins.CurrentUser.Id))) return true; else return false; }, (p) =>
            {
                EditTransformerManualViewModel editTransformerManualViewModel = new EditTransformerManualViewModel(_Product.ProductCode, _SelectedTransformerManual.FileId,_SelectedTransformerManual.DocTitleId);
                EditTransformerManualWindow editTransformerManualWindow = new EditTransformerManualWindow() { DataContext = editTransformerManualViewModel };
                editTransformerManualWindow.ShowDialog();
                TransformerManuals = LoadTransformerManual();
            });
        }
        private ObservableCollection<TransformerManualDTO> LoadTransformerManual()
        {
            MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);

            ObservableCollection<TransformerManualDTO> ketqua = new ObservableCollection<TransformerManualDTO>();
            try
            {
                _MyClient.Open();
                ketqua = _MyClient.LoadTransformerManual(_Product.Id).ToObservableCollection();
                _MyClient.Close();
            }            
            catch (CommunicationException ex)
            {
                _MyClient.Abort();
            }
            return ketqua;
        }          
    }
}
