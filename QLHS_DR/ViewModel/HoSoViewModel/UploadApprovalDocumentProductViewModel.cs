using Microsoft.Win32;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.HoSoViewModel
{
    class UploadApprovalDocumentProductViewModel : BaseViewModel
    {
        #region "Field and Properties"
        ServiceFactory _ServiceFactory;
        private string _TittleWindow;
        public string TittleWindow { get => _TittleWindow; set { _TittleWindow = value; OnPropertyChanged("TittleWindow"); } }
        private string _DocumentName;
        public string DocumentName { get => _DocumentName; set { _DocumentName = value; OnPropertyChanged("DocumentName"); } }
        private string _Description;
        public string Description { get => _Description; set { _Description = value; OnPropertyChanged("Description"); } }
        public ObservableCollection<Product> _Products;
        public ObservableCollection<Product> Products { get => _Products; set { _Products = value; OnPropertyChanged("Products"); } }

        private string _FilePath;
        public string FilePath
        {
            get => _FilePath;
            set
            {
                if (_FilePath != value)
                {
                    _FilePath = value;
                    NotifyPropertyChanged("FilePath");
                }
            }
        }
        private int _ApprovalNumber;
        public int ApprovalNumber
        {
            get => _ApprovalNumber;
            set
            {
                if (_ApprovalNumber != value)
                {
                    _ApprovalNumber = value;
                    NotifyPropertyChanged("ApprovalNumber");
                }
            }
        }
        #endregion

        #region "Commands"
        public ICommand ExitCommand { get; set; }
        public ICommand UploadCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand AddProductsCommand { get; set; }
        public ICommand ClearAllProductCommand { get; set; }
        public ICommand RemoveProductCommand { get; set; }
        #endregion

        public UploadApprovalDocumentProductViewModel(Product product)
        {
            _ServiceFactory = new ServiceFactory();
            Products = new ObservableCollection<Product>();
            TittleWindow = "Tải lên hồ sơ phê duyệt của sản phẩm";
            //Gettransformer
            if (product != null)
            {
                TittleWindow = "Upload tài liệu phê duyệt của: " + product.ProductCode;
                Products.Add(product);
                int num = _ServiceFactory.GetLastApprovalNumber(product.Id);
                ApprovalNumber = num != 0 ? num : 1;
            }
            LoadedWindowCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {


            });
            UploadCommand = new RelayCommand<Window>((p) => { if (!string.IsNullOrEmpty(_FilePath) && !String.IsNullOrEmpty(_DocumentName) && _Products.Count > 0 && _ApprovalNumber != 0) return true; else return false; }, (p) =>
            {
                byte[] fileData = System.IO.File.ReadAllBytes(_FilePath);
                List<ApprovalDocumentProduct> approvalDocumentProducts = new List<ApprovalDocumentProduct>();
                if (fileData != null)
                {
                    foreach (Product product in _Products)
                    {
                        approvalDocumentProducts.Add(new ApprovalDocumentProduct()
                        {
                            FileName = Path.GetFileName(_FilePath),
                            DocumentName = _DocumentName,
                            FileExtension = Path.GetExtension(_FilePath),
                            ProductId = product.Id,
                            Description = _Description,
                            ApprovalNumber = _ApprovalNumber
                            //DateCreate= DateTime.Now,
                            //DateExpired
                            //DecryptKey
                            //ExpiredByUserId
                            //ExtensionData
                            //FilePath
                            //IsExpired=false
                            //UserCreateId
                        });
                    }
                    _ServiceFactory.UploadApprovalDocumentProduct(fileData, approvalDocumentProducts.ToArray());
                }
                p.Close();
            });
            OpenFileCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    FilePath = openFileDialog.FileName;
                }
            });
            AddProductsCommand = new RelayCommand<System.Windows.Controls.TextBox>((p) => { if (p?.Text != "") return true; else return false; }, (p) =>
            {
                try
                {
                    List<string> singleCodes = DocScan.GetProductCodeSingle(p.Text.Trim()); // Lấy về tập hợp các mã số có trong mã số đầy đủ
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
            ClearAllProductCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                Products.Clear();
            });
            RemoveProductCommand = new RelayCommand<Product>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                Products.Remove(p);
            });
            ExitCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                p.Close();
            });
        }

        public void ClearField()
        {
            FilePath = null;
            Products.Clear();
        }

    }
}
