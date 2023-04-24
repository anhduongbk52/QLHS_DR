using QLHS_DR.EOfficeServiceReference;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using ToastNotifications.Core;
using ToastNotifications;
using System.Windows.Input;
using QLHS_DR.ViewModel.Message;
using System.Drawing;
using DevExpress.Pdf;
using DevExpress.XtraPrinting;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class EditStampViewModel : BaseViewModel
    {
        #region "Field and properties"
        const float DrawingDpi = 96f;
        private bool _IsFirtLoad;
        private string _Url;
        public string Url
        {
            get => _Url;
            set
            {
                if (_Url != value)
                {
                    _Url = value;
                    NotifyPropertyChanged("Url");
                }
            }
        }

        private Object _PathFile;
        public Object PathFile
        {
            get => _PathFile;
            set
            {
                if (_PathFile != value)
                {
                    _PathFile = value;
                    NotifyPropertyChanged("PathFile");
                }
            }
        }
        private float _Stamp_Width;
        public float Stamp_Width
        {
            get => _Stamp_Width;
            set
            {
                if (_Stamp_Width != value)
                {
                    _Stamp_Width = value;
                    NotifyPropertyChanged("Stamp_Width");
                }
            }
        }
        private float _Stamp_Heigh;
        public float Stamp_Heigh
        {
            get => _Stamp_Heigh;
            set
            {
                if (_Stamp_Heigh != value)
                {
                    _Stamp_Heigh = value;
                    NotifyPropertyChanged("Stamp_Heigh");
                }
            }
        }
        private float _Page_Width;
        public float Page_Width
        {
            get => _Page_Width;
            set
            {
                if (_Page_Width != value)
                {
                    _Page_Width = value;
                    NotifyPropertyChanged("Page_Width");
                }
            }
        }
        private float _Page_Heigh;
        public float Page_Heigh
        {
            get => _Page_Heigh;
            set
            {
                if (_Page_Heigh != value)
                {
                    _Page_Heigh = value;
                    NotifyPropertyChanged("Page_Heigh");
                }
            }
        }

        private float _StartX;
        public float StartX
        {
            get => _StartX;
            set
            {
                if (_StartX != value)
                {
                    _StartX = value;
                    NotifyPropertyChanged("StartX");
                }
            }
        }
        private float _StartY;
        public float StartY
        {
            get => _StartY;
            set
            {
                if (_StartY != value)
                {
                    _StartY = value;
                    NotifyPropertyChanged("StartY");
                }
            }
        }
        #endregion

        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ReDrawStamp { get; set; }
        public ICommand DocumentLoadedCommand { get; set; }
        #endregion
        public EditStampViewModel()
        {
            _IsFirtLoad = true;
            DocumentLoadedCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                try
                {
                    if(_IsFirtLoad)
                    {
                        Url = _PathFile.ToString();
                        _IsFirtLoad=false;
                    }
                    
                    using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                    {
                        processor.LoadDocument(_Url, true);

                        IList<PdfPage> pages = processor.Document.Pages;

                        PdfGraphics graphics = processor.CreateGraphics();

                        PdfPage page = pages[0];
                        SizeF actualPageSize = PrepareGraphics(page, graphics);
                        Page_Width = actualPageSize.Width;
                        Page_Heigh = actualPageSize.Height;
                                               
                    }  
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
            ReDrawStamp = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                try
                {
                    using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                    {
                        processor.LoadDocument(_Url, true);
                        MemoryStream outputStream = new MemoryStream();
                        using (SolidBrush textBrush = new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Blue)))
                        {
                            AddValidStamp1(processor, textBrush);
                            processor.SaveDocument(outputStream, true);
                            PathFile = outputStream;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
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
        void AddValidStamp1(PdfDocumentProcessor processor, SolidBrush textBrush)
        {
            IList<PdfPage> pages = processor.Document.Pages;

            for (int i = 0; i < pages.Count; i++)
            {
                PdfPage page = pages[i];
                using (PdfGraphics graphics = processor.CreateGraphics())
                {
                    SizeF actualPageSize = PrepareGraphics(page, graphics);
                    System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Segoe UI");
                    using (Font font = new Font(fontFamily, 10, System.Drawing.FontStyle.Bold),font1 = new Font(fontFamily, 14, System.Drawing.FontStyle.Bold))
                    {
                        string text1 = "TỔNG CÔNG TY";
                        string text1_1 = "THIẾT BỊ ĐIỆN ĐÔNG ANH";
                        string text3 = "HIỆU LỰC";
                        string text2 = DateTime.Now.Day + " - " + DateTime.Now.Month + " - " + DateTime.Now.Year;

                        SizeF text1Size = graphics.MeasureString(text1, font, PdfStringFormat.GenericDefault);
                        SizeF text1_1Size = graphics.MeasureString(text1_1, font, PdfStringFormat.GenericDefault);
                        SizeF text2Size = graphics.MeasureString(text2, font1, PdfStringFormat.GenericDefault);
                        SizeF text3Size = graphics.MeasureString(text3, font1, PdfStringFormat.GenericDefault);

                        PointF center = new PointF(_StartX + Stamp_Width / 2, _StartY + _Stamp_Heigh / 2);

                        PointF topLeftText1 = new PointF(center.X - text1Size.Width / 2, center.Y - text1Size.Height / 2 - 40);
                        PointF topLeftText1_1 = new PointF(center.X - text1_1Size.Width / 2, center.Y - text1_1Size.Height / 2 - 25);
                        PointF topLeftText2 = new PointF(center.X - text2Size.Width / 2, center.Y - text1Size.Height / 2);
                        PointF topLeftText3 = new PointF(center.X - text3Size.Width / 2, center.Y - text1Size.Height / 2+30);

                        graphics.DrawString(text1, font, textBrush, topLeftText1);
                        graphics.DrawString(text1_1, font, textBrush, topLeftText1_1);
                        graphics.DrawString(text2, font1, textBrush, topLeftText2);
                        graphics.DrawString(text3, font1, textBrush, topLeftText3);

                       System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Blue)); 
                        graphics.DrawRectangle(pen, new RectangleF(_StartX, _StartY, _Stamp_Width, _Stamp_Heigh));
                     
                        graphics.AddToPageForeground(page, DrawingDpi, DrawingDpi);


                    }
                                 
                }
            }
        }    
    }
}
