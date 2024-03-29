﻿using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.TransformerManualViewModel
{
    internal class EditTransformerManualViewModel : BaseViewModel
    {
        #region "Field and Properties"
        private readonly bool status = false;
        public bool Status { get; set; }

        private string _TittleWindow;
        public string TittleWindow { get => _TittleWindow; set { _TittleWindow = value; OnPropertyChanged("TittleWindow"); } }

        private string _Description;
        public string Description { get => _Description; set { _Description = value; OnPropertyChanged("Description"); } }

        private string _DocumentName;
        public string DocumentName
        {
            get => _DocumentName;
            set
            {
                if (_DocumentName != value)
                {
                    _DocumentName = value;
                    OnPropertyChanged("DocumentName");
                }
            }
        }
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
        public ICommand SaveCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        #endregion

        public EditTransformerManualViewModel(TransformerManualDTO transformerManualDTO, string productCode)
        {
            this.ProductCode = productCode;

            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ListContents = LoadAllContents();
                ContentTypeSelected = ListContents.Where(x => x.Id == transformerManualDTO.DocTitleId).FirstOrDefault();
                DocumentName = transformerManualDTO.DocumentName;
                Description = transformerManualDTO.Description;
            });
            SaveCommand = new RelayCommand<System.Windows.Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                try
                {
                    _MyClient.Open();
                    _MyClient.EditTransformerManual(transformerManualDTO.FileId, _Description, _ContentTypeSelected.Id, _DocumentName);
                    _MyClient.Close();
                    System.Windows.MessageBox.Show("Cập nhật thành công");
                    p.Close();
                }
                catch (Exception ex)
                {
                    _MyClient.Abort();
                    System.Windows.MessageBox.Show("Thao tác thất bại, vui lòng thử lại");
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });

            ExitCommand = new RelayCommand<System.Windows.Window>((p) => { if (p != null) return true; else return false; }, (p) =>
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
            catch (Exception ex)
            {
                _MyClient.Abort();
                System.Windows.MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
    }
}
