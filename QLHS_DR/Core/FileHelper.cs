using EofficeClient.Core;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.EOfficeServiceReference;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace QLHS_DR.Core
{
    internal class FileHelper
    {
        private EofficeMainServiceClient _MyClient;
        private string _username;
        private string _token;
        private IReadOnlyList<User> _iReadOnlyListUser;
        private byte[] _MasterKey;

        public byte[] MasterKey { get => _MasterKey; set => _MasterKey = value; }

        public FileHelper(string username, string token)
        {
            try
            {
                _username = username;
                _token = token;
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(_username, _token);
                _MyClient.Open();
                this._iReadOnlyListUser = _MyClient.GetUserContacts(SectionLogin.Ins.CurrentUser.UserName);
                _MyClient.Close();
                MasterKey = GetMasterKey();
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
        private byte[] GetMasterKey()
        {
            byte[] password = null;
            byte[] masterKey = null;
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(_username, _token);
                _MyClient.Open();
                password = Convert.FromBase64String(_MyClient.ChannelFactory.Endpoint.Behaviors.Find<ClientCredentials>().UserName.Password);
                _MyClient.Close();
                masterKey = CryptoUtil.HashPassword(CryptoUtil.GetKeyFromPassword(password), CryptoUtil.GetSaltFromPassword(password));
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
            return masterKey;
        }
        internal byte[] GetKeyDecryptOfTask(int taskId)
        {
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                UserTask userTask_0 = _MyClient.GetUserTask(SectionLogin.Ins.CurrentUser.Id, taskId);
                _MyClient.Close();
                if (userTask_0 != null)
                {
                    byte[] masterKey = _MasterKey;
                    if (userTask_0.AssignedBy == SectionLogin.Ins.CurrentUser.Id) //Nếu luồng công việc được tạo bởi _CurrentUser
                    {
                        return CryptoUtil.DecryptByDerivedPassword(masterKey, userTask_0.TaskKey); //lấy key giải mã là của TaskKey của userTask
                    }
                    //Nếu luồng công việc không được tạo bởi _CurrentUser
                    User user = _iReadOnlyListUser.FirstOrDefault((User u) => u.Id == userTask_0.AssignedBy); //Lấy về user chủ nhân của luồng.
                    if (user.ECPrKeyForFile == null)  //Nếu user không có ECPrKeyForFile
                    {
                        byte[] byte_ = DecryptECPrKeyForFile(masterKey);
                        SetECPrKeyForFile(byte_, user);
                    }
                    return CryptoUtil.DecryptWithoutIV(user.ECPrKeyForFile, userTask_0.TaskKey);
                }
                return null;
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
            return null;
        }
        internal byte[] GetKeyDecryptOfTask(int taskId, UserTask userTask)
        {
            try
            {
                if (userTask != null)
                {
                    byte[] masterKey = _MasterKey;
                    if (userTask.AssignedBy == SectionLogin.Ins.CurrentUser.Id) //Nếu luồng công việc được tạo bởi _CurrentUser
                    {
                        return CryptoUtil.DecryptByDerivedPassword(masterKey, userTask.TaskKey); //lấy key giải mã là của TaskKey của userTask
                    }
                    //Nếu luồng công việc không được tạo bởi _CurrentUser
                    User user = _iReadOnlyListUser.FirstOrDefault((User u) => u.Id == userTask.AssignedBy); //Lấy về user chủ nhân của luồng.
                    if (user.ECPrKeyForFile == null)  //Nếu user không có ECPrKeyForFile
                    {
                        byte[] ecpr = DecryptECPrKeyForFile(masterKey);
                        SetECPrKeyForFile(ecpr, user);
                    }
                    return CryptoUtil.DecryptWithoutIV(user.ECPrKeyForFile, userTask.TaskKey);
                }
                return null;
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
            return null;
        }
        public byte[] DecryptECPrKeyForFile(byte[] masterKey)
        {
            byte[] ketqua = null;
            try
            {
                _MyClient = ServiceHelper.NewEofficeMainServiceClient(SectionLogin.Ins.CurrentUser.UserName, SectionLogin.Ins.Token);
                _MyClient.Open();
                QLHS_DR.EOfficeServiceReference.User user = SectionLogin.Ins.CurrentUser ?? (SectionLogin.Ins.CurrentUser = _MyClient.GetUserByName(_MyClient.ClientCredentials.UserName.UserName));
                if (user.ECPrKeyForFile == null)
                {
                    user.ECPrKeyForFile = _MyClient.GetUserECPrKeyFor(user.Id, _MyClient.ChannelFactory.Endpoint.Behaviors.Find<ClientCredentials>().UserName.Password, ECKeyPurpose.FILE);
                }
                ketqua = CryptoUtil.DecryptByDerivedPassword(masterKey, user.ECPrKeyForFile);
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
        public void SetECPrKeyForFile(byte[] byte_0, User user_1)
        {
            if (user_1.ECPrKeyForFile == null)
            {
                user_1.ECPrKeyForFile = CryptoUtil.GetECSessionKey(byte_0, user_1.ECPuKeyForFile);
            }
        }
    }
}
