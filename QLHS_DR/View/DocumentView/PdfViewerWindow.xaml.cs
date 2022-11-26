using DevExpress.ClipboardSource.SpreadsheetML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private byte[] contextFile;
        private string _Url;
        public string Url
        {
            get => _Url;
            set => _Url = value;
        }

        public PdfViewerWindow(byte[] data)
        {
            InitializeComponent();
            contextFile = data;
        }

        private void pdfViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MemoryStream stream = new MemoryStream(contextFile);
            pdfViewer.DocumentSource = stream;
        }
    }
}
