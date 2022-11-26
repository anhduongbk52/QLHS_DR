using DevExpress.Xpf.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace QLHS_DR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void tabControl_TabHidden(object sender, TabControlTabHiddenEventArgs e)
        {
            //comment this for memory leak
            DXTabControl tabControl = (DXTabControl)sender;
            IList source = tabControl.ItemsSource as IList;
            if (source != null)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    source.RemoveAt(e.TabIndex);
                }), DispatcherPriority.Render);

                tabControl.SelectedIndex = 0;
            }

        }
    }
}
