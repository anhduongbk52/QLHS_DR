﻿using DevExpress.Pdf;
using System;
using System.Collections.Generic;
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

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for EditStampWindow.xaml
    /// </summary>
    public partial class EditStampWindow : Window
    {
        public EditStampWindow()
        {
            InitializeComponent();
        }

        private void pdfViewer_DocumentLoaded(object sender, RoutedEventArgs e)
        {
            //var temp = pdfViewer.DocumentSource;
        }
    }
}