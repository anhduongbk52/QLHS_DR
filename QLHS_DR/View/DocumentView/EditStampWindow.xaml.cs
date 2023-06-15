using System.Windows;

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for EditStampWindow.xaml
    /// </summary>
    public partial class EditStampWindow : Window
    {
        public EditStampWindow()
        {
            InitializeComponent();
        }

        private void pdfViewer_DocumentLoaded(object sender, RoutedEventArgs e)
        {
            //var temp = pdfViewer.DocumentSource;
        }
    }
}
