using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class RevokedPrintedDocumentManagerViewModel : BaseViewModel
    {
        #region "Properties and Field"
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set
            {
                _IsBusy = value; OnPropertyChanged("IsBusy");
            }
        }

        private ObservableCollection<Document> _Documents;
        public ObservableCollection<Document> Documents { get => _Documents; set { _Documents = value; OnPropertyChanged("Documents"); } }
        private TaskAttachedFileDTO _TaskAttachedFileDTO;
        public TaskAttachedFileDTO TaskAttachedFileDTO { get => _TaskAttachedFileDTO; set { _TaskAttachedFileDTO = value; OnPropertyChanged("TaskAttachedFile"); } }
        private ObservableCollection<User> _Users;

        private UserTask _YourUserTask;
        public UserTask YourUserTask
        {
            get => _YourUserTask;
            set
            {
                _YourUserTask = value;
                OnPropertyChanged("YourUserTask");
            }
        }
        private UserTaskPrintManager _SelectedUserTaskPrintManager;
        public UserTaskPrintManager SelectedUserTaskPrintManager
        {
            get => _SelectedUserTaskPrintManager;
            set
            {
                if (_SelectedUserTaskPrintManager != value)
                {
                    _SelectedUserTaskPrintManager = value;

                    if (_TaskAttachedFileDTO != null && _SelectedUserTaskPrintManager != null)
                    {
                        using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                        {
                            MemoryStream outputStream = new MemoryStream();
                            processor.LoadDocument(new MemoryStream(_TaskAttachedFileDTO.Content));
                            List<int> countPrinteds = new List<int>();

                            using (PdfDocumentProcessor target = new PdfDocumentProcessor())
                            {
                                target.CreateEmptyDocument();
                                if (processor.Document.Pages.Count >= _SelectedUserTaskPrintManager.PageNumber)
                                {
                                    target.Document.Pages.Add(processor.Document.Pages[_SelectedUserTaskPrintManager.PageNumber - 1]);
                                    target.SaveDocument(outputStream);
                                    target.CloseDocument();
                                }
                            }
                            if (outputStream != null)
                                DocumentSource = outputStream;
                        }
                    }
                    OnPropertyChanged("SelectedUserTaskPrintManager");
                }
            }
        }
        private ObservableCollection<UserTaskPrintManager> _SelectedUserTaskPrintManagers;
        public ObservableCollection<UserTaskPrintManager> SelectedUserTaskPrintManagers { get => _SelectedUserTaskPrintManagers; set { _SelectedUserTaskPrintManagers = value; OnPropertyChanged("SelectedUserTaskPrintManagers"); } }

        private ObservableCollection<UserTask> _UserTasks;
        public ObservableCollection<UserTask> UserTasks { get => _UserTasks; set { _UserTasks = value; OnPropertyChanged("UserTasks"); } }
        private ObservableCollection<UserTaskPrintManager> _UserTaskPrintManagers;
        public ObservableCollection<UserTaskPrintManager> UserTaskPrintManagers { get => _UserTaskPrintManagers; set { _UserTaskPrintManagers = value; OnPropertyChanged("UserTaskPrintManagers"); } }

        private List<int> _ConfidentialLevels;
        public List<int> ConfidentialLevels { get => _ConfidentialLevels; set { _ConfidentialLevels = value; OnPropertyChanged("ConfidentialLevels"); } }

        private int _ConfidentialLevelSelected;
        public int ConfidentialLevelSelected
        {
            get => _ConfidentialLevelSelected;
            set
            {
                if (_ConfidentialLevelSelected != value)
                {
                    _ConfidentialLevelSelected = value;

                    NotifyPropertyChanged("ConfidentialLevelSelected");
                }
            }
        }
        private bool _VisiblePrintedGridView;
        public bool VisiblePrintedGridView
        {
            get => _VisiblePrintedGridView;
            set
            {
                if (_VisiblePrintedGridView != value)
                {
                    _VisiblePrintedGridView = value;
                    NotifyPropertyChanged("VisiblePrintedGridView");
                }
            }
        }
        private Document _SelectedDocument;
        public Document SelectedDocument
        {
            get => _SelectedDocument;
            set
            {
                if (_SelectedDocument != value)
                {
                    _SelectedDocument = value;
                    try
                    {
                        IsBusy = true;
                        if (_SelectedDocument != null)
                        {
                            if (_SelectedDocument.Task != null)
                            {
                                UserTasks = GetUserTaskOfTask(_SelectedDocument.Task.Id);
                                if (UserTasks != null)
                                {
                                    YourUserTask = UserTasks.Where(x => x.UserId == SectionLogin.Ins.CurrentUser.Id).FirstOrDefault();
                                    if (_YourUserTask != null)
                                    {
                                        TaskAttachedFileDTO = GetTaskAttachedFileDTO(_SelectedDocument.Task.Id);
                                        if (_TaskAttachedFileDTO != null)
                                        {
                                            DecryptTaskAttachedFile(TaskAttachedFileDTO, _YourUserTask);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                    OnPropertyChanged("SelectedDocument");
                }
            }
        }
        private UserTask _SelectedUserTask;
        public UserTask SelectedUserTask
        {
            get => _SelectedUserTask;
            set
            {
                if (_SelectedUserTask != value)
                {
                    _SelectedUserTask = value;
                    if (_SelectedUserTask != null)
                    {
                        UserTaskPrintManagers = _SelectedUserTask.UserTaskPrintManagers.ToObservableCollection();
                    }

                    OnPropertyChanged("SelectedUserTask");
                }
            }
        }
        private Object _DocumentSource;
        public Object DocumentSource
        {
            get => _DocumentSource;
            set
            {
                if (_DocumentSource != value)
                {
                    _DocumentSource = value;
                    NotifyPropertyChanged("DocumentSource");
                }
            }
        }
        private ObservableCollection<Task> _SelectedTasks;
        public ObservableCollection<Task> SelectedTasks
        {
            get => _SelectedTasks;
            set
            {
                _SelectedTasks = value; OnPropertyChanged("SelectedTasks");
            }
        }

        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ConfidentialLevelChangeCommand { get; set; }
        public ICommand ConfirmDisposedCommand { get; set; }
        #endregion
        internal RevokedPrintedDocumentManagerViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ConfidentialLevels = new List<int>() { 1, 2, 3 };
                SelectedDocument = new Document();
                _Users = LoadUsers();
                ConfidentialLevelSelected = 3;
                Documents = GetAllTasksByConfidentialLevel(_ConfidentialLevelSelected);

            });
            ConfidentialLevelChangeCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                Documents = GetAllTasksByConfidentialLevel(_ConfidentialLevelSelected);
            });
            ConfirmDisposedCommand = new RelayCommand<Object>((p) => { if (_SelectedUserTaskPrintManager != null) return true; else return false; }, (p) =>
            {
                int revokedCount;
                if (_SelectedUserTaskPrintManager.PrintedRevoked == null)
                {
                    revokedCount = 0;
                }
                else revokedCount = _SelectedUserTaskPrintManager.PrintedRevoked.Value;
                MessageBoxResult result = MessageBox.Show((_SelectedUserTaskPrintManager.PrintCount - revokedCount)
                                                         + " trang tài liệu số" + _SelectedUserTaskPrintManager.PageNumber
                                                         + " được in bởi " + _SelectedUserTask.User.FullName
                                                         + "vào ngày " + _SelectedUserTaskPrintManager.TimePrint + " đã được hủy ?", "Xác nhận", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    if (_SelectedUserTaskPrintManager.PrintCount > revokedCount)
                    {
                        ConfirmDisposedPrintedDocument(_SelectedUserTaskPrintManager.Id, _SelectedUserTaskPrintManager.PrintCount - revokedCount);
                        MessageBox.Show("Xác nhận thành công");
                        SelectedUserTaskPrintManager.Revoked = true;
                        SelectedUserTaskPrintManager.PrintedRevoked = _SelectedUserTaskPrintManager.PrintCount;
                    }
                    else
                    {
                        MessageBox.Show("Số lượng thu hồi vượt quá số lượng đang lưu hành, vui lòng kiểm tra lại");
                    }
                }
            });

        }
        private void ConfirmDisposedPrintedDocument(int userTaskPrintManagerId, int numberDisposed)
        {
            MessageServiceClient _MyClient = new MessageServiceClient();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                _MyClient.ConfirmDisposedPrintedDocument(userTaskPrintManagerId, numberDisposed);
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
        }
        private ObservableCollection<Document> GetAllTasksByConfidentialLevel(int confidentialLevel)
        {
            ObservableCollection<Document> ketqua = new ObservableCollection<Document>();
            MessageServiceClient _MyClient = new MessageServiceClient();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);

                _MyClient.Open();
                var tasks = _MyClient.LoadTaskByConfidentialLevel(confidentialLevel).OrderByDescending(x => x.Id).ToObservableCollection();
                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        task.UserTasks = GetUserTaskOfTask(task.Id).ToArray();
                        bool isFinish = !task.UserTasks.Any(x => x.UserTaskPrintManagers.Any(y => y.PrintCount > y.PrintedRevoked && y.Success != false));
                        ketqua.Add(new Document()
                        {
                            Task = task,
                            FinishDisposed = isFinish
                        });
                    }
                }

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
            return ketqua;
        }
        private ObservableCollection<User> LoadUsers()
        {
            ObservableCollection<User> ketqua = new ObservableCollection<User>();
            MessageServiceClient _MyClient = new MessageServiceClient();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                ketqua = _MyClient.GetUsers().ToObservableCollection();
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
            return ketqua;
        }

        public void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO, UserTask userTask)
        {
            FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            byte[] orAdd = fileHelper.GetKeyDecryptOfTask(taskAttachedFileDTO.TaskId, userTask);
            if (orAdd != null)
            {
                taskAttachedFileDTO.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO.Content);
            }
        }
        private ObservableCollection<UserTask> GetUserTaskOfTask(int taskId)
        {
            ObservableCollection<UserTask> ketqua = new ObservableCollection<UserTask>();
            List<User> users = new List<User>();
            MessageServiceClient _MyClient = new MessageServiceClient();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);

                _MyClient.Open();
                ketqua = _MyClient.GetAllUserTaskOfTask(taskId).OrderByDescending(x => x.Id).ToObservableCollection();
                if (ketqua.Count > 0 && _Users.Count > 0)
                {
                    foreach (var item in ketqua)
                    {
                        item.User = _Users.Where(x => x.Id == item.UserId).FirstOrDefault();
                        item.UserTaskPrintManagers = _MyClient.GetUserTaskPrintManagers(item.Id).ToArray();
                    }
                }
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
            return ketqua;
        }
        private TaskAttachedFileDTO GetTaskAttachedFileDTO(int taskId)
        {
            TaskAttachedFileDTO ketqua = new TaskAttachedFileDTO();
            MessageServiceClient _MyClient = new MessageServiceClient();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                if (_MyClient.InnerChannel.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    ketqua = _MyClient.GetTaskDocument(taskId);
                    _MyClient.Close();
                }
                else
                {
                    GetTaskAttachedFileDTO(taskId);
                }
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
            return ketqua;
        }

    }
    internal class Document
    {
        public Task Task { get; set; }
        public bool FinishDisposed { get; set; }

    }
}
