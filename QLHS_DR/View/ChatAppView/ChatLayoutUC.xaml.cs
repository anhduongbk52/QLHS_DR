using System.Windows.Controls;

namespace QLHS_DR.View.ChatAppView
{
    /// <summary>
    /// Interaction logic for ChatLayoutUC.xaml
    /// </summary>
    public partial class ChatLayoutUC : UserControl
    {
        public ChatLayoutUC()
        {
            InitializeComponent();
        }
        public Label MessageTitle
        {
            get { return Title; }
            set { Title = value; }
        }
        public Button SendMessageButton
        {
            get { return SendButton; }
            set { SendButton = value; }
        }
        public ScrollViewer ContentScrollViewer
        {
            get { return ContentScroller; }
            set { ContentScroller = value; }
        }
        public TextBox MessageContainer
        {
            get { return MessageContent; }
            set { MessageContent = value; }
        }
        public ItemsControl MessageDisplay
        {
            get { return MessageTemplate; }
            set { MessageTemplate = value; }
        }

    }
}
