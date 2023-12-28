using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class ReceiveUserOfMyDepartmentManagerViewModel : BaseViewModel
    {
        #region "Properties and Field"
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
        //private List<int> _UserNotInTaskIds;
        private readonly Task _Task;
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
        internal ReceiveUserOfMyDepartmentManagerViewModel(Task task)
        {
            _Task = task;
            ReceiveUsers = new ObservableCollection<ReceiveUser>();
            WindowTittle = "Phân công công việc trong đơn vị: " + task.Description + " /-/ " + task.Subject;
            try
            {
                List<User> usersOfDepartment = new List<User>();
                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                Department myDepartment = _MyClient.GetDepartment(SectionLogin.Ins.CurrentUser.Id);
                usersOfDepartment = _MyClient.GetUsersOfDepartment(myDepartment.Id).ToList();
                var allUserTaskOfTask = _MyClient.GetAllUserTaskOfTask(_Task.Id);
                foreach (var u in usersOfDepartment)
                {
                    PermissionType taskPermissions = _MyClient.GetTaskPermissions(u.Id, _Task.Id);
                    ReceiveUser receiveUser = new ReceiveUser()
                    {
                        IsMainProcess = false,
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
                        receiveUser.IsMainProcess = userTask.IsProcessing;
                        receiveUser.IsReceive = true;
                        receiveUser.CanSave = userTask.CanSave.Value;
                        receiveUser.CanPrintFile = taskPermissions.HasFlag(PermissionType.PRINT_DOCUMENT);
                        receiveUser.CanViewFile = userTask.CanViewAttachedFile ?? false; //
                        receiveUser.JobContent = userTask.JobContent;
                    }
                    if (_Task.OwnerUserId == u.Id)
                    {
                        receiveUser.IsReceive = true;
                        receiveUser.CanViewFile = true;
                        receiveUser.ReadOnlyPermission = true;
                    }
                    receiveUser.IsMainProcessChanged = false;
                    receiveUser.IsReceiveChanged = false;
                    receiveUser.CanPrintFileChanged = false;
                    receiveUser.CanViewFileChanged = false;
                    receiveUser.CanSaveChanged = false;
                    receiveUser.PropertyChanged += OnItemPropertyChanged;
                    ReceiveUsers.Add(receiveUser);
                }
                _MyClient.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {

            });
            SaveCommand = new RelayCommand<Window>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                if (_DataChanged)
                {
                    Save();
                    MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
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
                FileHelper fileHelper = new FileHelper(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);

                MessageServiceClient _MyClient = ServiceHelper.NewMessageServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
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
                    List<int> userIdAddIsMainProcess = new List<int>(); //List Id Cac nguoi dung xoa quyen xem file.
                    List<int> userIdRemoveIsMainProcess = new List<int>(); //List Id Cac nguoi dung xoa quyen xem file.
                    List<ReceiveUser> receiveUserChangedJobContent = new List<ReceiveUser>(); //List Id Cac nguoi dung xoa quyen xem file.
                    UserTask userTask_0 = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, _Task.Id);


                    foreach (var receiveuser in ReceiveUsers)
                    {

                        if (receiveuser.IsReceive)
                        {
                            var userTask = allUserTaskOfTask.Where(x => x.UserId == receiveuser.User.Id).FirstOrDefault();
                            if (userTask == null) // User is not in UserTask;
                            {
                                if (receiveuser.User.ECPrKeyForFile == null)
                                {
                                    byte[] masterKey = fileHelper.MasterKey;
                                    byte[] ecpr = fileHelper.DecryptECPrKeyForFile(masterKey);
                                    fileHelper.SetECPrKeyForFile(ecpr, receiveuser.User);
                                }
                                byte[] userTaskKey = new byte[0];

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
                                    IsProcessing = receiveuser.IsMainProcess,
                                    JobContent = receiveuser.JobContent
                                };
                                newUserTasks.Add(newUserTask);
                            }
                            else //Neu nguoi dung da o trong luong cong viec -> thuc hien viec update trang thai cua user;
                            {
                                if (!receiveuser.IsMainProcess && receiveuser.IsMainProcessChanged) //
                                {
                                    userIdRemoveIsMainProcess.Add(receiveuser.User.Id);
                                }
                                if (receiveuser.IsMainProcess && receiveuser.IsMainProcessChanged) //
                                {
                                    userIdAddIsMainProcess.Add(receiveuser.User.Id);
                                }
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
                                if (receiveuser.JobContent != null && receiveuser.JobContentChanged) //Them quyen luu file cua user
                                {
                                    receiveUserChangedJobContent.Add(receiveuser);
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
                    if (receiveUserChangedJobContent.Count > 0)
                    {
                        foreach (var item in receiveUserChangedJobContent)
                        {
                            _MyClient.SetJobContent(_Task.Id, item.User.Id, item.JobContent);
                        }
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
                        _MyClient.SetPrintableUsersInTask(_Task.Id, userIdAddPrintAble.ToArray(), true, SectionLogin.Ins.CurrentUser.Id);
                    }
                    if (userIdRemovePrintAble.Count > 0) //Xoa quyen in file
                    {
                        _MyClient.SetPrintableUsersInTask(_Task.Id, userIdRemovePrintAble.ToArray(), false, SectionLogin.Ins.CurrentUser.Id);
                    }
                    if (userIdAddIsMainProcess.Count > 0)
                    {
                        _MyClient.SetUserTaskIsProcess(_Task.Id, userIdRemovePrintAble.ToArray(), true);
                    }
                    if (userIdRemoveIsMainProcess.Count > 0)
                    {
                        _MyClient.SetUserTaskIsProcess(_Task.Id, userIdRemovePrintAble.ToArray(), false);
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

}
