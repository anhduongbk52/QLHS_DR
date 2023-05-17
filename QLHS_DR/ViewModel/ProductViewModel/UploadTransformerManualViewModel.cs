using DevExpress.Data.TreeList;
using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.Core;
using QLHS_DR.ChatAppServiceReference;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    internal class UploadTransformerManualViewModel : BaseViewModel
    {
        #region "Field and Properties"
        private bool status = false;
        public bool Status { get; set; }

       

        private string _TittleWindow;
        public string TittleWindow { get => _TittleWindow; set { _TittleWindow = value; OnPropertyChanged("TittleWindow"); } }
        private bool _IsEnableCodeTextBox;
        public bool IsEnableCodeTextBox { get => _IsEnableCodeTextBox; set { _IsEnableCodeTextBox = value; OnPropertyChanged("IsEnableCodeTextBox"); } }
        private string _Description;
        public string Description { get => _Description; set { _Description = value; OnPropertyChanged("Description"); } }
      
        private string _ProductCode;
        public string ProductCode { get => _ProductCode; set { _ProductCode = value; OnPropertyChanged("ProductCode"); } }
        private Product _Product;
        public Product Product
        { 
            get => _Product;
            set 
            {
                _Product = value; OnPropertyChanged("Product"); 
            } 
        }
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
        #endregion

        public UploadTransformerManualViewModel(string productCode)
        {
            this.ProductCode = productCode;
                     
            LoadedWindowCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                ListContents = LoadAllContents();
                TittleWindow = "Upload hồ sơ máy biến áp: " + _ProductCode;
                ////Gettransformer
                if (_ProductCode != null)
                {
                    //this.Product = ServiceProxy.Instance.Proxy.GetProductById.Where(x => x.Code == transformerCode).FirstOrDefault();
                    //PowerTransformerCode = transformerCode;
                    //IsEnableCodeTextBox = false;
                }
                else
                {
                    IsEnableCodeTextBox = true;
                }
            });
            UploadCommand = new RelayCommand<Window>((p) => { if (_ContentTypeSelected != null && _FilePath != null && _ProductCode != null) return true; else return false; }, (p) =>
            {
                UploadTransformerManual(p);                
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
            ExitCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                p.Close();
            });
        }        
       
        public void ClearField()
        {
            FilePath = null;
            ContentTypeSelected = null;
        }
        private ObservableCollection<DocTittle> LoadAllContents()
        {
            MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            ObservableCollection<DocTittle> ketqua = new ObservableCollection<DocTittle>();
            try
            {
                _MyClient.Open();
                ketqua = _MyClient.LoadDoctitle().ToObservableCollection();
                _MyClient.Close();
            }
            catch(Exception ex)
            {
                _MyClient.Abort();
                System.Windows.MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
        private void UploadTransformerManual(Window window)
        {
            MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            try
            {
                _MyClient.Open();
                byte[] fileData = System.IO.File.ReadAllBytes(_FilePath);
                _MyClient.UploadTransformerManual1(fileData,Path.GetFileName(_FilePath), _ContentTypeSelected.Title,_ProductCode,_Description,false);
                _MyClient.Close();
                System.Windows.MessageBox.Show("Tải lên thành công");
                window.Close();
            }
            catch (Exception ex)
            {
                _MyClient.Abort();
                System.Windows.MessageBox.Show("Thao tác thất bại, vui lòng thử lại");
                System.Windows.MessageBox.Show(ex.Message);
            }
        }     

    }

}
