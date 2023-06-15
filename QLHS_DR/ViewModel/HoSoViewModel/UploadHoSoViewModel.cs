using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.HoSoViewModel
{
    class UploadHoSoViewModel : BaseViewModel
    {
        #region "Field and Properties"
        ServiceFactory _ServiceFactory;
        public bool Status { get; set; }

        MessageServiceClient _Proxy;

        private string _TittleWindow;
        public string TittleWindow { get => _TittleWindow; set { _TittleWindow = value; OnPropertyChanged("TittleWindow"); } }

        private string _DocumentName;
        public string DocumentName { get => _DocumentName; set { _DocumentName = value; OnPropertyChanged("DocumentName"); } }
        private string _Description;
        public string Description { get => _Description; set { _Description = value; OnPropertyChanged("Description"); } }
        public ObservableCollection<Product> _Products;
        public ObservableCollection<Product> Products { get => _Products; set { _Products = value; OnPropertyChanged("Products"); } }


        private ObservableCollection<DocTittle> _ListContents;
        public ObservableCollection<DocTittle> ListContents
        {
            get => _ListContents;
            set
            {
                if (_ListContents != value)
                {
                    _ListContents = value;
                    NotifyPropertyChanged("ListContents");
                }
            }
        }
        private DocTittle _ContentTypeSelected;
        public DocTittle ContentTypeSelected
        {
            get => _ContentTypeSelected;
            set
            {
                if (_ContentTypeSelected != value)
                {
                    _ContentTypeSelected = value;
                    NotifyPropertyChanged("ContentTypeSelected");
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
        #endregion

        #region "Commands"
        public ICommand ExitCommand { get; set; }
        public ICommand UploadCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand NewContentCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand AddProductsCommand { get; set; }
        public ICommand ClearAllProductCommand { get; set; }
        public ICommand RemoveProductCommand { get; set; }
        #endregion

        public UploadHoSoViewModel(string transformerCode)
        {
            _ServiceFactory = new ServiceFactory();
            Products = new ObservableCollection<Product>();
            ListContents = LoadAllContents();
            if (transformerCode != null)
            {
                TittleWindow = "Upload hồ sơ nghiệm thu: " + transformerCode;
                Product product = _ServiceFactory.GetProductByProductCode(transformerCode);
                Products.Add(product);
            }
            else
            {
                TittleWindow = "Tải lên hồ sơ nghiệm thu của sản phẩm";
            }
            LoadedWindowCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {


            });
            UploadCommand = new RelayCommand<Window>((p) => { if (_ContentTypeSelected != null && _FilePath != "" && _DocumentName != "" && _Products.Count > 0) return true; else return false; }, (p) =>
            {
                try
                {
                    List<int> productIds = new List<int>();
                    foreach (Product product in _Products)
                    {
                        productIds.Add(product.Id);
                    }
                    byte[] fileData = System.IO.File.ReadAllBytes(_FilePath);

                    if (fileData != null)
                    {
                        _ServiceFactory.UploadProductManual(fileData, _Description, Path.GetFileName(_FilePath), _ContentTypeSelected.Id, productIds.ToArray());
                    }
                    System.Windows.MessageBox.Show("Tải lên thành công");
                    p.Close();
                }
                catch (Exception ex)
                {
                    _Proxy.Abort();
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
                    double fileSize = fileInfo.Length / 1000000;
                    fileSize = Math.Round(fileSize, 1);
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
                    System.Windows.MessageBox.Show(ex.Message);
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
            ContentTypeSelected = null;
            Products.Clear();
        }
        private ObservableCollection<DocTittle> LoadAllContents()
        {
            ObservableCollection<DocTittle> ketqua = new ObservableCollection<DocTittle>();
            try
            {
                _Proxy = ServiceHelper.NewMessageServiceClient();
                _Proxy.Open();
                ; ketqua = _Proxy.LoadDoctitle().ToObservableCollection();
                _Proxy.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                _Proxy.Abort();
            }
            return ketqua;
        }
    }

}
