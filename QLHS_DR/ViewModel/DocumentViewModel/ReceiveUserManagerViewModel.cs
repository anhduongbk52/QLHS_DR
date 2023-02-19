using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.Core;
using QLHS_DR.EOfficeServiceReference;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PermissionType = QLHS_DR.EOfficeServiceReference.PermissionType;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class ReceiveUserManagerViewModel:BaseViewModel
    {
        #region "Properties and Field"
        private ConcurrentDictionary<int, byte[]> concurrentDictionary_2 = new ConcurrentDictionary<int, byte[]>();
        private bool _DataChanged;
        public bool DataChanged
        {
            get => _DataChanged;
            set
            {
                if (_DataChanged != value)
                {
                    _DataChanged = value; OnPropertyChanged("DataChanged");
                }
            }
        }
        private IReadOnlyList<User> iReadOnlyListUser;
        //private List<int> _UserNotInTaskIds;
        private EOfficeServiceReference.Task _Task;
        private ObservableCollection<ReceiveUser> _ReceiveUsers;       
        public ObservableCollection<ReceiveUser> ReceiveUsers
        {
            get => _ReceiveUsers;
            set
            {
                if (_ReceiveUsers != value)
                {
                    _ReceiveUsers = value; OnPropertyChanged("ReceiveUsers");
                }
            }
        }
        private string _WindowTittle;    
        public string WindowTittle
        {
            get => _WindowTittle;
            set
            {
                if (_WindowTittle != value)
                {
                    _WindowTittle = value; OnPropertyChanged("WindowTittle");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion
        internal ReceiveUserManagerViewModel(EOfficeServiceReference.Task task)
        {
            _Task = task;
            ReceiveUsers = new ObservableCollection<ReceiveUser>();
            WindowTittle = "Chỉnh sửa người nhận: " + task.Description + " /-/ " +task.Subject;
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                try
                {
                    EofficeMainServiceClient _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);

                    _MyClient.Open();
                    var users = _MyClient.GetUserContacts(SectionLogin.Ins.CurrentUser.UserName);
                    int i = users.Length;

                    var allUserTaskOfTask = _MyClient.GetAllUserTaskOfTask(_Task.Id);
                    iReadOnlyListUser = _MyClient.GetUserContacts(SectionLogin.Ins.CurrentUser.UserName);
                    if (users != null && allUserTaskOfTask != null)
                    {
                        foreach (var u in users)
                        {
                            PermissionType taskPermissions = _MyClient.GetTaskPermissions(u.Id, _Task.Id);
                            ReceiveUser receiveUser = new ReceiveUser()
                            {
                                User = u,
                                IsReceive = false,
                                CanSave = false,
                                CanPrintFile = false,
                                CanViewFile = false,
                                ReadOnlyPermission = false
                            };
                            var userTask = allUserTaskOfTask.Where(x => x.UserId == u.Id).FirstOrDefault();
                            if (userTask != null)
                            {
                                receiveUser.IsReceive = true;
                                if (userTask.CanSave == true)
                                {
                                    receiveUser.CanSave = true;
                                }
                                else receiveUser.CanSave = false;
                                receiveUser.CanPrintFile = taskPermissions.HasFlag(PermissionType.PRINT_DOCUMENT);
                                receiveUser.CanViewFile = userTask.CanViewAttachedFile ?? false; //
                            }
                            if (_Task.OwnerUserId == u.Id)
                            {
                                receiveUser.IsReceive = true;
                                receiveUser.CanViewFile = true;
                                receiveUser.ReadOnlyPermission = true;
                            }
                            receiveUser.IsReceiveChanged = false;
                            receiveUser.CanPrintFileChanged = false;
                            receiveUser.CanViewFileChanged = false;
                            receiveUser.CanSaveChanged = false;
                            receiveUser.PropertyChanged += OnItemPropertyChanged;
                            ReceiveUsers.Add(receiveUser);
                        }
                    }
                    _MyClient.Close();
                    _DataChanged = false;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + "Function: LoadedWindowCommand");
                }



            });
            SaveCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                if(_DataChanged)
                {
                    Save();
                    EofficeMainServiceClient _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                    _MyClient.Open();
                    UserTask myUserTask = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, _Task.Id);

                    if (myUserTask != null && myUserTask.IsFinish != true)
                    {
                        MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Bạn có muốn chuyển công việc này sang mục đã kết thúc ?", "?", MessageBoxButton.OKCancel);
                        if (dialogResult == MessageBoxResult.OK)
                        {
                            _MyClient.SetUserTaskFinish(_Task.Id, SectionLogin.Ins.CurrentUser.Id, true);
                        }
                    }
                    _MyClient.Close();
                }          
                p.Close();
            });          
            CancelCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                p.Close();
            });
        }
        private void Save()
        {
            try
            {
                FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token, iReadOnlyListUser);
               
                EofficeMainServiceClient _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                var allUserTaskOfTask = _MyClient.GetAllUserTaskOfTask(_Task.Id);

                if (ReceiveUsers != null && ReceiveUsers.Count > 0)
                {
                    List<UserTask> newUserTasks = new List<UserTask>(); //List Cac nguoi dung them moi vao luong cong viec.
                    List<int> userIdRemovePrintAble = new List<int>(); //List Id Cac nguoi dung bi xoa quyen in.
                    List<int> userIdAddPrintAble = new List<int>(); //List Id Cac nguoi dung them quyen in.
                    List<int> userIdAddSaveAble = new List<int>(); //List Id Cac nguoi dung them quyen save.
                    List<int> userIdRemoveSaveAble = new List<int>(); //List Id Cac nguoi dung xoa quyen save.
                    List<int> userIdAddViewFileAble = new List<int>(); //List Id Cac nguoi dung them quyen xem file.
                    List<int> userIdRemoveViewFileAble = new List<int>(); //List Id Cac nguoi dung xoa quyen xem file.

                    byte[] keyDecryptOfTask = fileHelper.GetKeyDecryptOfTask(_Task.Id);
                    byte[] data = concurrentDictionary_2.GetOrAdd(_Task.Id, (int int_1) => keyDecryptOfTask);

                    foreach (var receiveuser in ReceiveUsers)
                    {

                        if (receiveuser.IsReceive)
                        {
                            var userTask = allUserTaskOfTask.Where(x => x.UserId == receiveuser.User.Id).FirstOrDefault();
                            if (userTask == null) // User is not in UserTask;
                            {
                                if (receiveuser.User.ECPrKeyForFile == null)
                                {
                                    byte[] hassPasword = fileHelper.HashPasword; //Ton thoi gian
                                    byte[] array = fileHelper.method_20(hassPasword);
                                    fileHelper.SetECPrKeyForFile(array, receiveuser.User);
                                }
                                byte[] userTaskKey = CryptoUtil.EncryptWithoutIV(receiveuser.User.ECPrKeyForFile, data);
                                //_MyClient.AddUserTask(_Task.Id, receiveuser.User.Id, SectionLogin.Ins.CurrentUser.Id, userTaskKey);

                                PermissionType permissionType = new PermissionType();
                                if (receiveuser.CanPrintFile)
                                {
                                    permissionType = PermissionType.PRINT_DOCUMENT;
                                }
                                else permissionType = PermissionType.NONE;

                                UserTask newUserTask = new UserTask()
                                {
                                    UserId = receiveuser.User.Id,
                                    TaskId = _Task.Id,
                                    AssignedBy = SectionLogin.Ins.CurrentUser.Id,
                                    PermissionType = permissionType,
                                    Seen = false,
                                    HasPrinted = false,
                                    IsFinish = false,

                                    CanSave = receiveuser.CanSave,
                                    CanViewAttachedFile = receiveuser.CanViewFile,
                                    TaskKey = userTaskKey,
                                };
                                newUserTasks.Add(newUserTask);
                                //_UserNotInTaskIds.Add(receiveuser.User.Id);
                            }
                            else //Neu nguoi dung da o trong luong cong viec -> thuc hien viec update trang thai cua user;
                            {
                                if (!receiveuser.CanViewFile && receiveuser.CanViewFileChanged) //Xoa quyen xem file cua user
                                {
                                    userIdRemoveViewFileAble.Add(receiveuser.User.Id);
                                }
                                if (receiveuser.CanViewFile && receiveuser.CanViewFileChanged) //Them quyen xem file cua user
                                {
                                    userIdAddViewFileAble.Add(receiveuser.User.Id);
                                }
                                if (!receiveuser.CanSave && receiveuser.CanSaveChanged) //Xoa quyen luu file cua user
                                {
                                    userIdRemoveSaveAble.Add(receiveuser.User.Id);
                                }
                                if (receiveuser.CanSave && receiveuser.CanSaveChanged) //Them quyen luu file cua user
                                {
                                    userIdAddSaveAble.Add(receiveuser.User.Id);
                                }
                                if (!receiveuser.CanPrintFile && receiveuser.CanPrintFileChanged) //Xoa quyen luu file cua user
                                {
                                    userIdRemovePrintAble.Add(receiveuser.User.Id);
                                }
                                if (receiveuser.CanPrintFile && receiveuser.CanPrintFileChanged) //Them quyen luu file cua user
                                {
                                    userIdAddPrintAble.Add(receiveuser.User.Id);
                                }
                            }
                        }
                        else //Xoa user trong luong cong viec
                        {
                            if (receiveuser.IsReceiveChanged == true)
                            {
                                _MyClient.DeleteUserTask(_Task.Id, receiveuser.User.Id, SectionLogin.Ins.CurrentUser.Id);
                            }
                        }
                    }
                    //_MyClient.SetPrintableUsersInTask(_Task.Id, _UserIds.ToArray(), receiveuser.CanPrintFile, SectionLogin.Ins.CurrentUser.Id);
                    //Them cac nguoi dung chua co vao luong cuong viec
                    if (newUserTasks.Count > 0)
                    {
                        _MyClient.AddUserTasks(newUserTasks.ToArray());
                    }

                    if (userIdAddViewFileAble.Count > 0) //Set quyen view file
                    {
                        _MyClient.SetUserTaskViewFileAble(_Task.Id, userIdAddViewFileAble.ToArray(), true, SectionLogin.Ins.CurrentUser.Id);
                    }
                    if (userIdRemoveViewFileAble.Count > 0) //Xoa quyen view file
                    {
                        _MyClient.SetUserTaskViewFileAble(_Task.Id, userIdRemoveViewFileAble.ToArray(), false, SectionLogin.Ins.CurrentUser.Id);
                    }
                    if (userIdAddSaveAble.Count > 0) //Set quyen save file
                    {
                        _MyClient.SetUserTaskSaveFileAble(_Task.Id, userIdAddSaveAble.ToArray(), true, SectionLogin.Ins.CurrentUser.Id);
                    }
                    if (userIdRemoveSaveAble.Count > 0) //Xoa quyen save file
                    {
                        _MyClient.SetUserTaskSaveFileAble(_Task.Id, userIdRemoveSaveAble.ToArray(), false, SectionLogin.Ins.CurrentUser.Id);
                    }
                    if (userIdAddPrintAble.Count > 0) //Set quyen in file
                    {
                        _MyClient.SetUserTaskSaveFileAble(_Task.Id, userIdAddPrintAble.ToArray(), true, SectionLogin.Ins.CurrentUser.Id);
                    }
                    if (userIdRemovePrintAble.Count > 0) //Xoa quyen in file
                    {
                        _MyClient.SetUserTaskSaveFileAble(_Task.Id, userIdRemovePrintAble.ToArray(), false, SectionLogin.Ins.CurrentUser.Id);
                    }
                }
                _MyClient.Close();
               
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }
        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _DataChanged = true;
        }
    }
    internal class ReceiveUser : BaseViewModel
    {
        private bool _IsReceive;
        public bool IsReceive
        {
            get => _IsReceive;
            set
            {
                if (_IsReceive != value)
                {                   
                    _IsReceive = value;
                    if (!_IsReceive)
                    {
                        IsReceive = false;
                        CanViewFile = false;
                        CanPrintFile = false;
                        CanSave = false;
                    }
                    OnPropertyChanged("IsReceive");
                    IsReceiveChanged=true;
                }
            }
        }
        private bool _IsReceiveChanged;
        public bool IsReceiveChanged
        {
            get => _IsReceiveChanged;
            set
            {
                if (_IsReceiveChanged != value)
                {
                    _IsReceiveChanged = value; OnPropertyChanged("IsReceiveChanged");
                }
            }
        }
        private bool _CanSave;
        public bool CanSave
        {
            get => _CanSave;
            set
            {
                if (_CanSave != value)
                {
                    _CanSave = value;
                    if (_CanSave)
                    {
                        IsReceive = true;
                        CanViewFile = true;
                        CanPrintFile = true;
                    }                   
                    OnPropertyChanged("CanSave");
                    CanSaveChanged = true;
                }
            }
        }
        private bool _CanSaveChanged;
        public bool CanSaveChanged
        {
            get => _CanSaveChanged;
            set
            {
                if (_CanSaveChanged != value)
                {
                    _CanSaveChanged = value; OnPropertyChanged("CanSaveChanged");
                }
            }
        }
        private bool _CanViewFile;
        public bool CanViewFile
        {
            get => _CanViewFile;
            set
            {
                if (_CanViewFile != value)
                {
                    _CanViewFile = value; OnPropertyChanged("CanViewFile");
                    CanViewFileChanged = true;
                }
            }
        }
        private bool _CanViewFileChanged;
        public bool CanViewFileChanged
        {
            get => _CanViewFileChanged;
            set
            {
                if (_CanViewFileChanged != value)
                {
                    _CanViewFileChanged = value; OnPropertyChanged("CanViewFileChanged");
                }
            }
        }
        private bool _CanPrintFile;
        public bool CanPrintFile
        {
            get => _CanPrintFile;
            set
            {
                if (_CanPrintFile != value)
                {
                    _CanPrintFile = value;
                    if (_CanPrintFile)
                    {
                        IsReceive = true;
                        CanViewFile = true;                        
                    }
                    OnPropertyChanged("CanPrintFile");
                    CanPrintFileChanged=true;
                }
            }
        }
        private bool _CanPrintFileChanged;
        public bool CanPrintFileChanged
        {
            get => _CanPrintFileChanged;
            set
            {
                if (_CanPrintFileChanged != value)
                {
                    _CanPrintFileChanged = value; 
                    OnPropertyChanged("CanPrintFileChanged");
                }
            }
        }
        private bool _ReadOnlyPermission;
        public bool ReadOnlyPermission
        {
            get => _ReadOnlyPermission;
            set
            {
                if (_ReadOnlyPermission != value)
                {
                    _ReadOnlyPermission = value; 
                    OnPropertyChanged("ReadOnlyPermission");
                }
            }
        }
        private User _User;
        public User User
        {
            get => _User;
            set
            {
                if (_User != value)
                {
                    _User = value; OnPropertyChanged("User");
                }
            }
        }
    }
}
