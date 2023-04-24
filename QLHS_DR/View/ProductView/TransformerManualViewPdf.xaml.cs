using DevExpress.Pdf;
using DevExpress.Xpf.Core;
using EofficeClient.Core;
using QLHS_DR.Core;
using QLHS_DR.EOfficeServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static DevExpress.Utils.HashCodeHelper.Primitives;

namespace QLHS_DR.View.ProductView
{
    /// <summary>
    /// Interaction logic for TransformerManualViewPdf.xaml
    /// </summary>
    public partial class TransformerManualViewPdf : Window
    {
        ChannelFactory<IEofficeMainService> _ChannelFactory;
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


        public TransformerManualViewPdf(bool canPrint, bool canSave, string fileName,TransformerManualDTO transformerManualDTO)
        {
            InitializeComponent();
            pdfViewer1.DataContext = this;
            _CanPrint = canPrint;
            _CanSave= canSave;
            _TransformerManualDTO= transformerManualDTO;
            pdfViewerWindow.Title = fileName;
           
        }
        private void pdfViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //loadingDecorator.IsSplashScreenShown = true;
                IsBusy = true;
                pdfViewer1.CanPrint = _CanPrint;
                pdfViewer1.CanSave = _CanSave;

                _ChannelFactory = new ChannelFactory<IEofficeMainService>("WSHttpBinding_IEofficeMainService");
                _ChannelFactory.Credentials.UserName.UserName = SectionLogin.Ins.CurrentUser.UserName;
                _ChannelFactory.Credentials.UserName.Password = SectionLogin.Ins.Token;
                var _proxy = _ChannelFactory.CreateChannel();
                ((IClientChannel)_proxy).Open();
                _ContextFile = _proxy.DownloadTransformerManualFile(_TransformerManualDTO.TransformerManualId);
                ((IClientChannel)_proxy).Close();
                MemoryStream ms = new MemoryStream(_ContextFile);
                pdfViewer1.DocumentSource = ms;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "Error at pdfViewerWindow_Loaded");
            }
            finally
            {
                IsBusy  = false;
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
