using AutoUpdaterDotNET;
using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using EofficeClient.ViewModel;
using EofficeClient.ViewModel.DocumentViewModel;
using EofficeCommonLibrary.Common.Util;
using Prism.Events;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.Properties;
using QLHS_DR.View;
using QLHS_DR.View.ContractView;
using QLHS_DR.View.DepartmentView;
using QLHS_DR.View.DocumentView;
using QLHS_DR.View.HosoView;
using QLHS_DR.View.PhanQuyenView;
using QLHS_DR.View.ProductView;
using QLHS_DR.View.UserView;
using QLHS_DR.ViewModel.DocumentViewModel;
using QLHS_DR.ViewModel.HoSoViewModel;
using QLHS_DR.ViewModel.Message;
using QLHS_DR.ViewModel.PhanQuyen;
using QLHS_DR.ViewModel.UserViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace QLHS_DR.ViewModel
{
    class MainViewModel : BaseViewModel,IDisposable
    {
        private BackgroundWorker backgroundWorker;
        ObservableCollection<UserTask> _OldUserTasks;

        string _ConnectionString = "";
        private readonly IEventAggregator _eventAggregator;
        private readonly SubscriptionToken _subscriptionToken;

        #region "Field and properties"

        Notifier notifierForNormalUser;
        MessageOptions optionsForNormalUser;

        private Window window;
        private MessageServiceClient _MyClient;
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
        private bool _IsActiveRibbonPageCategoryQLSP;
        public bool IsActiveRibbonPageCategoryQLSP
        {
            get => _IsActiveRibbonPageCategoryQLSP;
            set
            {
                if (_IsActiveRibbonPageCategoryQLSP != value)
                {
                    _IsActiveRibbonPageCategoryQLSP = value;
                    NotifyPropertyChanged("IsActiveRibbonPageCategoryQLSP");
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
        private string _ActiveTransformerCode;
        public string ActiveTransformerCode
        {
            get => _ActiveTransformerCode;
            set
            {
                if (_ActiveTransformerCode != value)
                {
                    _ActiveTransformerCode = value;
                    NotifyPropertyChanged("ActiveTransformerCode");
                }
            }
        }
        private Product _ActiveProduct;
        public Product ActiveProduct
        {
            get => _ActiveProduct;
            set
            {
                if (_ActiveProduct != value)
                {
                    _ActiveProduct = value;
                    NotifyPropertyChanged("ActiveProduct");
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
        public ICommand CheckUpdateCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand LoadListNewDocument { get; set; }
        public ICommand LoadListCompltetedDocument { get; set; }
        public ICommand LoadListRevokeDocument { get; set; }
        public ICommand OpenChangePassWordWindowCommand { get; set; }
        public ICommand NewTaskCommand { get; set; }
        public ICommand RefeshCommand { get; set; }
        public ICommand LoadAllTask { get; set; }
        public ICommand OpenProductManagerCommand { get; set; }
        public ICommand EditStampCommand { get; set; }
        public ICommand LoadTaskCreateByMe { get; set; }
        public ICommand SetTotalPageOfTaskAttackedFileCommand { get; set; }
        public ICommand OpenRevokedPrintedDocumentManagerCommand { get; set; }
        public ICommand OpenDocumentPrintedByUserWindowCommand { get; set; }
        public ICommand NewProductCommand { get; set; }
        public ICommand AddContract { get; set; }
        public ICommand UploadFileHoSoCommand { get; set; }
        public ICommand UploadApprovalDocumentProductCommand { get; set; }
        public ICommand SelectionChangedTab { get; set; }
        public ICommand OpenBCTDesignManagerCommand { get; set; }
        public ICommand NewBCTDesignManagerCommand{ get; set; }
        public ICommand SignDocumentCommand { get; set; }
        public ICommand OpenListUserCommand { get; set; }
        public ICommand NewUserCommand { get; set; }
        public ICommand OpenPhanQuyenCommand { get; set; }
        public ICommand OpenLoginManagerCommand { get; set; }
        public ICommand NewGroupsUserCommand { get; set; }
        public ICommand OpenDepartmentManagerCommand { get; set; }
        public ICommand OpenFunctionManagerCommand { get; set; }
        public ICommand OpenListGroupCommand { get; set; }
        public ICommand OpenLogsCommand { get; set; }

        #endregion

        private ListNewDocumentUC listNewDocumentUC;
        private UserTaskFinishUC userTaskFinishUC;
        private UserTaskRevokedUC userTaskRevokedUC;
        private AllTaskUC allTaskUC;

        ListNewDocumentViewModel listNewDocumentViewModel;
        TaskCreateByMeViewModel taskCreateByMeViewModel;
        UserTaskFinishViewModel userTaskFinishViewModel;
        UserTaskRevokedViewModel userTaskRevokedViewModel;
        public ICommand NewLsxCommand { get; set; }
        AllTaskViewModel allTaskViewModel;
        public MainViewModel()
        {
            TileApplication = "Quản lý hồ sơ";
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

                    TabContainer item = Workspaces.Where(x => x.Header.Contains("Tài liệu chưa xử lý")).FirstOrDefault();
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
                TileApplication = $"EEMC Office - {version?.Major}.{version?.Minor}.{+version?.Build}.{+version?.Revision}";

                //Login process
                Isloaded = true;
                if (p == null) return;
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
                            _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                            _MyClient.Open();
                            _OldUserTasks = _MyClient.GetUserTaskNotFinish(_CurrentUser.Id).ToObservableCollection();
                            _MyClient.Close();
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

                            //var mapper = new ModelToTableMapper<NortifyUserTask>();
                            //mapper.AddMapping(c => c.Id, "Id");
                            //mapper.AddMapping(c => c.UserId, "UserId");
                            //mapper.AddMapping(c => c.TaskId, "TaskId");
                            //mapper.AddMapping(c => c.AssignedById, "AssignedById");
                            //mapper.AddMapping(c => c.ProductCode, "ProductCode");
                            //mapper.AddMapping(c => c.Subject, "Subject");
                            //mapper.AddMapping(c => c.Actived, "Actived");
                            //mapper.AddMapping(c => c.TimeCreate, "TimeCreate");
                            //_TbRequestSendDocumentDependency = new SqlTableDependency<NortifyUserTask>(_ConnectionString, tableName: "NortifyUserTask", schemaName: "dbo");
                            //_TbRequestSendDocumentDependency.OnChanged += RequestSendDocument_OnChanged;
                            //_TbRequestSendDocumentDependency.OnError += RequestSendDocument_OnError;
                            //_TbRequestSendDocumentDependency.Start();
                        }
                        catch (Exception ex)
                        {
                            _MyClient.Abort();
                            System.Windows.Forms.MessageBox.Show(ex.Message);
                        }

                        p.Show();
                        //LoadDefaultTab();
                    }
                    else
                    { p.Close(); }
                }
            });
            LogoutCommand = new RelayCommand<Window>((p) => { if (p == null) return false; else return true; }, (p) =>
            {
                try
                {
                    ServiceFactory serviceManager = new ServiceFactory();

                    LoginManager loginManager = new LoginManager()
                    {
                        ComputerName = Environment.MachineName,
                        LoginIp = GetLocalIPAddress(),
                        LogType = LoginType.Logout,
                        ApplicationVersion = GetRunningVersion().ToString(),
                        ApplicationName = AppDomain.CurrentDomain.FriendlyName
                    };
                    serviceManager.RecordLogin(loginManager);

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
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            });
            EditStampCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                EditStampViewModel editStampViewModel = new EditStampViewModel();
                EditStampWindow editStampWindow = new EditStampWindow();
                editStampWindow.DataContext = editStampViewModel;
                editStampWindow.ShowDialog();
            });
            RefeshCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                TabContainer item = Workspaces.Where(x => x.IsSelected == true && x.IsVisible).FirstOrDefault();
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
            LoadTaskCreateByMe = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                TabContainer item = Workspaces.Where(x => x.Header.Contains("Tài liệu tạo bởi tôi")).FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.IsVisible = true;
                }
                else
                {
                    taskCreateByMeViewModel = new TaskCreateByMeViewModel(_eventAggregator);
                    listNewDocumentUC.DataContext = taskCreateByMeViewModel;
                    TabContainer tabItemNew = new TabContainer
                    {
                        Header = "Tài liệu tạo bởi tôi",
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
            OpenRevokedPrintedDocumentManagerCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.CanConfirmDisposedPrintedDocument) return true; else return false; }, (p) =>
            {
                RevokedPrintedDocumentManagerWD window = new RevokedPrintedDocumentManagerWD();
                RevokedPrintedDocumentManagerViewModel model = new RevokedPrintedDocumentManagerViewModel();
                window.DataContext = model;
                window.ShowDialog();
            });
            OpenDocumentPrintedByUserWindowCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.CanConfirmDisposedPrintedDocument) return true; else return false; }, (p) =>
            {
                DocumentPrintedByUserWindow window = new DocumentPrintedByUserWindow();
                DocumentPrintedByUserViewModel model = new DocumentPrintedByUserViewModel();
                window.DataContext = model;
                window.ShowDialog();
            });
            OpenProductManagerCommand = new RelayCommand<Window>((p) => { if (SectionLogin.Ins.CanViewListProduct) return true; else return false; }, (p) =>
            {
                ProductManagerUC uc = new ProductManagerUC();
                TabContainer item = Workspaces.Where(x => x.Header == "Products").FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.Content = uc;
                    item.IsVisible = true;
                    item.AllowHide = "true";
                }
                else
                {
                    TabContainer tabItemMain = new TabContainer
                    {
                        Header = "Products",
                        IsSelected = true,
                        AllowHide = "true",
                        IsVisible = true,
                        Content = uc
                    };
                    Workspaces.Add(tabItemMain);
                }
            });
            NewProductCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.CanNewProduct) return true; else return false; }, (p) =>
            {
                NewProductWindow newProductWindow = new NewProductWindow();
                newProductWindow.Show();
            });
            AddContract = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.CanUploadContract) return true; else return false; }, (p) =>
            {
                AddContractWindow addContractWindow = new AddContractWindow();
                ViewModel.ContractViewModel.AddContractViewModel addContractViewModel = new ViewModel.ContractViewModel.AddContractViewModel(_ActiveProduct);
                addContractWindow.DataContext = addContractViewModel;
                addContractWindow.ShowDialog();
            });
            UploadFileHoSoCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.CanUploadTransformerManualFile) return true; else return false; }, (p) =>
            {
                UploadHoSoWindow uploadHoSo = new UploadHoSoWindow();
                UploadHoSoViewModel hoSoViewModel = new UploadHoSoViewModel(null);
                uploadHoSo.DataContext = hoSoViewModel;
                uploadHoSo.ShowDialog();
            });
            UploadApprovalDocumentProductCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.CanUploadTransformerManualFile) return true; else return false; }, (p) =>
            {
                UploadApprovalDocumentProductViewModel uploadApprovalDocumentProductViewModel = new UploadApprovalDocumentProductViewModel(null);
                UploadApprovalDocumentProductWindow uploadApprovalDocumentProductWindow = new UploadApprovalDocumentProductWindow() { DataContext = uploadApprovalDocumentProductViewModel };
                uploadApprovalDocumentProductWindow.ShowDialog();
            });
            NewLsxCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                //LSXView.NewLsxWindow window = new LSXView.NewLsxWindow();
                //window.ShowDialog();
            });
            SignDocumentCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                SignPdfWindow signPdfWindow = new SignPdfWindow();
                signPdfWindow.ShowDialog();
            });          
            CheckUpdateCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
                AutoUpdater.Start(addressUpdateInfo);
            });
            SelectionChangedTab = new RelayCommand<TabContainer>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                var def = p.Content?.GetType();
                if (def?.Name == "ProductUC")
                {
                    //IsActiveRibbonPageCategoryQLSP = true;
                    ActiveTransformerCode = p.Header;

                    try
                    {
                        _MyClient = ServiceHelper.NewMessageServiceClient();
                        _MyClient.Open();
                        ActiveProduct = _MyClient.GetProductByProductCode(_ActiveTransformerCode);

                        _MyClient.Close();
                    }
                    catch (Exception ex)
                    {
                        _MyClient.Abort();
                        System.Windows.MessageBox.Show(ex.Message);
                    }

                }
                else IsActiveRibbonPageCategoryQLSP = false;
            });
            OpenListUserCommand = new RelayCommand<Window>((p) => { if (SectionLogin.Ins.CanViewListUsers) return true; else return false; }, (p) =>
            {
                ListUserUC viewListUser = new ListUserUC();
                Workspaces.Clear();
                TabContainer tabItemMain = new TabContainer
                {
                    Header = "Danh sách user",
                    IsSelected = true,
                    Content = viewListUser,
                    IsVisible = true
                };
                Workspaces.Add(tabItemMain);
            });
            NewUserCommand = new RelayCommand<Window>((p) => { if (SectionLogin.Ins.CanNewUser) return true; else return false; }, (p) =>
            {
                AddNewUserUC addNewUserUC = new AddNewUserUC();
                TabContainer tabItemMain = new TabContainer
                {
                    Header = "Thêm mới user",
                    IsSelected = true,
                    IsVisible = true,
                    Content = addNewUserUC
                };
                TabContainer item = Workspaces.Where(x => x.Header == "Thêm mới user").FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.Content = addNewUserUC;
                    item.IsVisible = true;
                }
                else
                {
                    Workspaces.Add(tabItemMain);
                }
            });
            NewGroupsUserCommand = new RelayCommand<Window>((p) => { if (SectionLogin.Ins.CanNewRole) return true; else return false; }, (p) =>
            {
                NewGroupsUserUC newGroupsUserUC = new NewGroupsUserUC();
                TabContainer tabItemMain = new TabContainer
                {
                    Header = "Thêm nhóm mới",
                    IsSelected = true,
                    IsVisible = true,
                    Content = newGroupsUserUC
                };
                TabContainer item = Workspaces.Where(x => x.Header == "Thêm nhóm mới").FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.Content = newGroupsUserUC;
                    item.IsVisible = true;
                }
                else
                {
                    Workspaces.Add(tabItemMain);
                }
            });
            OpenListGroupCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.CanViewListGroup) return true; else return false; }, (p) =>
            {
                ListGroupUC viewListGroups = new ListGroupUC();
                TabContainer tabItemListUser = new TabContainer()
                {
                    Header = "Danh sách các Group",
                    IsSelected = true,
                    IsVisible = true,
                    Content = viewListGroups
                };
                TabContainer item = Workspaces.Where(x => x.Header == "Danh sách các Group").FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.Content = viewListGroups;
                    item.IsVisible = true;
                }
                else Workspaces.Add(tabItemListUser);
            });
            OpenPhanQuyenCommand = new RelayCommand<Object>((p) => { if (SectionLogin.Ins.CanPermissionManager) return true; else return false; }, (p) =>
            {          
                PhanQuyenWindow phanQuyenWindow = new PhanQuyenWindow() { DataContext = new PhanQuyenViewModel() };               
                phanQuyenWindow.ShowDialog();
            });
            OpenFunctionManagerCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                FunctionsManagerUC tabContent = new FunctionsManagerUC();
                TabContainer tabItemMain = new TabContainer
                {
                    Header = "Danh sách chức năng",
                    IsSelected = true,
                    Content = tabContent,
                    IsVisible = true
                };
                TabContainer item = Workspaces.Where(x => x.Header == "Danh sách chức năng").FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.Content = tabContent;
                    item.IsVisible = true;
                }
                else
                {
                    Workspaces.Add(tabItemMain);
                }
            });
            OpenDepartmentManagerCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                DepartmentManagerUC tabContent = new DepartmentManagerUC();
                TabContainer tabItemMain = new TabContainer
                {
                    Header = "Danh sách phòng ban",
                    IsSelected = true,
                    Content = tabContent,
                    IsVisible = true
                };
                TabContainer item = Workspaces.Where(x => x.Header == "Danh sách phòng ban").FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.Content = tabContent;
                    item.IsVisible = true;
                }
                else
                {
                    Workspaces.Add(tabItemMain);
                }
            });
            OpenLoginManagerCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                LoginManagerWindow tabContent = new LoginManagerWindow();
                TabContainer tabItemMain = new TabContainer
                {
                    Header = "Quản lý đăng nhập",
                    IsSelected = true,
                    Content = tabContent,
                    IsVisible = true
                };
                TabContainer item = Workspaces.Where(x => x.Header == "Quản lý đăng nhập").FirstOrDefault();
                if (item != null)
                {
                    item.IsSelected = true;
                    item.Content = tabContent;
                    item.IsVisible = true;
                }
                else
                {
                    Workspaces.Add(tabItemMain);
                }
            });
            OpenLogsCommand = new RelayCommand<Object>((p) => {if(SectionLogin.Ins.CanViewLogs) return true; else return false; }, (p) =>
            {
                LogView logView = new LogView() { DataContext = new LogViewModel() };
                logView.ShowDialog();
            });
        }

        public void DecryptTaskAttachedFile(TaskAttachedFileDTO taskAttachedFileDTO, UserTask userTask)
        {
            FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
            byte[] orAdd = fileHelper.GetKeyDecryptOfTask(userTask);
            if (orAdd != null)
            {
                taskAttachedFileDTO.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO.Content);
            }
        }
        private void backgroundWorker_0_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 300000; // set the interval to 1 minute
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
            if (SectionLogin.Ins.CurrentUser != null && SectionLogin.Ins.Token != null)
            {
                MessageServiceClient _MyClient1 = new MessageServiceClient();
                try
                {
                    _MyClient1 = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient1.Open();
                    ObservableCollection<UserTask> newUserTasks = _MyClient1.GetUserTaskNotFinish(_CurrentUser.Id).ToObservableCollection();
                    if (newUserTasks != null && _OldUserTasks != null)
                    {
                        if (newUserTasks.Count > _OldUserTasks.Count)
                        {
                            for (int i = _OldUserTasks.Count; i < newUserTasks.Count; i++)
                            {
                                Task task = _MyClient1.LoadTask(newUserTasks[i].TaskId);
                                string message = "Có tài liệu vừa được gửi tới bạn: " + task.Subject;
                                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    notifierForNormalUser.ShowInformation(message, optionsForNormalUser);
                                }));
                            }
                        }
                    }
                    _MyClient1.Close();
                    _OldUserTasks = newUserTasks;
                }
                catch (Exception ex)
                {
                    _MyClient1.Abort();
                }
            }
        }

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
            if (item != null)
            {
                item.Header = message.Title;
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
                try
                {
                    Process.Start(args.ChangelogURL);

                    if (AutoUpdater.DownloadUpdate(args))
                    {
                        window.Close();
                    }
                    //AutoUpdater.BasicAuthChangeLog();
                }
                catch (Exception exception)
                {
                 MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static Version GetRunningVersion()
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

        public void Dispose()
        {
            backgroundWorker?.Dispose();
            backgroundWorker = null;
        }
        #endregion

        public class NortifyUserTask
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int TaskId { get; set; }
            public int AssignedById { get; set; }
            public bool Actived { get; set; }
            public int TimeCreate { get; set; }
            public string ProductCode { get; set; }
            public string Subject { get; set; }

        }
    }
}
