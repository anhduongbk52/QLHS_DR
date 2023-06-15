using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ContractViewModel
{
    class AddContractViewModel : BaseViewModel
    {
        #region "Properties and Filed"
        MessageServiceClient _Proxy;
        ServiceFactory _ServiceFactory;
        private string _TittleWindow;
        public string TittleWindow { get => _TittleWindow; set { _TittleWindow = value; OnPropertyChanged("TittleWindow"); } }
        private string _ErrorMessage;
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set
            {
                if (_ErrorMessage != value)
                {
                    _ErrorMessage = value;
                    OnPropertyChanged("ErrorMessage");
                }
            }
        }
        private string _PowerTransformerMultiCode;
        public string PowerTransformerMultiCode
        {
            get => _PowerTransformerMultiCode;
            set
            {
                if (_PowerTransformerMultiCode != value)
                {
                    _PowerTransformerMultiCode = value;
                    OnPropertyChanged("PowerTransformerMultiCode");
                }
            }
        }
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
        private string _ContractName;
        public string ContractName
        {
            get => _ContractName;
            set
            {
                if (_ContractName != value)
                {
                    _ContractName = value;
                    NotifyPropertyChanged("ContractName");
                }
            }
        }
        private string _ContractDescription;
        public string ContractDescription
        {
            get => _ContractDescription;
            set
            {
                if (_ContractDescription != value)
                {
                    _ContractDescription = value;
                    NotifyPropertyChanged("ContractDescription");
                }
            }
        }
        public Product _Product;
        public Product Product { get => _Product; set { _Product = value; OnPropertyChanged("TransformerDTO"); } }

        public ObservableCollection<Product> _Products;
        public ObservableCollection<Product> Products { get => _Products; set { _Products = value; OnPropertyChanged("Products"); } }

        public string Error { get { return null; } }

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case "PowerTransformerMultiCode":
                        if (DocScan.IsTransformerCode(PowerTransformerMultiCode))
                        {
                            ErrorMessage = error = "Sai định dạng mã số";
                        }
                        break;
                }
                return error;
            }
        }
        #endregion

        #region "Command" 

        public ICommand AddPowerTransformerCodeToListCommand { get; set; }
        public ICommand AddnewContractCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ClearAllTransformerCodeCommand { get; set; }
        public ICommand DeletePowerTransformerCodeCommand { get; set; }
        #endregion
        public AddContractViewModel(Product product)
        {
            _ServiceFactory = new ServiceFactory();
            Product = product;
            Products = new ObservableCollection<Product>();
            if (_Product != null)
            {
                Products.Add(_Product);
                TittleWindow = "Upload hồ sơ: " + _Product.ProductCode;
            }
            else
            {
                TittleWindow = "Upload hồ sơ";
            }
            AddPowerTransformerCodeToListCommand = new RelayCommand<System.Windows.Controls.TextBox>((p) => { if (p?.Text != "") return true; else return false; }, (p) =>
            {
                try
                {
                    if (p.Text.ToUpper().Contains("DUNGCHUNG") || p.Text.ToUpper().Contains("DUNG CHUNG"))
                    {
                        string code = p.Text;
                        Product transformer = _ServiceFactory.GetProductByProductCode(code);
                        if (transformer != null)
                        {
                            if (!_Products.Any(x => x.ProductCode == transformer.ProductCode))
                            {
                                Products.Add(transformer);
                            }
                        }
                    }
                    else
                    {
                        List<string> singleCodes = DocScan.GetProductCodeSingle(p.Text); // Lấy về tập hợp các mã số có trong mã số đầy đủ
                        if (singleCodes != null)
                        {
                            foreach (var code in singleCodes)
                            {
                                Product transformer = _ServiceFactory.GetProductByProductCode(code);
                                if (transformer != null)
                                {
                                    if (!_Products.Any(x => x.ProductCode == transformer.ProductCode))
                                    {
                                        Products.Add(transformer);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
            OpenFileCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FilePath = openFileDialog.FileName;
                    FileInfo fileInfo = new FileInfo(_FilePath);
                    ContractName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                }
            });
            ClearAllTransformerCodeCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                Products.Clear();
            });
            DeletePowerTransformerCodeCommand = new RelayCommand<Product>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                Products.Remove(p);
            });
            CancelCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                p.Close();
            });
            AddnewContractCommand = new RelayCommand<Window>((p) => { if (_Products?.Count > 0 && _ContractName != null && _FilePath != null) return true; else return false; }, (p) =>
            {
                try
                {
                    byte[] fileData = System.IO.File.ReadAllBytes(_FilePath);
                    _Proxy = ServiceHelper.NewMessageServiceClient();
                    _Proxy.Open();

                    foreach (var item in _Products)
                    {
                        _Proxy.UploadContract(fileData, item.Id, _ContractName, Path.GetFileName(_FilePath), _ContractDescription, false);
                    }
                    _Proxy.Close();
                    System.Windows.MessageBox.Show("Tải lên thành công.");
                    p.Close();
                }
                catch (CommunicationException ex)
                {
                    _Proxy.Abort();
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
