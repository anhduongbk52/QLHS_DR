﻿using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm;
using DevExpress.Pdf;
using EofficeClient.Core;
using EofficeCommonLibrary.Common.Ultil;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.DocumentView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class TaskAttackFileViewerViewModel : BaseViewModel, IDisposable
    {
        const float DrawingDpi = 72f;

        MessageServiceClient _MyClient;
        private PdfPrinterSettings _PrinterSetting;
        private bool _CanPrintFile;
        public bool CanPrintFile
        {
            get => _CanPrintFile;
            set
            {
                if (_CanPrintFile != value)
                {
                    _CanPrintFile = value;
                    NotifyPropertyChanged("CanPrintFile");
                }
            }
        }
        private bool _CanSaveFile;
        public bool CanSaveFile
        {
            get => _CanSaveFile;
            set
            {
                if (_CanSaveFile != value)
                {
                    _CanSaveFile = value;
                    NotifyPropertyChanged("CanSaveFile");
                }
            }
        }
        private TaskAttachedFileDTO _TaskAttachedFileDTO;
        private UserTask _UserTaskPrint;
        public UserTask UserTaskPrint
        {
            get => _UserTaskPrint;
            set
            {
                if (_UserTaskPrint != value)
                {
                    _UserTaskPrint = value;
                    NotifyPropertyChanged("UserTaskPrint");
                }
            }
        }
        private string _TaskName;
        public string TaskName
        {
            get => _TaskName;
            set
            {
                if (_TaskName != value)
                {
                    _TaskName = value;
                    NotifyPropertyChanged("TaskName");
                }
            }
        }

        private string _FileName;
        public string FileName
        {
            get => _FileName;
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    NotifyPropertyChanged("FileName");
                }
            }
        }
        private string _Title;
        public string Title
        {
            get => _Title;
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }
        private Object _DocumentSource;
        public Object DocumentSource
        {
            get => _DocumentSource;
            set
            {
                if (_DocumentSource != value)
                {
                    _DocumentSource = value;
                    NotifyPropertyChanged("DocumentSource");
                }
            }
        }
        private int _currentPage;
        private MemoryStream _outputStream;
        private MemoryStream _outputStream1;
        [Command]
        public void SaveAs()
        {
            this.GetService<IMessageBoxService>().ShowMessage("Saved");
        }
        public ICommand LoadedWindowCommand { get; private set; }
        public ICommand CustomPrintCommand { get; private set; }
        public ICommand CustomSaveCommand { get; private set; }
        public ICommand ClosedWindowCommand { get; private set; }

        internal TaskAttackFileViewerViewModel(TaskAttachedFileDTO taskAttachedFileDTO, bool canPrint, bool canSave, UserTask userTask)
        {
            _TaskAttachedFileDTO = taskAttachedFileDTO;
            taskAttachedFileDTO = null;
            UserTaskPrint = userTask;
            CanPrintFile = canPrint;
            CanSaveFile = canSave;
            FileName = _TaskAttachedFileDTO.FileName;
            ClosedWindowCommand = new RelayCommand<Object>((p) => { return true; },(p)=>
            {
                this.Dispose();                
            });

            LoadedWindowCommand = new RelayCommand<PdfViewerControlEx>((p) => { return true; }, (p) =>
            {
                try
                {
                    p.CanSave = _CanSaveFile;
                    
                    TaskName = _UserTaskPrint.Task.Subject;

                    _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    
                    if(_TaskAttachedFileDTO.KeyFile!=null)
                    {
                        _TaskAttachedFileDTO.Content = AESHelper.DecryptWithoutIV(_TaskAttachedFileDTO.KeyFile, _TaskAttachedFileDTO.Content);
                    }
                    else DecryptTaskAttachedFile(_TaskAttachedFileDTO, _UserTaskPrint);

                    _outputStream = new MemoryStream();
                    using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                    {
                        using (MemoryStream stream = new MemoryStream(_TaskAttachedFileDTO.Content))
                        {
                            processor.LoadDocument(stream);
                            List<int> countPrinteds = new List<int>();
                            for (int i = 0; i < processor.Document.Pages.Count; i++)
                            {
                                countPrinteds.Add(_MyClient.GetCountPrintDocument(_UserTaskPrint.Id, i + 1));
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
                                    AddValidStamp1(processor, textBrush1, $"BẢO MẬT CẤP {_TaskAttachedFileDTO.ConfidentialLevel}");
                                }
                            }
                            processor.SaveDocument(_outputStream);
                            if (processor.Document.Pages.Count > 0)
                            {
                                DocumentSource = _outputStream;
                            }
                            else
                            {
                                // Do something if the document does not contain any pages
                            }
                        }                           
                    }
                    _MyClient.SetSeenUserInTask(_UserTaskPrint.TaskId, SectionLogin.Ins.CurrentUser.Id);
                    _MyClient.Close();
                    Title = _TaskName + " -------- " + _FileName;
                }
                catch (Exception ex)
                {
                    _MyClient.Abort();
                    System.Windows.MessageBox.Show(ex.Message + "Error at pdfViewerWindow_Loaded");
                }
            });
            CustomPrintCommand = new RelayCommand<PdfViewerControlEx>((p) => { return _CanPrintFile; }, (p) =>
            {
                try
                {
                    _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    PdfViewerControlEx pdfViewerControlEx = new PdfViewerControlEx();
                    pdfViewerControlEx.CanPrint = _CanPrintFile;
                    pdfViewerControlEx.CanSave = _CanSaveFile;
                    _currentPage = p.CurrentPageNumber;
                    _outputStream1 = new MemoryStream();
                    using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                    {
                        using (MemoryStream stream = new MemoryStream(_TaskAttachedFileDTO.Content))
                        {
                            processor.LoadDocument(stream);
                            List<int> countPrinteds = new List<int>();
                            for (int i = 0; i < processor.Document.Pages.Count; i++)
                            {
                                countPrinteds.Add(_MyClient.GetCountPrintDocument(_UserTaskPrint.Id, i + 1) + 1);
                            }
                            string addText = $"{SectionLogin.Ins.CurrentUser.FullName} - Time: + {DateTime.Now.ToString()} IP: {EofficeCommonLibrary.Common.MyCommon.GetLocalIPAddress()}";

                            using (SolidBrush textBrush = new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Blue)))
                            {
                                AddGraphics(processor, addText, textBrush, countPrinteds);
                            }
                            using (SolidBrush textBrush1 = new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red)))
                            {
                                if (_TaskAttachedFileDTO.ConfidentialLevel != null && _TaskAttachedFileDTO.ConfidentialLevel != 0)
                                {
                                    AddValidStamp1(processor, textBrush1, $"BẢO MẬT CẤP{ _TaskAttachedFileDTO.ConfidentialLevel}");
                                }
                            }
                            processor.SaveDocument(_outputStream1);
                            if (processor.Document.Pages.Count > 0)
                            {
                                pdfViewerControlEx.DocumentSource = _outputStream1;
                            }
                            else
                            {
                                // Do something if the document does not contain any pages
                            }
                        }                            
                    }
                    pdfViewerControlEx.DocumentLoaded += PdfViewerControlEx_DocumentLoaded;
                    _MyClient.Close();
                }
                catch (Exception ex)
                {
                    _MyClient.Abort();
                    System.Windows.MessageBox.Show(ex.Message + "Error at pdfViewerWindow_Loaded");
                }
            });
        }

        private void PdfViewerControlEx_DocumentLoaded(object sender, System.Windows.RoutedEventArgs e)
        {

            PdfViewerControlEx pdfViewer = (PdfViewerControlEx)sender;
            pdfViewer.CurrentPageNumber = _currentPage;
            PdfPrinterSettings printerSettings = pdfViewer.ShowPrintPageSetupDialog();
            if (printerSettings != null)
            {

                if (printerSettings.Settings.PrinterName.ToLower().Contains("pdf") || printerSettings.Settings.PrinterName.ToLower().Contains("xps") || printerSettings.Settings.PrinterName.ToLower().Contains("onenote"))
                {
                    System.Windows.MessageBox.Show("Bạn không được quyền sử dụng máy in ảo cho tập tin này !");
                }
                else
                {
                    try
                    {
                        _PrinterSetting = printerSettings;
                        int copies = printerSettings.Settings.Copies;
                        pdfViewer.Print(printerSettings, true);

                        MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                        _MyClient.Open();

                        for (int i = 0; i < printerSettings.PageNumbers.Length; i++)
                        {
                            _MyClient.CountPrintDocument1(_UserTaskPrint.Id, printerSettings.PageNumbers[i], copies, true, _PrinterSetting.Settings.PrinterName);
                        }
                        Log log = new Log
                        {
                            Created = DateTime.Now,
                            UserId = SectionLogin.Ins.CurrentUser.Id,
                            LogType = LogType.PRINT_DOCUMENT,
                            Description = "Người dùng [" + SectionLogin.Ins.CurrentUser.FullName +
                                          "] đã thực hiện in [" + printerSettings.PageNumbers.Length +
                                          "] trang tài liệu [" + _FileName +
                                          "] trong luồng công việc [" + _TaskName +
                                          "] sử dụng máy in [" + printerSettings.Settings.PrinterName + "].",
                            IPAddress = EofficeCommonLibrary.Common.MyCommon.GetLocalIPAddress()
                        };
                        _MyClient.AddLog(log);
                        _MyClient.SetPrintedUserInTask(_UserTaskPrint.TaskId, SectionLogin.Ins.CurrentUser.Id);

                        _MyClient.Close();
                    }
                    catch (Exception ex)
                    {
                        _MyClient.Abort();
                        System.Windows.MessageBox.Show(ex.InnerException.Message);
                    }
                }
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
                //textInPage = text + " - " + "Number of copies: " + countPrinted[i];
                textInPage = text;
                PdfPage page = pages[i];
                using (PdfGraphics graphics = processor.CreateGraphics())
                {
                    SizeF actualPageSize = PrepareGraphics(page, graphics);
                    int fontSize = (int)(Math.Min(actualPageSize.Width, actualPageSize.Height) * 0.013);
                    System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Segoe UI");
                    using (Font font = new Font(fontFamily, fontSize, System.Drawing.FontStyle.Bold))
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
            int fontSize;
            PdfPage page;
            SizeF actualPageSize;
            SizeF text1Size;
            PointF center;
            PointF topLeftText1;
            string text1 = confidentialLevel;
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(125, System.Drawing.Color.Red));

            System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Segoe UI");
            IList<PdfPage> pages = processor.Document.Pages;

            for (int i = 0; i < pages.Count; i++)
            {
                page = pages[i];
                using (PdfGraphics graphics = processor.CreateGraphics())
                {
                    actualPageSize = PrepareGraphics(page, graphics);
                    fontSize = (int)(Math.Min(actualPageSize.Width, actualPageSize.Height) * 0.02);
                    using (Font font = new Font(fontFamily, fontSize, System.Drawing.FontStyle.Bold), font1 = new Font(fontFamily, fontSize, System.Drawing.FontStyle.Bold))
                    {
                        text1Size = graphics.MeasureString(text1, font1, PdfStringFormat.GenericTypographic, 72f, 72f);
                        _Stamp_Width = (int)(text1Size.Width * 1.1);
                        _Stamp_Heigh = (int)((text1Size.Height) * 1.1);
                        center = new PointF(_StartX + _Stamp_Width / 2, _StartY + _Stamp_Heigh / 2);
                        topLeftText1 = new PointF(center.X - text1Size.Width / 2, center.Y - text1Size.Height / 2);
                        graphics.DrawString(text1, font, textBrush, topLeftText1);
                        graphics.DrawRectangle(pen, new RectangleF(_StartX, _StartY, _Stamp_Width, _Stamp_Heigh));
                        graphics.AddToPageForeground(page, 72f, 72f);
                    }
                }
            }
        }

        public void Dispose()
        {
            _TaskAttachedFileDTO = null;
            _UserTaskPrint = null;
            _TaskName = null;
            _FileName = null;
            _Title = null;
            LoadedWindowCommand = null;
            CustomPrintCommand = null;
            ClosedWindowCommand = null;
            DocumentSource = null;
            _outputStream?.Dispose();
            _outputStream1?.Dispose();
            _outputStream = null;
            _outputStream1 = null;
            _MyClient = null;
           
        }
    }
}
