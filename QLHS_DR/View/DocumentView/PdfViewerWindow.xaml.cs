using DevExpress.Pdf;
using DevExpress.Xpf.PdfViewer;
using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for PdfViewerWindow.xaml
    /// </summary>
    public partial class PdfViewerWindow : Window
    {
        const float DrawingDpi = 72f;

        MessageServiceClient _MyClient;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly bool _CanPrint;
        private readonly bool _CanSave;
        private readonly TaskAttachedFileDTO _TaskAttachedFileDTO;
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
        public PdfViewerWindow(TaskAttachedFileDTO taskAttachedFileDTO, bool canPrint, bool canSave, IReadOnlyList<User> iReadOnlyListUser, UserTask userTask)
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

                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                DecryptTaskAttachedFile(_TaskAttachedFileDTO, _UserTaskPrint);

                MemoryStream stream = new MemoryStream(_TaskAttachedFileDTO.Content);

                MemoryStream outputStream = new MemoryStream();
                using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                {
                    processor.LoadDocument(stream);
                    List<int> countPrinteds = new List<int>();
                    for (int i = 0; i < processor.Document.Pages.Count; i++)
                    {
                        countPrinteds.Add(_MyClient.GetCountPrintDocument(_UserTaskPrint.Id, i + 1) + 1);
                    }
                    string addText = SectionLogin.Ins.CurrentUser.FullName + " - Time: " + DateTime.Now.ToString() + " IP: " + EofficeCommonLibrary.Common.MyCommon.GetLocalIPAddress();

                    using (SolidBrush textBrush = new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Blue)))
                    {
                        AddGraphics(processor, addText, textBrush, countPrinteds);
                    }
                    using (SolidBrush textBrush1 = new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red)))
                    {
                        if (_TaskAttachedFileDTO.ConfidentialLevel != null && _TaskAttachedFileDTO.ConfidentialLevel != 0)
                        {
                            AddValidStamp1(processor, textBrush1, "BẢO MẬT CẤP " + _TaskAttachedFileDTO.ConfidentialLevel);
                        }
                    }
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
            catch (Exception ex)
            {
                _MyClient.Abort();
                System.Windows.MessageBox.Show(ex.Message + "Error at pdfViewerWindow_Loaded");
            }
        }

        public void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO, UserTask userTask)
        {
            FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            byte[] orAdd = fileHelper.GetKeyDecryptOfTask(userTask);
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void pdfViewer_PrintPage(DependencyObject d, DevExpress.Xpf.PdfViewer.PdfPrintPageEventArgs e)
        {
            if (e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("pdf") || e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("xps") || e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("onenote"))
            {
                e.Cancel = true;
                System.Windows.MessageBox.Show("Bạn không được quyền sử dụng máy in ảo cho tập tin này !");
            }
            else
            {
                try
                {
                    MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    _MyClient.CountPrintDocument(_UserTaskPrint.Id, e.PageNumber, e.PageSettings.PrinterSettings.Copies);
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
                        _MyClient.AddLog(log);
                        _MyClient.SetPrintedUserInTask(_UserTaskPrint.TaskId, SectionLogin.Ins.CurrentUser.Id);
                    }
                    _MyClient.Close();
                }
                catch (Exception ex)
                {
                    _MyClient.Abort();
                    System.Windows.MessageBox.Show(ex.InnerException.Message);
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
        void AddGraphics(PdfDocumentProcessor processor, string text, SolidBrush textBrush, List<int> countPrinted)
        {
            string textInPage;
            IList<PdfPage> pages = processor.Document.Pages;
            for (int i = 0; i < pages.Count; i++)
            {
                textInPage = text + " - " + "Number of copies: " + countPrinted[i];
                PdfPage page = pages[i];
                using (PdfGraphics graphics = processor.CreateGraphics())
                {
                    SizeF actualPageSize = PrepareGraphics(page, graphics);
                    System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Segoe UI");
                    using (Font font = new Font(fontFamily, 6, System.Drawing.FontStyle.Bold))
                    {
                        SizeF textSize = graphics.MeasureString(textInPage, font, PdfStringFormat.GenericDefault);
                        PointF topLeft = new PointF(0, 0);
                        PointF bottomRight = new PointF(actualPageSize.Width - textSize.Width, actualPageSize.Height - textSize.Height);
                        graphics.DrawString(textInPage, font, textBrush, bottomRight);
                        System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black);
                        graphics.AddToPageForeground(page, DrawingDpi, DrawingDpi);
                    }
                }
            }
        }
        void AddValidStamp1(PdfDocumentProcessor processor, SolidBrush textBrush, string confidentialLevel)
        {
            int _StartX = 5;
            int _StartY = 5;
            int _Stamp_Width;
            int _Stamp_Heigh;
            int fontSize = 12;
            IList<PdfPage> pages = processor.Document.Pages;

            for (int i = 0; i < pages.Count; i++)
            {
                PdfPage page = pages[i];
                using (PdfGraphics graphics = processor.CreateGraphics())
                {
                    SizeF actualPageSize = PrepareGraphics(page, graphics);


                    System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Segoe UI");
                    using (Font font = new Font(fontFamily, fontSize, System.Drawing.FontStyle.Bold), font1 = new Font(fontFamily, fontSize, System.Drawing.FontStyle.Bold))
                    {
                        string text1 = confidentialLevel;
                        //string text2 = confidentialLevel;

                        SizeF text1Size = graphics.MeasureString(text1, font, PdfStringFormat.GenericDefault);
                        //SizeF text2Size = graphics.MeasureString(text2, font1, PdfStringFormat.GenericDefault);

                        _Stamp_Width = (int)(text1Size.Width * 1.1);

                        _Stamp_Heigh = (int)((text1Size.Height) * 1.1);

                        PointF center = new PointF(_StartX + _Stamp_Width / 2, _StartY + _Stamp_Heigh / 2);

                        PointF topLeftText1 = new PointF(center.X - text1Size.Width / 2, center.Y - text1Size.Height / 2);
                        // PointF topLeftText2 = new PointF(center.X - text2Size.Width / 2, center.Y - text2Size.Height / 2);                    

                        graphics.DrawString(text1, font, textBrush, topLeftText1);
                        //graphics.DrawString(text2, font1, textBrush, topLeftText2);                      

                        System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red));
                        graphics.DrawRectangle(pen, new RectangleF(_StartX, _StartY, _Stamp_Width, _Stamp_Heigh));

                        graphics.AddToPageForeground(page, 96f, 96f);
                    }
                }
            }
        }
        
    }
}
