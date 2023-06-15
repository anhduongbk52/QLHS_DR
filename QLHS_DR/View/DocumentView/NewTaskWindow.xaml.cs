using System.Windows;

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for NewTaskWindow.xaml
    /// </summary>
    public partial class NewTaskWindow : Window
    {
        public NewTaskWindow()
        {
            InitializeComponent();
        }

        private void PdfViewerControl_DocumentLoaded_1(object sender, RoutedEventArgs e)
        {
            var temp = pdfViwer.DocumentSource;
        }
    }
}
