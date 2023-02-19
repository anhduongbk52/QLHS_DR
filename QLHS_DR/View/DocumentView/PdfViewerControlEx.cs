using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PdfViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QLHS_DR.View.DocumentView
{
    public class PdfViewerControlEx : PdfViewerControl
    {
        public bool CanPrint { get; set; }
        public bool CanSave { get; set; }
        public ICommand CustomPrintCommand { get; private set; }
        public ICommand CustomSaveCommand { get; private set; }
        public PdfViewerControlEx()
        {
            CustomPrintCommand = DelegateCommandFactory.Create(() => PrintDocumentCommand.Execute(null), () => PrintDocumentCommand.CanExecute(null) && CanPrint);
            CustomSaveCommand = DelegateCommandFactory.Create(() => SaveAsCommand.Execute(null),() => SaveAsCommand.CanExecute(null) && CanSave);
        }

    }
}
