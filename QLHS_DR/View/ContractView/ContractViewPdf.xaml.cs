using QLHS_DR.Core;
using QLHS_DR.ChatAppServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using EofficeClient.Core;

namespace QLHS_DR.View.ContractView
{
    /// <summary>
    /// Interaction logic for ContractViewPdf.xaml
    /// </summary>
    public partial class ContractViewPdf : Window
    {      
        private Contract _Contract;
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
        public ContractViewPdf(bool canPrint, bool canSave, Contract contract)
        {
            InitializeComponent();
            _Contract = contract;
            pdfViewer2.DataContext = this;
            _CanPrint = canPrint;
            _CanSave = canSave;
            this.Title = contract.ContractName + " " + System.IO.Path.GetFileName(contract.FilePath);
        }
        private void pdfViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            try
            {
                IsBusy = true;
                pdfViewer2.CanPrint = _CanPrint;
                pdfViewer2.CanSave = _CanSave;

                _MyClient.Open();
                _ContextFile = _MyClient.DownloadContractFile(_Contract.id);
                _MyClient.Close();
                if (_ContextFile != null)
                {
                    MemoryStream ms = new MemoryStream(_ContextFile);
                    pdfViewer2.DocumentSource = ms;
                }               
            }
            catch (Exception ex)
            {
                _MyClient.Abort();
                System.Windows.MessageBox.Show(ex.Message + "Error at pdfViewerWindow_Loaded");
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
