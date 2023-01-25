using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PdfViewer;
using QLHS_DR.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for PdfViewerWindow.xaml
    /// </summary>
    public partial class PdfViewerWindow : Window
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _CanPrint;
        private bool _CanSave;
        private byte[] contextFile;
        private string _Url;
        public string Url
        {
            get => _Url;
            set => _Url = value;
        }
        public PdfViewerWindow(byte[] data, bool canPrint, bool canSave)
        {
            _CanPrint = canPrint;
            _CanSave = canSave;
            InitializeComponent();
            pdfViewer.DataContext = this;
            contextFile = data;
            
        }
        private void pdfViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MemoryStream stream = new MemoryStream(contextFile);
            pdfViewer.DocumentSource = stream;
            pdfViewer.CanPrint = _CanPrint;
            pdfViewer.CanSave= _CanSave;
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

       
        private void pdfViewer_PrintPage(DependencyObject d, PdfPrintPageEventArgs e)
        {
            if(e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("pdf") || e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("xps"))
            {
                MessageBox.Show("Bạn không được quyền sử dụng máy in ảo cho tập tin này !");
                e.Cancel=true;
            }
        }
    }
}
