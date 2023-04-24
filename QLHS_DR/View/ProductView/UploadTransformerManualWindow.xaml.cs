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

namespace QLHS_DR.View.ProductView
{
    /// <summary>
    /// Interaction logic for UploadTransformerManualWindow.xaml
    /// </summary>
    public partial class UploadTransformerManualWindow : Window
    {
        public UploadTransformerManualWindow()
        {
            InitializeComponent();
        }
        private void uploadFilePdf_Drop(object sender, DragEventArgs e)
        {
            string[] array = (string[])e.Data.GetData(DataFormats.FileDrop, autoConvert: false);
            string path = array?[0];
            if (path != null)
            {
                if (System.IO.Path.GetExtension(path).ToLower() == ".pdf")
                {
                    string filename = System.IO.Path.GetFileName(path);
                    FileInfo fileInfo = new FileInfo(path);
                    double fileSize = fileInfo.Length / 1000000;
                    fileSize = Math.Round(fileSize, 1);
                    DateTime DateIssus = fileInfo.LastWriteTime;
                    textBoxFilePath.Text = path;
                }
                else MessageBox.Show("Bạn phải chọn file pdf");
            }
        }

        private void uploadFilePdf_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
    }

}
