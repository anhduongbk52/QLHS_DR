using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    internal class TransformerTDViewModel : BaseViewModel
    {
        MessageServiceClient client;
        ServiceFactory _ServiceFactory;
        private TransformerDTO _TransformerDTO;
        private ObservableCollection<ChatAppServiceReference.Standard> _ListStandards;
        public ObservableCollection<ChatAppServiceReference.Standard> ListStandards
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
                    _ProductTypeNews = value;
                    OnPropertyChanged("ProductTypeNews");
                }
            }
        }
        private ChatAppServiceReference.Standard _SelectedStandard;
        public ChatAppServiceReference.Standard SelectedStandard
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
        public TransformerDTO TransformerDTO
        {
            get => _TransformerDTO;
            set
            {
                _TransformerDTO = value;
                OnPropertyChanged("TransformerDTO");
            }
        }
        public ICommand SaveChangeCommand { get; set; }
        public ICommand CongTruCommand { get; set; }
        public TransformerTDViewModel(Product product)
        {
            _ServiceFactory = new ServiceFactory();
            TransformerDTO = _ServiceFactory.GetTransformerDTOById(product.Id);
            ListStandards = ServiceHelper.LoadStandards().ToObservableCollection();
            SelectedStandard = _ListStandards.Where(x => x.Id == _TransformerDTO.StandardId).FirstOrDefault();
            ProductTypeNews = _ServiceFactory.LoadProducTypeNews();
            SelectedProductTypeNew = _ProductTypeNews.Where(x => x.Id == product.ProductTypeNewId).FirstOrDefault();

            CongTruCommand = new RelayCommand<System.Windows.Controls.TextBox>((p) => { if (p == null) return false; else return true; }, (p) =>
            {
                p.Text = p.Text + "±";
            });
            SaveChangeCommand = new RelayCommand<object>((p) => { if (SectionLogin.Ins.CanChangeTDOfProduct) return true; else return false; }, (p) =>
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Bạn có muốn lưu thay đổi không", "Cảnh báo", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        client = ServiceHelper.NewMessageServiceClient();
                        client.Open();
                        TransformerDTO.StandardId = _SelectedStandard?.Id;
                        TransformerDTO.ProductTypeNewId = _SelectedProductTypeNew.Id;
                        client.UpdateTransformer(_TransformerDTO);
                        client.Close();
                        MessageBox.Show("Cập nhật thành công");
                    }
                    catch (Exception ex)
                    {
                        client.Abort();
                        MessageBox.Show(ex.Message + "at SaveChangeCommand");
                    }
                }
            });
        }
    }
}
