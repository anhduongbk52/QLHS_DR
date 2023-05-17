using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Xpf.PdfViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace QLHS_DR.CustomControl
{
    public class PdfViewerControlForTask : PdfViewerControl
    {
        public bool CanPrint { get; set; }
        public bool CanSave { get; set; }
        public ICommand CustomPrintCommand { get; private set; }
        public ICommand CustomSaveCommand { get; private set; }
        public PdfViewerControlForTask()
        {
            CustomSaveCommand = DelegateCommandFactory.Create(() => SaveAsCommand.Execute(null), () => SaveAsCommand.CanExecute(null) && CanSave);
            CustomPrintCommand = DelegateCommandFactory.Create(MyPrint, () => PrintDocumentCommand.CanExecute(null) && CanPrint);
        }
        void MyPrint()
        {
            PdfPrinterSettings printerSettings = this.ShowPrintPageSetupDialog();
            if (printerSettings != null)
            {

            }
        }

    }
}
