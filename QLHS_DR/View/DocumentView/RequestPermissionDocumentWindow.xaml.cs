using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
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
using System.Windows.Shapes;

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for RequestPermissionDocumentWindow.xaml
    /// </summary>
    public partial class RequestPermissionDocumentWindow : Window
    {
        private Task _Task;
        private UserTask _UserTask;
        private MessageServiceClient _MyClient;
        public RequestPermissionDocumentWindow(int taskId)
        {
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient();
                _MyClient.Open();
                _Task = _MyClient.LoadTask(taskId);
                _UserTask = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, taskId);
                _MyClient.Close();
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                if (ex.InnerException != null)
                {
                    System.Windows.MessageBox.Show(ex.InnerException.Message);
                }
                _MyClient.Abort();
            }
            InitializeComponent();
            lbMaSo.Content = _Task?.Description;
            lbDocumentName.Content = _Task?.Subject;
            checkBoxViewPermission.IsChecked = _UserTask?.CanViewAttachedFile;
            checkBoxPrintPermission.IsChecked = _UserTask?.PermissionType.HasFlag(PermissionType.PRINT_DOCUMENT);
            checkBoxSavePermission.IsChecked = _UserTask?.CanSave;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Ok button
            try
            {
                if(checkBoxPrintPermission.IsChecked==true || checkBoxSavePermission.IsChecked==true||checkBoxViewPermission.IsChecked == true)
                {
                    _MyClient = ServiceHelper.NewMessageServiceClient();
                    _MyClient.Open();
                    RequestPermissionDocument requestPermissionDocument = new RequestPermissionDocument()
                    {
                        CanPrint = checkBoxPrintPermission.IsChecked ?? false,
                        CanSave = checkBoxSavePermission.IsChecked ?? false,
                        CanView = checkBoxViewPermission.IsChecked ?? false,
                        Reason = textBoxReason.Text,
                        TaskId = _Task.Id
                    };
                    _MyClient.NewRequestPermissionDocument(requestPermissionDocument);
                    _MyClient.Close();
                    this.Close();
                }
                else MessageBox.Show("Bạn cần chọn một quyền tối thiểu");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                if (ex.InnerException != null)
                {
                    System.Windows.MessageBox.Show(ex.InnerException.Message);
                }
                _MyClient.Abort();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Cancel button           
            this.Close();
        }
    }
}
