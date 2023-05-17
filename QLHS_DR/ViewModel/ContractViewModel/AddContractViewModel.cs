using QLHS_DR.Core;
using QLHS_DR.ChatAppServiceReference;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using EofficeClient.Core;

namespace QLHS_DR.ViewModel.ContractViewModel
{
    internal class AddContractViewModel : BaseViewModel
    {
        #region "Properties and Filed"
        MessageServiceClient _MyClient;
        private bool status = false;
        public bool Status { get; set; }
        private int _ProgessBarValue;
        public int ProgessBarValue { get => _ProgessBarValue; set { _ProgessBarValue = value; OnPropertyChanged("ProgressBarValue"); } }
        private bool _ProgessBarIsVisible;
        public bool ProgessBarIsVisible { get => _ProgessBarIsVisible; set { _ProgessBarIsVisible = value; OnPropertyChanged("ProgessBarIsVisible"); } }
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
        public AddContractViewModel(TransformerDTO transformerDTO)
        {
            
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();               
                Product = _MyClient.GetProductById(transformerDTO.Id);
                _MyClient.Close();

                Products = new ObservableCollection<Product>();
                if (_Product != null)
                {
                    Products.Add(_Product);
                }
            }
            catch (Exception ex)
            {
                _MyClient.Abort();
                System.Windows.MessageBox.Show(ex.Message);
            }

            AddPowerTransformerCodeToListCommand = new RelayCommand<System.Windows.Controls.TextBox>((p) => { if (DocScan.IsTransformerCode(p?.Text)) return true; else return false; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    List<string> singleCodes = DocScan.GetTransformerCodeSingle(p.Text); // Lấy về tập hợp các mã số có trong mã số đầy đủ

                    _MyClient.Open();

                    foreach (var code in singleCodes)
                    {
                        Product product = _MyClient.GetProductByProductCode(code);
                        if (product != null && !Products.Any(x => x.Id == product.Id))
                        {
                            if (product != null)
                            {
                                Products.Add(product);
                            }
                            else System.Windows.MessageBox.Show("Máy biến áp MS: " + code + " chưa được khởi tạo");
                        }
                    }
                    _MyClient.Close();
                }
                catch (Exception ex)
                {
                    _MyClient.Abort();
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
                    ContractName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
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
                    _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    byte[] fileData = System.IO.File.ReadAllBytes(_FilePath);                  

                    foreach (var item in _Products)
                    {
                        _MyClient.UploadContract(fileData, item.Id, _ContractName, System.IO.Path.GetFileName(_FilePath), _ContractDescription, false);
                    }
                    _MyClient.Close();
                    System.Windows.MessageBox.Show("Tải lên thành công.");
                    p.Close();
                }
                catch (Exception ex)
                {
                    _MyClient.Abort();
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
