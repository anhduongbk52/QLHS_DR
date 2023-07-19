using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using QLHS_DR.ViewModel;
using System;
using System.Windows;

namespace QLHS_DR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 


    public partial class App : Application
    {
        MainViewModel mainViewModel;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (SingleInstance.AlreadyRunning())
            {
                MessageBox.Show("Một phiên bản của phần mềm đang chạy! Vui lòng kiểm tra biểu tượng chương trình ở góc dưới bên phải màn hình");
                App.Current.Shutdown(); // Just shutdown the current application,if any instance found.
            }
            var viewModel = new DXSplashScreenViewModel
            {
                Copyright = "Copyright @EEMC",
                IsIndeterminate = true,
                Title = "Luân chuyển tài liệu",
                Subtitle = "Loading...",
                Logo = null
            };
            var splashScreen = SplashScreenManager.CreateThemed(viewModel, false);
            splashScreen.ShowOnStartup();
            mainViewModel = new MainViewModel();
            var mainWindow = new MainWindow() { DataContext = mainViewModel };
            mainWindow.Show();
            mainWindow.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            mainViewModel.Dispose();
            Application.Current.Shutdown();
        }
    }
}
