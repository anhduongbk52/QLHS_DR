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
using System.Windows.Forms;
using EofficeCommonLibrary.Common.Util;
using System.Drawing;
using DevExpress.Pdf;

using System.Diagnostics;
using Syncfusion.Pdf.Parsing;

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for PdfViewerWindow.xaml
    /// </summary>
    public partial class PdfViewerWindow : Window
    {
        const float DrawingDpi = 72f;
        EofficeMainServiceClient _MyClient;
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _CanPrint;
        private bool _CanSave;
        private TaskAttachedFileDTO _TaskAttachedFileDTO;
        private byte[] contextFile;
        private IReadOnlyList<User> _IReadOnlyListUser;
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
        public PdfViewerWindow(TaskAttachedFileDTO taskAttachedFileDTO, bool canPrint, bool canSave, IReadOnlyList<User> iReadOnlyListUser,UserTask userTask)
        {           
            InitializeComponent();
            pdfViewer.DataContext = this;
            _TaskAttachedFileDTO = taskAttachedFileDTO;
            _UserTaskPrint = userTask;
            _CanPrint = canPrint;
            _CanSave = canSave;
        }
        private void pdfViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                FileName = _TaskAttachedFileDTO.FileName;
                TaskName = _UserTaskPrint.Task.Subject;

                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                DecryptTaskAttachedFile(_TaskAttachedFileDTO, _UserTaskPrint);
                MemoryStream stream = new MemoryStream(_TaskAttachedFileDTO.Content);

                MemoryStream outputStream = new MemoryStream();
                using(PdfDocumentProcessor processor = new PdfDocumentProcessor())
                {
                    processor.LoadDocument(stream);

                    using (SolidBrush textBrush = new SolidBrush(System.Drawing.Color.FromArgb(80, System.Drawing.Color.Blue)))
                        AddGraphics(processor,SectionLogin.Ins.CurrentUser.FullName+ " - Time: " +DateTime.Now.ToString()+" IP: "+ EofficeCommonLibrary.Common.MyCommon.GetLocalIPAddress(), textBrush);
                    processor.SaveDocument(outputStream);
                    if (processor.Document.Pages.Count > 0)
                    {
                        pdfViewer.DocumentSource = outputStream;
                    }
                    else
                    {
                        // Do something if the document does not contain any pages
                    }
                }             
               
                
                pdfViewer.CanPrint = _CanPrint;
                pdfViewer.CanSave = _CanSave;

                _MyClient.SetSeenUserInTask(_UserTaskPrint.TaskId, SectionLogin.Ins.CurrentUser.Id);
                _MyClient.Close();

                pdfViewerWindow.Title = _TaskName + " -------- " + _FileName;
               
            }
            catch(Exception ex)
            {
                _MyClient.Abort();
                System.Windows.MessageBox.Show(ex.Message + "Error at pdfViewerWindow_Loaded");
            }
        }

        public void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO, UserTask userTask)
        {
            FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            byte[] orAdd = fileHelper.GetKeyDecryptOfTask(taskAttachedFileDTO.TaskId, userTask);
            if (orAdd != null)
            {
                taskAttachedFileDTO.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO.Content);
            }
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
       
        private void pdfViewer_PrintPage(DependencyObject d, DevExpress.Xpf.PdfViewer.PdfPrintPageEventArgs e)
        {            
            if(e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("pdf") 
                || e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("xps") 
                || e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("onenote"))
            {
                e.Cancel = true;
                System.Windows.MessageBox.Show("Bạn không được quyền sử dụng máy in ảo cho tập tin này !");               
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
                        System.Windows.MessageBox.Show(ex.InnerException.Message);
                    }
                }                    
            }
        }
        static SizeF PrepareGraphics(PdfPage page, PdfGraphics graphics)
        {
            PdfRectangle cropBox = page.CropBox;
            float cropBoxWidth = (float)cropBox.Width;
            float cropBoxHeight = (float)cropBox.Height;

            switch (page.Rotate)
            {
                case 90:
                    graphics.RotateTransform(-90);
                    graphics.TranslateTransform(-cropBoxHeight, 0);
                    return new SizeF(cropBoxHeight, cropBoxWidth);
                case 180:
                    graphics.RotateTransform(-180);
                    graphics.TranslateTransform(-cropBoxWidth, -cropBoxHeight);
                    return new SizeF(cropBoxWidth, cropBoxHeight);
                case 270:
                    graphics.RotateTransform(-270);
                    graphics.TranslateTransform(0, -cropBoxWidth);
                    return new SizeF(cropBoxHeight, cropBoxWidth);
            }
            return new SizeF(cropBoxWidth, cropBoxHeight);
        }
        void AddGraphics(PdfDocumentProcessor processor, string text, SolidBrush textBrush)
        {
            IList<PdfPage> pages = processor.Document.Pages;
            for (int i = 0; i < pages.Count; i++)
            {
                PdfPage page = pages[i];
                using (PdfGraphics graphics = processor.CreateGraphics())
                {
                    SizeF actualPageSize = PrepareGraphics(page, graphics);
                    System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Segoe UI");                   
                    using (Font font = new Font(fontFamily, 10, System.Drawing.FontStyle.Bold))
                    {
                        SizeF textSize = graphics.MeasureString(text, font, PdfStringFormat.GenericDefault);
                        PointF topLeft = new PointF(0, 0);
                        PointF bottomRight = new PointF(actualPageSize.Width - textSize.Width, actualPageSize.Height - textSize.Height);
                        graphics.DrawString(text, font, textBrush, bottomRight);
                        System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black);                       
                        graphics.AddToPageForeground(page, DrawingDpi, DrawingDpi);
                    }
                }
            }
        }
     
    }
}
