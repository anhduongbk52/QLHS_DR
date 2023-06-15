using System;
using System.IO;
using System.Windows;

namespace QLHS_DR.View.HosoView
{
    /// <summary>
    /// Interaction logic for UploadHoSoWindow.xaml
    /// </summary>
    public partial class UploadHoSoWindow : Window
    {
        public UploadHoSoWindow()
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
