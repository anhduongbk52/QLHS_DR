using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PdfViewer;
using QLHS_DR.Core;
using QLHS_DR.EOfficeServiceReference;
using QLHS_DR.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using EofficeCommonLibrary;
using EofficeClient.Core;

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for PdfViewerWindow.xaml
    /// </summary>
    public partial class PdfViewerWindow : Window
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _CanPrint;
        private bool _CanSave;
        private byte[] contextFile;
        
        private UserTask _UserTaskPrint;
       
        public UserTask UserTaskPrint
        {
            get => _UserTaskPrint;
            set => _UserTaskPrint = value;
        }
        private string _TaskName;
        public string TaskName
        {
            get => _TaskName;
            set => _TaskName = value;
        }
        private string _FileName;
        public string FileName
        {
            get => _FileName;
            set => _FileName = value;
        }
        public PdfViewerWindow(byte[] data, bool canPrint, bool canSave)
        {
            _CanPrint = canPrint;
            _CanSave = canSave;
            InitializeComponent();
            pdfViewer.DataContext = this;
            contextFile = data;
            
        }
        private void pdfViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MemoryStream stream = new MemoryStream(contextFile);
            pdfViewer.DocumentSource = stream;
            pdfViewer.CanPrint = _CanPrint;
            pdfViewer.CanSave= _CanSave;
            EofficeMainServiceClient _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            _MyClient.Open();
            _MyClient.SetSeenUserInTask(_UserTaskPrint.TaskId, SectionLogin.Ins.CurrentUser.Id);
            _MyClient.Close();
            pdfViewerWindow.Title = _TaskName+ " -------- "+_FileName;
        }

      
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual bool SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(member, value))
            {
                return false;
            }

            member = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

       
        private void pdfViewer_PrintPage(DependencyObject d, PdfPrintPageEventArgs e)
        {            
            if(e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("pdf") 
                || e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("xps") 
                || e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("onenote"))
            {
                e.Cancel = true;
                MessageBox.Show("Bạn không được quyền sử dụng máy in ảo cho tập tin này !");               
            }            
            else
            {
                if (e.PageNumber == e.PageCount)
                {
                    Log log = new Log
                    {
                        Created = DateTime.Now,
                        UserId = SectionLogin.Ins.CurrentUser.Id,
                        LogType = LogType.PRINT_DOCUMENT,
                        Description = "Người dùng [" + SectionLogin.Ins.CurrentUser.FullName + "] đã thực hiện in [" +
                                   e.PageCount + "] trang tài liệu [" + _FileName + "] trong luồng công việc [" + _TaskName + "] sử dụng máy in [" + e.PageSettings.PrinterSettings.PrinterName + "].",
                        IPAddress = EofficeCommonLibrary.Common.MyCommon.GetLocalIPAddress()
                    };
                    try
                    {
                        EofficeMainServiceClient _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                        _MyClient.Open();
                        _MyClient.AddLog(log);
                        _MyClient.SetPrintedUserInTask(_UserTaskPrint.TaskId,SectionLogin.Ins.CurrentUser.Id);
                        _MyClient.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.InnerException.Message);
                    }
                }                    
            }
        }        
    }
}
