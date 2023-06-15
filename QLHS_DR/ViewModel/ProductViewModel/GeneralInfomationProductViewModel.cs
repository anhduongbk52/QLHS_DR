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
    class GeneralInfomationProductViewModel : BaseViewModel
    {
        MessageServiceClient client;
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
        private ProductTypeWrapper _SelectedProductTypeWrapper;
        public ProductTypeWrapper SelectedProductTypeWrapper
        {
            get => _SelectedProductTypeWrapper;
            set
            {
                if (_SelectedProductTypeWrapper != value)
                {
                    _SelectedProductTypeWrapper = value;
                    TransformerDTO.ProductType = _SelectedProductTypeWrapper.EnumValue;
                    OnPropertyChanged("SelectedProductTypeWrapper");
                }
            }
        }
        private ObservableCollection<ProductTypeWrapper> _ProductTypeWrappers;
        public ObservableCollection<ProductTypeWrapper> ProductTypeWrappers
        {
            get => _ProductTypeWrappers;
            set
            {
                if (_ProductTypeWrappers != value)
                {
                    _ProductTypeWrappers = value;
                    OnPropertyChanged("ProductTypeWrappers");
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
        private bool _CanChangeProduct;
        public bool CanChangeProduct
        {
            get => _CanChangeProduct;
            set
            {
                if (_CanChangeProduct != value)
                {
                    _CanChangeProduct = value;
                    OnPropertyChanged("CanChangeProduct");
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
        public GeneralInfomationProductViewModel(TransformerDTO transformerDTO)
        {
            try
            {
                CanChangeProduct = SectionLogin.Ins.ListPermissions.Any(x => x.Code == "productChangeProductInfo");
                ListStandards = ServiceHelper.LoadStandards().ToObservableCollection();
                TransformerDTO = transformerDTO;
                if (_ListStandards != null)
                    SelectedStandard = _ListStandards.Where(x => x.Id == _TransformerDTO.StandardId).FirstOrDefault();

                ProductTypeWrappers = new ObservableCollection<ProductTypeWrapper>()
                {
                   new ProductTypeWrapper(ProductType.PowerTransformer),
                   new ProductTypeWrapper(ProductType.DistributionTransformer)
                };
                SelectedProductTypeWrapper = ProductTypeWrappers.Where(x => x.EnumValue == transformerDTO.ProductType).FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "at GeneralInfomationProductViewModel");
            }
            CongTruCommand = new RelayCommand<System.Windows.Controls.TextBox>((p) => { if (p == null) return false; else return true; }, (p) =>
            {
                p.Text = p.Text + "±";
            });
            SaveChangeCommand = new RelayCommand<object>((p) => { if (_CanChangeProduct) return true; else return false; }, (p) =>
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Bạn có muốn lưu thay đổi không", "Cảnh báo", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        client = ServiceHelper.NewMessageServiceClient();
                        client.Open();
                        this.TransformerDTO.StandardId = SelectedStandard?.Id;
                        client.UpdateTransformer(_TransformerDTO);
                        MessageBox.Show("Cập nhật thành công");
                    }
                    catch (Exception ex)
                    {
                        client.Abort();
                        MessageBox.Show(ex.Message + "at SaveChangeCommand");
                    }
                    finally
                    {
                        if (client != null && client.State != System.ServiceModel.CommunicationState.Faulted)
                        {
                            client.Close();
                        }
                    }
                }
            });
        }

    }
}
