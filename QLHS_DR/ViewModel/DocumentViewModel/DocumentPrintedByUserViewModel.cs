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
    internal class DocumentPrintedByUserViewModel : BaseViewModel
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

        private ObservableCollection<DocumentOfMe> _Documents;
        public ObservableCollection<DocumentOfMe> Documents { get => _Documents; set { _Documents = value; OnPropertyChanged("Documents"); } }
        private TaskAttachedFileDTO _TaskAttachedFileDTO;
        public TaskAttachedFileDTO TaskAttachedFileDTO { get => _TaskAttachedFileDTO; set { _TaskAttachedFileDTO = value; OnPropertyChanged("TaskAttachedFile"); } }
        private ObservableCollection<User> _Users;
      
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

        private ObservableCollection<UserTaskPrintManager> _UserTaskPrintManagers;
        public ObservableCollection<UserTaskPrintManager> UserTaskPrintManagers
        { 
            get => _UserTaskPrintManagers; 
            set 
            {
                if(value != _UserTaskPrintManagers)
                {
                    _UserTaskPrintManagers = value;
                    OnPropertyChanged("UserTaskPrintManagers");
                }               
            }
        }

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

        private DocumentOfMe _SelectedDocument;
        public DocumentOfMe SelectedDocument
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
                            if (_SelectedDocument.UserTask != null)
                            {
                                UserTaskPrintManagers = _SelectedDocument.UserTask.UserTaskPrintManagers.ToObservableCollection();
                                if (_SelectedDocument.UserTask != null)
                                {
                                    if(_UserTaskPrintManagers.Count>0)
                                    {
                                        TaskAttachedFileDTO = GetTaskAttachedFileDTO(_SelectedDocument.UserTask.TaskId);
                                        if (_TaskAttachedFileDTO != null)
                                        {
                                            DecryptTaskAttachedFile(TaskAttachedFileDTO, _SelectedDocument.UserTask);
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
        #endregion
        internal DocumentPrintedByUserViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                ConfidentialLevels = new List<int>() { 1, 2, 3 };
                SelectedDocument = new DocumentOfMe();
                _Users = LoadUsers();
                ConfidentialLevelSelected = 3;
                Documents = GetUserTasksByOfMe(_ConfidentialLevelSelected);
            });
            ConfidentialLevelChangeCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                Documents = GetUserTasksByOfMe(_ConfidentialLevelSelected);
            });    
        }
        private ObservableCollection<DocumentOfMe> GetUserTasksByOfMe(int confidentialLevel)
        {
            ObservableCollection<DocumentOfMe> ketqua = new ObservableCollection<DocumentOfMe>();
            MessageServiceClient _MyClient = new MessageServiceClient();
            try
            {
                _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);

                _MyClient.Open();
                var usertasks = _MyClient.LoadUserTaskOfMyByConfidentialLevel(confidentialLevel).OrderByDescending(x => x.Id).ToObservableCollection();
                if (usertasks != null)
                {
                    foreach (var usertask in usertasks)
                    {
                        usertask.UserTaskPrintManagers = _MyClient.GetUserTaskPrintManagers(usertask.Id);
                        usertask.Task = _MyClient.LoadTask(usertask.TaskId);
                        bool isFinish = !usertask.UserTaskPrintManagers.Any(y => y.PrintCount > y.PrintedRevoked && y.Success != false);
                        ketqua.Add(new DocumentOfMe()
                        {
                            UserTask = usertask,
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
    internal class DocumentOfMe
    {
        public UserTask UserTask { get; set; }
        public bool FinishDisposed { get; set; }

    }
}
