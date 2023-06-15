using System.Windows;

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        public EditTaskWindow()
        {
            InitializeComponent();
        }

        private void pdfViwer_DocumentLoaded(object sender, RoutedEventArgs e)
        {
            var temp = pdfViwer.DocumentSource;
        }
    }
}
