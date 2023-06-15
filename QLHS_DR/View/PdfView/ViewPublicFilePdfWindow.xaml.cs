using QLHS_DR.Core;
using System.IO;
using System.Windows;

namespace QLHS_DR.View.PdfView
{
    /// <summary>
    /// Interaction logic for ViewPublicFilePdfWindow.xaml
    /// </summary>
    public partial class ViewPublicFilePdfWindow : Window
    {
        public ViewPublicFilePdfWindow(int publicFileId)
        {
            ServiceFactory _ServiceFactory = new ServiceFactory();
            byte[] temp = _ServiceFactory.DownloadPublicFile(publicFileId, true);
            InitializeComponent();
            if (temp != null)
            {
                var stream = new MemoryStream(temp);
                pdfViewer.DocumentSource = stream;
            }
            else this.Close();
        }
    }
}

