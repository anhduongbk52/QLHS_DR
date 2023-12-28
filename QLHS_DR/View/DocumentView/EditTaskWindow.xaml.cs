using DevExpress.Pdf;
using EofficeClient.Core;
using EofficeCommonLibrary.Common.Ultil;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.ViewModel.DocumentViewModel;
using System;
using System.IO;
using System.Windows;

namespace QLHS_DR.View.DocumentView
{
    /// <summary>
    /// Interaction logic for EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        private MessageServiceClient _MyClient;
        private Task _Task;
        private TaskAttachedFile _TaskAttachedFile;
        public EditTaskWindow(Task task)
        {
            InitializeComponent();
            _Task = task;
            checkBoxCanSave.IsChecked = task.CanSaveFile;
            textBoxDocumentName.Text = _Task.Subject;
            textBoxMaso.Text = _Task.Description;
            
        }

        private void pdfViwer_DocumentLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void editTaskWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();

                UserTask _UserTaskSelected = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, _Task.Id);
                
                if (_UserTaskSelected != null && _UserTaskSelected.CanViewAttachedFile == true)
                {
                    var taskAttachedFileDTOs = _MyClient.GetTaskDocuments(_Task.Id); //get all file PDF in task
                    if (taskAttachedFileDTOs != null && taskAttachedFileDTOs.Length > 0)
                    {
                        if (taskAttachedFileDTOs[0].KeyFile != null)
                        {
                            cbCapBaoMat.Text = taskAttachedFileDTOs[0].ConfidentialLevel.ToString();
                            taskAttachedFileDTOs[0].Content = AESHelper.DecryptWithoutIV(taskAttachedFileDTOs[0].KeyFile, taskAttachedFileDTOs[0].Content);
                            MemoryStream stream = new MemoryStream(taskAttachedFileDTOs[0].Content);
                            pdfViwer.DocumentSource = stream;
                        }
                    }
                    else
                    {
                       MessageBox.Show("Không tìm thấy file đính kèm");
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa có quyền xem tài liệu này, vui lòng liên hệ quản trị viên!");
                }
                _MyClient.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (ex.InnerException != null)
                {
                    MessageBox.Show(ex.InnerException.Message);
                }
                _MyClient.Abort();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _Task.Subject = textBoxDocumentName.Text;
                _Task.CanSaveFile = checkBoxCanSave.IsChecked;
                _Task.Description = textBoxMaso.Text;
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                _TaskAttachedFile = _MyClient.GetTaskAttachedFile(_Task.Id);
                _TaskAttachedFile.ConfidentialLevel = Convert.ToInt16(cbCapBaoMat.Text);
                
                _MyClient.UpdateTaskAndTaskAttackedFile(_Task, _TaskAttachedFile);
                _MyClient.Close();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (ex.InnerException != null)
                {
                    MessageBox.Show(ex.InnerException.Message);
                }
                _MyClient.Abort();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
