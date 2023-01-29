using QLHS_DR.ChatAppServiceReference;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QLHS_DR.View.ChatAppView
{
    /// <summary>
    /// Interaction logic for ChatAppWindow.xaml
    /// </summary>
    public partial class ChatAppWindow : Window
    {
        private User _user;
        private MainWindow _window;
        private ObservableCollection<ChatAppServiceReference.Message> _message;
        private readonly SolidColorBrush[] userBackground = new SolidColorBrush[4];
        public ChatAppWindow(MainWindow window,User user)
        {
            InitializeComponent();
            this._window = window; 
            _user = user;
            _window.Width = 540;
            _window.Height = 400;
            _window.Background = new SolidColorBrush();
            userBackground[0] = new SolidColorBrush(Color.FromArgb(233, 108, 41, 239));
            userBackground[1] = new SolidColorBrush(Color.FromArgb(233, 239, 41, 210));
            userBackground[2] = new SolidColorBrush(Color.FromArgb(233, 73, 41, 130));
            userBackground[3] = new SolidColorBrush(Color.FromArgb(233, 115, 36, 103));
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Timer timer = new Timer(obj => 
            { 
               
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
    }
}
