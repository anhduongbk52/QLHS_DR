using DevExpress.Xpf.Core;
using System.Windows;

namespace QLHS_DR.View.ProductView
{
    /// <summary>
    /// Interaction logic for NewProductWindow.xaml
    /// </summary>
    public partial class NewProductWindow : Window
    {
        public NewProductWindow()
        {
            InitializeComponent();
        }
        private void Wizard_Cancel(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = (DXMessageBox.Show("Bạn muốn thoát quá trình khai báo sản phẩm mới?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No);
        }
    }
}
