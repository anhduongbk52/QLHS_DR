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
    public class G6Class
    {
        private ConcurrentDictionary<int, byte[]> concurrentDictionary_2 = new ConcurrentDictionary<int, byte[]>();
        private User user_0;
        private IReadOnlyList<User> ireadOnlyList_0;
        private EofficeMainServiceClient eemcdrwcfServiceClient_0;

        //Khoi tao
        public G6Class() //Khởi tạo
        {
            //imethod_0(client.ClientCredentials.UserName.UserName);
        }


        //public Task<GInterface2> imethod_0(string userName)
        //{
        //    return concurrentDictionary_0.GetOrAdd(userName, delegate (string string_0)
        //    {
        //        TaskCompletionSource<GInterface2> taskCompletionSource = new TaskCompletionSource<GInterface2>();
        //        new Class13(this, taskCompletionSource, string_0);
        //        return taskCompletionSource.Task;
        //    });
        //}
        private byte[] GetHashPasword()
        {
            byte[] password = Convert.FromBase64String(ServiceProxy.Ins.ChannelFactory.Endpoint.Behaviors.Find<ClientCredentials>().UserName.Password);
            return CryptoUtil.HashPassword(CryptoUtil.GetKeyFromPassword(password), CryptoUtil.GetSaltFromPassword(password));
        }
        private byte[] method_20(byte[] byte_0)
        {
            User user = user_0 ?? (user_0 = ServiceProxy.Ins.GetUserByName(ServiceProxy.Ins.ClientCredentials.UserName.UserName));
            if (user.ECPrKeyForFile == null)
            {
                user.ECPrKeyForFile = ServiceProxy.Ins.GetUserECPrKeyFor(user.Id, ServiceProxy.Ins.ChannelFactory.Endpoint.Behaviors.Find<ClientCredentials>().UserName.Password, ECKeyPurpose.FILE);
            }
            return CryptoUtil.DecryptByDerivedPassword(byte_0, user.ECPrKeyForFile);
        }
        private void SetECPrKeyForMessage(byte[] byte_0, User user_1)
        {
            if (user_1.ECPrKeyForMsg == null)
            {
                user_1.ECPrKeyForMsg = CryptoUtil.GetECSessionKey(byte_0, user_1.ECPuKeyForMsg);
            }
        }
        private void SetECPrKeyForFile(byte[] byte_0, User user_1)
        {
            if (user_1.ECPrKeyForFile == null)
            {
                user_1.ECPrKeyForFile = CryptoUtil.GetECSessionKey(byte_0, user_1.ECPuKeyForFile);
            }
        }


        private byte[] method_7(int taskId)
        {
            UserTask userTask_0 = ServiceProxy.Ins.GetUserTask(user_0.Id, taskId);
            if (userTask_0 != null)
            {
                byte[] array = GetHashPasword();
                if (userTask_0.AssignedBy == user_0.Id)
                {
                    return CryptoUtil.DecryptByDerivedPassword(array, userTask_0.TaskKey);
                }
                User user = ireadOnlyList_0.FirstOrDefault((User user_0) => user_0.Id == userTask_0.AssignedBy);
                if (user.ECPrKeyForFile == null)
                {
                    byte[] byte_ = method_20(array);
                    SetECPrKeyForFile(byte_, user);
                }
                return CryptoUtil.DecryptWithoutIV(user.ECPrKeyForFile, userTask_0.TaskKey);
            }
            return null;
        }
        public void DecrypFile(TaskAttachedFileDTO taskAttachedFileDTO_0)
        {
            byte[] orAdd = concurrentDictionary_2.GetOrAdd(taskAttachedFileDTO_0.TaskId, (int int_0) => method_7(taskAttachedFileDTO_0.TaskId));
            if (orAdd != null)
            {
                taskAttachedFileDTO_0.Content = CryptoUtil.DecryptWithoutIV(orAdd, taskAttachedFileDTO_0.Content);
            }
        }
    }
}
