using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Input;
using System.Deployment.Application;
using QLHS_DR.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.View;
using EofficeClient.ViewModel;
using QLHS_DR.View.DocumentView;
using EofficeClient.Core;
using QLHS_DR.EOfficeServiceReference;
using DevExpress.Mvvm.Native;
using EofficeClient.ViewModel.DocumentViewModel;
using AutoUpdaterDotNET;
using System.Windows.Forms;
using QLHS_DR.Properties;
using QLHS_DR.ViewModel.DocumentViewModel;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using TableDependency.SqlClient;
using DevExpress.Data.TreeList;
using TableDependency.SqlClient.Base.EventArgs;
using ToastNotifications.Messages;
using Prism.Events;
using QLHS_DR.ViewModel.Message;
using Prism.Commands;
using System.Configuration;
using System.Data.Common;
using System.ComponentModel;
using System.Threading;
using System.Timers;

namespace QLHS_DR.ViewModel
{
    class MainViewModel:BaseViewModel
    {        
        private BackgroundWorker backgroundWorker;
        ObservableCollection<UserTask> _OldUserTasks;

        string _ConnectionString = @"data source=***;initial catalog=EEMCDR;persist security info=true;user id=***;password=***";
        private readonly IEventAggregator _eventAggregator;
        private readonly SubscriptionToken _subscriptionToken;

        #region "Field and properties"

        Notifier notifierForNormalUser;
        MessageOptions optionsForNormalUser;
        private SqlTableDependency<UserTask> _TbRequestSendDocumentDependency;

        private Window window;
        private EofficeMainServiceClient _MyClient;
        private ObservableCollection<TabContainer> _Workspaces;
        public ObservableCollection<TabContainer> Workspaces
        {
            get => _Workspaces;
            set
            {
                if (_Workspaces != value)
                {
                    _Workspaces = value;
                    NotifyPropertyChanged("Workspaces");
                }
            }
        }      

        private User _CurrentUser;
        public User CurrentUser { get => _CurrentUser; set { _CurrentUser = value; OnPropertyChanged("CurrentUser"); } }

        public bool Isloaded = false;
        private string _TileApplication;
        public string TileApplication
        {
            get => _TileApplication;
            set
            {
                if (_TileApplication != value)
                {
                    _TileApplication = value;
                    NotifyPropertyChanged("TileApplication");
                }
            }
        }

        private string _ContentButtonNotCompleted;
        public string ContentButtonNotCompleted
        {
            get => _ContentButtonNotCompleted;
            set
            {
                if (_ContentButtonNotCompleted != value)
                {
                    _ContentButtonNotCompleted = value;
                    NotifyPropertyChanged("ContentButtonNotCompleted");
                }
            }
        }
        private string _ContentButtonCompleted;
        public string ContentButtonCompleted
        {
            get => _ContentButtonCompleted;
            set
            {
                if (_ContentButtonCompleted != value)
                {
                    _ContentButtonCompleted = value;
                    NotifyPropertyChanged("ContentButtonCompleted");
                }
            }
        }
        private string _ContentButtonRevoke;
        public string ContentButtonRevoke
        {
            get => _ContentButtonRevoke;
            set
            {
                if (_ContentButtonRevoke != value)
                {
                    _ContentButtonRevoke = value;
                    NotifyPropertyChanged("ContentButtonRevoke");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand LoadListNewDocument { get; set; }
        public ICommand LoadListCompltetedDocument { get; set; }
        public ICommand LoadListRevokeDocument { get; set; }
        public ICommand OpenChangePassWordWindowCommand { get; set; }
        public ICommand NewTaskCommand { get; set; }
        public ICommand RefeshCommand { get; set; }
        public ICommand LoadAllTask { get; set; }

        #endregion
        
        private ListNewDocumentUC listNewDocumentUC;
        private UserTaskFinishUC userTaskFinishUC;
        private UserTaskRevokedUC userTaskRevokedUC;
        private AllTaskUC allTaskUC;

        ListNewDocumentViewModel listNewDocumentViewModel;
        UserTaskFinishViewModel userTaskFinishViewModel;
        UserTaskRevokedViewModel userTaskRevokedViewModel;
        AllTaskViewModel allTaskViewModel;
        public MainViewModel()
        {
           

            //--------------------Message between Viewmodels------------------------//

            _eventAggregator = new EventAggregator();
            allTaskUC = new AllTaskUC();
            listNewDocumentUC = new ListNewDocumentUC();
            userTaskFinishUC = new UserTaskFinishUC();
            userTaskRevokedUC = new UserTaskRevokedUC();

            // Lấy sự kiện TitleTabControlMessage từ EventAggregator
            _eventAggregator.GetEvent<NewTasksTabTitleChangedEvent>().Subscribe(HandleNewTasksTabTitleChangedMessage);
            _eventAggregator.GetEvent<FinishTasksTabTitleChangedEvent>().Subscribe(HandleFinishTasksTabTitleChangedMessage);
            _eventAggregator.GetEvent<RevokedTasksTabTitleChangedEvent>().Subscribe(HandleRevokedTasksTabTitleChangedMessage);
            _eventAggregator.GetEvent<AllTaskTabTitleChangedEvent>().Subscribe(HandleAllTaskTabTitleChangedMessage);          
            ///////////////////////////////
            notifierForNormalUser = new Notifier(cfg =>
            {
                cfg.PositionProvider = new PrimaryScreenPositionProvider(corner: Corner.BottomRight, offsetX: 10, offsetY: 10);
                cfg.LifetimeSupervisor = new CountBasedLifetimeSupervisor(maximumNotificationCount: MaximumNotificationCount.UnlimitedNotifications());
                cfg.DisplayOptions.TopMost = true; // set the option to show notifications over other windows
                cfg.DisplayOptions.Width = 350; // set the notifications width
                cfg.Dispatcher = System.Windows.Application.Current.Dispatcher;
            });
            optionsForNormalUser = new MessageOptions
            {
                FontSize = 18, // set notification font size
                ShowCloseButton = true, // set the option to show or hide notification close button
                Tag = "Any object or value which might matter in callbacks",
                FreezeOnMouseEnter = true, // set the option to prevent notification dissapear automatically if user move cursor on it
                NotificationClickAction = n =>
                {
                    n.Close();
                    window.Show();
                    window.WindowState = System.Windows.WindowState.Normal;

                    TabContainer item = Workspaces.Where(x => x.Header.Contains( "Tài liệu chưa xử lý")).FirstOrDefault();
                    if (item != null)
                    {
                        item.IsSelected = true;
                        item.IsVisible = true;
                    }
                    else
                    {
                        //listNewDocumentUC.DataContext = listNewDocumentViewModel;
                        TabContainer tabItemNew = new TabContainer
                        {
                            Header = "Tài liệu chưa xử lý",
                            AllowHide = "true",
                            IsSelected = true,
                            IsVisible = true,
                            Content = listNewDocumentUC
                        };
                        Workspaces.Add(tabItemNew);
                    }
                }
            };
            //Settings for update
            string addressUpdateInfo = Settings.Default.AddressUpdateInfo;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start(addressUpdateInfo);

            Workspaces = new ObservableCollection<TabContainer>();

            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {              
                window = p;
                //Load current version
                Version version = GetRunningVersion();
                TileApplication = "EEMC Office - " + version?.Major + "." + version?.Minor + "." + version?.Build + "." + version?.Revision;

                //Login process
                Isloaded = true;
                if (p == null) return;
                CertificateValidation();
                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                if (loginWindow.DataContext == null) return;
                var loginVM = loginWindow.DataContext as LoginViewModel;
                if (loginVM != null)
                {
                    if (loginVM.IsLogin)
                    {
                        if (loginVM.User != null)
                        {                           
                            CurrentUser = loginVM.User;                           
                        }
                        try
                        {
                            EofficeMainServiceClient _client = ServiceHelper.NewEofficeMainServiceClient(_CurrentUser.UserName, SectionLogin.Ins.Token);
                            _client.Open();
                            _OldUserTasks = _client.GetUserTaskNotFinish(_CurrentUser.Id).ToObservableCollection();
                            _client.Close();

                            //--------------------Check New Message------------------------//
                            backgroundWorker = new BackgroundWorker
                            {
                                WorkerSupportsCancellation = true
                            };
                            backgroundWorker.DoWork += backgroundWorker_0_DoWork;
                            if (!backgroundWorker.IsBusy)
                            {
                                backgroundWorker.RunWorkerAsync();
                            }
                        }
                        catch(Exception ex)
                        { 
                            System.Windows.Forms.MessageBox.Show(ex.Message);
                        }
                       
                        //_TbRequestSendDocumentDependency = new SqlTableDependency<UserTask>(_ConnectionString, tableName: "UserTask", schemaName: "dbo");
                        //_TbRequestSendDocumentDependency.OnChanged += RequestSendDocument_OnChanged;                                                  
                        //_TbRequestSendDocumentDependency.Start();
                        p.Show();
                        LoadDefaultTab();
                    }
                    else
                    { p.Close(); }
                }
            });
            LogoutCommand = new RelayCommand<Window>((p) => { if (p == null) return false; else return true; }, (p) =>
            {              
                Isloaded = false;
                ConfigurationUtil.RemoveCreditalData(AppInfo.FolderPath);                
                SectionLogin.Ins = null;              

                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                if (loginWindow.DataContext == null) return;
                var loginVM = loginWindow.DataContext as LoginViewModel;
                if (loginVM != null)
                {
                    if (loginVM.IsLogin)
                    {
                        if (loginVM.User != null)
                        {
                            //SectionLogin.Ins.CurrentUser = loginVM.User;
                            CurrentUser = loginVM.User;
                        }
                        p.Show();
                        LoadDefaultTab();
                    }
                    else
                    { p.Close(); }
                }
            });
            RefeshCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                TabContainer item = Workspaces.Where(x => x.IsSelected==true && x.IsVisible).FirstOrDefault();
                if (item != null)
                {
                    if (item.Header.Contains("Tài liệu chưa xử lý"))
                    {
                        _eventAggregator.GetEvent<ReloadNewTasksTabEvent>().Publish(new object());
                    }
                    if (item.Header.Contains("Tài liệu đã xử lý"))
                    {
                        _eventAggregator.GetEvent<ReloadFinishTasksTabEvent>().Publish(new object());
                    }
                    if (item.Header.Contains("Tài liệu đã thu hồi"))
                    {
                        _eventAggregator.GetEvent<ReloadRevokedTasksTabEvent>().Publish(new object());
                    }
                    if (item.Header.Contains("All"))
                    {                       
                        _eventAggregator.GetEvent<ReloadAllTaskTabEvent>().Publish(new object());
                    }
                }
            });
            LoadListNewDocument = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {           
                TabContainer item = Workspaces.Where(x => x.Header.Contains("Tài liệu chưa xử lý")).FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.IsVisible = true;
                }
                else
                {
                    listNewDocumentViewModel = new ListNewDocumentViewModel(_eventAggregator);
                    listNewDocumentUC.DataContext = listNewDocumentViewModel;
                    TabContainer tabItemNew = new TabContainer
                    {
                        Header = "Tài liệu chưa xử lý",
                        AllowHide = "true",
                        IsSelected = true,
                        IsVisible = true,
                        Content = listNewDocumentUC
                    };
                    Workspaces.Add(tabItemNew);
                }

            });
            LoadListCompltetedDocument = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                TabContainer item = Workspaces.Where(x => x.Header.Contains("Tài liệu đã xử lý")).FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.IsVisible = true;
                }
                else
                {
                    userTaskFinishViewModel = new UserTaskFinishViewModel(_eventAggregator);
                    userTaskFinishUC.DataContext = userTaskFinishViewModel;
                    TabContainer tabItemNew = new TabContainer
                    {
                        Header = "Tài liệu đã xử lý",
                        AllowHide = "true",
                        IsSelected = true,
                        IsVisible = true,
                        Content = userTaskFinishUC
                    };
                    Workspaces.Add(tabItemNew);
                }
            });
            LoadListRevokeDocument = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                TabContainer item = Workspaces.Where(x => x.Header.Contains("Tài liệu đã thu hồi")).FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.IsVisible = true;
                }
                else
                {
                    userTaskRevokedViewModel = new UserTaskRevokedViewModel(_eventAggregator);
                    userTaskRevokedUC.DataContext = userTaskRevokedViewModel;
                    TabContainer tabItemNew = new TabContainer
                    {
                        Header = "Tài liệu đã thu hồi",
                        AllowHide = "true",
                        IsSelected = true,
                        IsVisible = true,
                        Content = userTaskRevokedUC
                    };
                    Workspaces.Add(tabItemNew);
                }
            });
            LoadAllTask = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                TabContainer item = Workspaces.Where(x => x.Header.Contains("All")).FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.IsVisible = true;
                }
                else
                {
                    allTaskViewModel = new AllTaskViewModel(_eventAggregator);
                    allTaskUC.DataContext = allTaskViewModel;
                    TabContainer tabItemNew = new TabContainer
                    {
                        Header = "All",
                        AllowHide = "true",
                        IsSelected = true,
                        IsVisible = true,
                        Content = allTaskUC
                    };
                    Workspaces.Add(tabItemNew);
                }

            });
            OpenChangePassWordWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                View.UserView.UserChangePasswordWindow userChangePasswordWindow = new View.UserView.UserChangePasswordWindow();
                ViewModel.UserViewModel.UserChangePasswordViewModel userChangePasswordViewModel = new UserViewModel.UserChangePasswordViewModel();
                userChangePasswordWindow.DataContext = userChangePasswordViewModel;
                userChangePasswordWindow.ShowDialog();
            });
            NewTaskCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.Permissions.HasFlag(PermissionType.CREATE_TASK)) return true; else return false; }, (p) =>
            {
                NewTaskWindow newTaskWindow = new NewTaskWindow();
                NewTaskViewModel newTaskViewModel = new NewTaskViewModel();
                newTaskWindow.DataContext = newTaskViewModel;
                newTaskWindow.ShowDialog();
            });
        }

        private void backgroundWorker_0_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 60000; // set the interval to 1 minute
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                EofficeMainServiceClient _client = ServiceHelper.NewEofficeMainServiceClient(_CurrentUser.UserName, SectionLogin.Ins.Token);
                _client.Open();
                ObservableCollection<UserTask> newUserTasks = _client.GetUserTaskNotFinish(_CurrentUser.Id).ToObservableCollection();
                if (newUserTasks != null && _OldUserTasks != null)
                {
                    if (newUserTasks.Count > _OldUserTasks.Count)
                    {
                        for (int i = _OldUserTasks.Count; i < newUserTasks.Count; i++)
                        {
                            EOfficeServiceReference.Task task = _client.LoadTask(newUserTasks[i].TaskId);
                            string message = "Có tài liệu vừa được gửi tới bạn: " + task.Subject;
                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                notifierForNormalUser.ShowInformation(message, optionsForNormalUser);
                            }));
                        }
                    }
                }
                _client.Close();
                _OldUserTasks = newUserTasks;                
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        //private void RequestSendDocument_OnChanged(object sender, RecordChangedEventArgs<UserTask> e)
        //{
        //    try
        //    {
        //        if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
        //        {
        //            var entry = e.Entity;
        //            if (e.ChangeType == TableDependency.SqlClient.Base.Enums.ChangeType.Insert && entry.UserId==_CurrentUser.Id)
        //            {
        //                _MyClient = ServiceHelper.NewEofficeMainServiceClient(_CurrentUser.UserName, SectionLogin.Ins.Token);
        //                _MyClient.Open();
        //                EOfficeServiceReference.Task task = _MyClient.LoadTask(entry.TaskId);
        //                 _MyClient.Close();
        //                string message = "Có tài liệu vừa được gửi tới bạn: " + task.Subject;

        //                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
        //                {
        //                    notifierForNormalUser.ShowInformation(message, optionsForNormalUser);
        //                }));
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show(ex.Message);
        //    }

        //}
        #region "Function"
        private void LoadDefaultTab()
        {
            listNewDocumentViewModel = new ListNewDocumentViewModel(_eventAggregator);
            userTaskFinishViewModel = new UserTaskFinishViewModel(_eventAggregator);
            userTaskRevokedViewModel = new UserTaskRevokedViewModel(_eventAggregator);
            allTaskViewModel = new AllTaskViewModel(_eventAggregator);

            Workspaces.Clear();

            listNewDocumentUC.DataContext = listNewDocumentViewModel;
            userTaskFinishUC.DataContext = userTaskFinishViewModel;
            userTaskRevokedUC.DataContext = userTaskRevokedViewModel;
            allTaskUC.DataContext = allTaskViewModel;

            TabContainer tabItemNew = new TabContainer
            {
                Header = "Tài liệu chưa xử lý",
                AllowHide = "true",
                IsSelected = true,
                IsVisible = true,
                Content = listNewDocumentUC
            };
            TabContainer tabItemCompleted = new TabContainer
            {
                Header = "Tài liệu đã xử lý",
                AllowHide = "true",
                IsSelected = false,
                IsVisible = true,
                Content = userTaskFinishUC
            };
            TabContainer tabItemRevoke = new TabContainer
            {
                Header = "Tài liệu đã thu hồi",
                AllowHide = "true",
                IsSelected = false,
                IsVisible = true,
                Content = userTaskRevokedUC
            };
            TabContainer tabItemAll = new TabContainer
            {
                Header = "All",
                AllowHide = "true",
                IsSelected = false,
                IsVisible = true,
                Content = allTaskUC
            };
            Workspaces.Add(tabItemNew);
            Workspaces.Add(tabItemCompleted);
            Workspaces.Add(tabItemRevoke);
            Workspaces.Add(tabItemAll);
        }
        private void HandleNewTasksTabTitleChangedMessage(TitletabControlMessage message)
        {
            TabContainer item = Workspaces.Where(x => x.Header.Contains("Tài liệu chưa xử lý")).FirstOrDefault();
            if(item!=null)
            {
                item.Header= message.Title;
            }           
        }
        private void HandleFinishTasksTabTitleChangedMessage(TitletabControlMessage message)
        {
            TabContainer item = Workspaces.Where(x => x.Header.Contains("Tài liệu đã xử lý")).FirstOrDefault();
            if (item != null)
            {
                item.Header = message.Title;
            }
        }
        private void HandleRevokedTasksTabTitleChangedMessage(TitletabControlMessage message)
        {
            TabContainer item = Workspaces.Where(x => x.Header.Contains("Tài liệu đã thu hồi")).FirstOrDefault();
            if (item != null)
            {
                item.Header = message.Title;
            }
        }
        private void HandleAllTaskTabTitleChangedMessage(TitletabControlMessage message)
        {
            TabContainer item = Workspaces.Where(x => x.Header.Contains("All")).FirstOrDefault();
            if (item != null)
            {
                item.Header = message.Title;
            }
        }
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.IsUpdateAvailable)
            {
                DialogResult dialogResult;
                dialogResult = System.Windows.Forms.MessageBox.Show($@"Bạn ơi, phần mềm của bạn có phiên bản mới {args.CurrentVersion}. Phiên bản bạn đang sử dụng hiện tại  {args.InstalledVersion}. Bạn có muốn cập nhật phần mềm không?", @"Cập nhật phần mềm",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
                {
                    try
                    {
                        if (AutoUpdater.DownloadUpdate(args))
                        {
                            window.Close();
                        }
                    }
                    catch (Exception exception)
                    {
                        System.Windows.Forms.MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            //else
            //{
            //    System.Windows.Forms.MessageBox.Show(@"Phiên bản bạn đang sử dụng đã được cập nhật mới nhất.", @"Cập nhật phần mềm",
            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }
        private Version GetRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
        private void CertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(IsSSL));
        }
        #endregion
        private static bool IsSSL(object A_0, X509Certificate A_1, X509Chain A_2, SslPolicyErrors A_3)
        {
            return true;
        }        
    }
}
