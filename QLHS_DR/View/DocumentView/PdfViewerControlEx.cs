﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Pdf;
using DevExpress.Xpf.PdfViewer;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.View.DocumentView
{
    public class PdfViewerControlEx : PdfViewerControl
    {
        public static readonly DependencyProperty CanPrintProperty =
        DependencyProperty.Register("CanPrint", typeof(bool), typeof(PdfViewerControlEx), new PropertyMetadata(null));

        public static readonly DependencyProperty CanSaveProperty =
        DependencyProperty.Register("CanSave", typeof(bool), typeof(PdfViewerControlEx), new PropertyMetadata(null));
        private bool _CanSave;
        public bool CanSave
        {
            get => _CanSave;
            set
            {
                if (_CanSave != value)
                {
                    _CanSave = value;
                    NotifyPropertyChanged("CanSave");
                }
            }
        }
        private bool _CanPrint;
        public bool CanPrint
        {
            get => _CanPrint;
            set
            {
                if (_CanPrint != value)
                {
                    _CanPrint = value;
                    NotifyPropertyChanged("CanPrint");
                }
            }
        }
    
       
        public ICommand CustomPrintCommand { get; private set; }
        public ICommand CustomSaveCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
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
        //protected override DevExpress.Mvvm.UI.SaveFileDialogService CreateDefaultSaveFileDialogService()
        //{
        //    return new SaveFileDialogService()
        //    {
        //        DefaultExt = PdfViewerLocalizer.GetString(PdfViewerStringId.PdfFileExtension),
        //        Filter = PdfViewerLocalizer.GetString(PdfViewerStringId.PdfFileFilter),
        //        DefaultFileName = DefaultFileName //custom dependency property of the PdfViewerControl  
        //    };
        //}
        public PdfViewerControlEx()
        {
            //CustomPrintCommand = DelegateCommandFactory.Create(() => PrintDocumentCommand.Execute(null), () => PrintDocumentCommand.CanExecute(null) && CanPrint);
            CustomSaveCommand = DelegateCommandFactory.Create(() => SaveAsCommand.Execute(null), () => SaveAsCommand.CanExecute(null) && CanSave);
        }
    }
}
