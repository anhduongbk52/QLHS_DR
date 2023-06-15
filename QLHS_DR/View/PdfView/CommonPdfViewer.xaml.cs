using System.Windows;

namespace QLHS_DR.View.PdfView
{
    /// <summary>
    /// Interaction logic for CommonPdfViewer.xaml
    /// </summary>
    public partial class CommonPdfViewer : Window
    {
        public CommonPdfViewer(object content)
        {
            InitializeComponent();
            pdfViewer.DocumentSource = content;
        }
    }
}
