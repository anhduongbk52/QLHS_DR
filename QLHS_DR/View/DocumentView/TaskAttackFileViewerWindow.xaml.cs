using System;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;


namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for TaskAttackFileViewerWindow.xaml
    /// </summary>
    public partial class TaskAttackFileViewerWindow : Window
    {
        const uint WDA_NONE = 0;
        const uint WDA_MONITOR = 1;

        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        public TaskAttackFileViewerWindow()
        {
            InitializeComponent();
        }

        private void TaskAttackFileViewer_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            SetWindowDisplayAffinity(hWnd, WDA_MONITOR);
        }
    }
}
