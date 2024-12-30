using QLHS_DR.ChatAppServiceReference;
using System;
using System.Collections.Generic;
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

namespace QLHS_DR.View.EmployeeView
{
    /// <summary>
    /// Interaction logic for EditEmployeeDocumentWindow.xaml
    /// </summary>
    public partial class EditEmployeeDocumentWindow : Window
    {
        public string DocumentName { get; private set; }
        public string Description { get; private set; }
        public EditEmployeeDocumentWindow(EmployeeDocument employeeDocument)
        {

            InitializeComponent();
            textEditDescription.Text = employeeDocument.Description;
            textEditDocumentName.Text = employeeDocument.DocumentName;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentName = textEditDocumentName.Text;
            Description = textEditDescription.Text;
            DialogResult = true; // Đóng cửa sổ và trả kết quả OK
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Đóng cửa sổ và không trả kết quả
        }
    }
}
