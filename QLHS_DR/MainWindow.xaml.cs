﻿using DevExpress.Services.Internal;
using DevExpress.Xpf.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections;
using System.ComponentModel;
using System.Deployment.Application;
using System.Drawing;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Threading;
using QLHS_DR.ViewModel;

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
            NotifyIcon ni = new()
            {
                Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/eemcicon.ico")).Stream),
                Text = "QLHS_DR",
                Visible = true
            };
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

        private void TabControl_TabHidden(object sender, TabControlTabHiddenEventArgs e)
        {
            //comment this for memory leak
            DXTabControl tabControl = (DXTabControl)sender;
            if (tabControl.ItemsSource is IList source)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    TabContainer temp = (TabContainer)(e.Item);
                    temp.Dispose();
                    source.RemoveAt(e.TabIndex);
                    // temp.Dis
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
            if (SectionLogin.Ins.CurrentUser != null && SectionLogin.Ins.Token!=null)
            {
                ServiceFactory serviceManager = new();
                LoginManager loginManager = new()
                {
                    ComputerName = Environment.MachineName,
                    LoginIp = MainViewModel.GetLocalIPAddress(),
                    LogType = LoginType.Logout,
                    ApplicationVersion = MainViewModel.GetRunningVersion().ToString(),
                    ApplicationName = AppDomain.CurrentDomain.FriendlyName
                };
                serviceManager.RecordLogin(loginManager);
            }            
            base.OnClosing(e);
        }
       
        
    }
}
