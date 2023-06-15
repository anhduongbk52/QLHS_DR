using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.HoSoViewModel
{
    internal class AddApprovalDocToOtherProductViewModel : BaseViewModel
    {
        ServiceFactory _ServiceFactory;
        private string _MultiCode;
        public string MultiCode
        {
            get => _MultiCode;
            set
            {
                _MultiCode = value;
                OnPropertyChanged("MultiCode");
            }
        }
        private ObservableCollection<Product> _Products;

        public ObservableCollection<Product> Products { get => _Products; set { _Products = value; OnPropertyChanged("Products"); } }
        public ICommand AddProductCodeCommand { get; set; }
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand DeleteProductCodeCommand { get; set; }
        public ICommand ClearAllProductCodeCommand { get; set; }
        internal AddApprovalDocToOtherProductViewModel(ObservableCollection<ApprovalDocumentProduct> approvalDocumentProducts)
        {
            _ServiceFactory = new ServiceFactory();
            Products = new ObservableCollection<Product>();
            AddProductCodeCommand = new RelayCommand<System.Windows.Controls.TextBox>((p) => { return true; }, (p) =>
            {
                try
                {
                    string input = p.Text.Trim().ToUpper();
                    Product product = _ServiceFactory.GetProductByProductCode(input);
                    if (product != null)
                    {
                        if (!_Products.Any(x => x.ProductCode == product.ProductCode))
                        {
                            Products.Add(product);
                        }
                    }
                    List<string> singleCodes = DocScan.GetTransformerCodeSingle(input); // Lấy về tập hợp các mã số có trong mã số đầy đủ
                    if (singleCodes != null && singleCodes.Count > 0)
                    {
                        foreach (var code in singleCodes)
                        {
                            Product product1 = _ServiceFactory.GetProductByProductCode(code);
                            if (product1 != null)
                            {
                                if (!_Products.Any(x => x.ProductCode == product1.ProductCode))
                                {
                                    Products.Add(product1);
                                }
                            }
                        }
                    }
                    else
                    {
                        Product product1 = _ServiceFactory.GetProductByProductCode(p.Text.Trim());
                        if (product1 != null)
                        {
                            if (!_Products.Any(x => x.ProductCode == product1.ProductCode))
                            {
                                Products.Add(product1);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            ClearAllProductCodeCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                Products.Clear();
            });
            DeleteProductCodeCommand = new RelayCommand<Product>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                Products.Remove(p);
            });
            CancelCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                p.Close();
            });
            OkCommand = new RelayCommand<Window>((p) => { if (p != null && _Products.Count > 0) return true; else return false; }, (p) =>
            {
                MessageServiceClient _Client = ServiceHelper.NewMessageServiceClient();
                int[] productIds = Products.Select(x => x.Id).ToArray();
                int[] approvalDocIds = approvalDocumentProducts.Select(x => x.Id).ToArray();
                try
                {
                    _ServiceFactory.AddApprovalDocToOtherProduct(approvalDocIds, productIds);
                    p.Close();
                }
                catch (Exception) { }

            });
        }
    }
}
