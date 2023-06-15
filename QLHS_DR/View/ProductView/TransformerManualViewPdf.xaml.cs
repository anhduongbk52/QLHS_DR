using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace QLHS_DR.View.ProductView
{
    /// <summary>
    /// Interaction logic for TransformerManualViewPdf.xaml
    /// </summary>
    public partial class TransformerManualViewPdf : Window
    {
        MessageServiceClient _MyClient;
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _CanPrint;
        private bool _CanSave;
        private byte[] _ContextFile;
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set
            {
                _IsBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
        private TransformerManualDTO _TransformerManualDTO;


        public TransformerManualViewPdf(bool canPrint, bool canSave, string fileName, TransformerManualDTO transformerManualDTO)
        {
            InitializeComponent();
            pdfViewer1.DataContext = this;
            _CanPrint = canPrint;
            _CanSave = canSave;
            _TransformerManualDTO = transformerManualDTO;
            pdfViewerWindow.Title = fileName;

        }
        private void pdfViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                IsBusy = true;
                pdfViewer1.CanPrint = _CanPrint;
                pdfViewer1.CanSave = _CanSave;

                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();

                _ContextFile = _MyClient.DownloadTransformerManualFile(_TransformerManualDTO.TransformerManualId);
                _MyClient.Close();
                MemoryStream ms = new MemoryStream(_ContextFile);
                pdfViewer1.DocumentSource = ms;
            }
            catch (Exception ex)
            {
                _MyClient.Abort();
                MessageBox.Show(ex.Message + "Error at pdfViewerWindow_Loaded");
            }
            finally
            {
                IsBusy = false;
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual bool SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(member, value))
            {
                return false;
            }
            member = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
