using DevExpress.Xpf.Core;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;

namespace QLHS_DR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        NotifyIcon nIcon = new NotifyIcon();
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/eemcicon.ico")).Stream);
            ni.Text = "QLHS_DR";
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    if (loginName.Content != null)
                    {
                        this.Show();
                        this.WindowState = System.Windows.WindowState.Normal;
                        this.Activate();
                    }
                };
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
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized) this.Hide();
            base.OnStateChanged(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
            base.OnClosing(e);
        }
    }
}
