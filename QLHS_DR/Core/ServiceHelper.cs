
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.EofficeMainServiceReference;
using QLHS_DR.ViewModel.ChatAppViewModel;
using System;
using System.ServiceModel;
using System.Windows;

namespace EofficeClient.Core
{
    internal class ServiceHelper
    {
        private const string protocol = "https";

        private const string ipAddress = "192.168.11.12";

        private const int port = 8888;

        private const string servicePath = "EEMCDRWcfService/service";
        //public static EofficeMainServiceClient NewEofficeMainServiceClient()
        //{
        //    EofficeMainServiceClient client = new EofficeMainServiceClient(new InstanceContext(new MessageServiceCallBack()), "NetTcpBinding_IEofficeMainService");
        //    client.ClientCredentials.UserName.UserName = SectionLogin.Ins.CurrentUser.UserName;
        //    client.ClientCredentials.UserName.Password = SectionLogin.Ins.Token;
        //    client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
        //    return client;
        //}
        public static MessageServiceClient NewMessageServiceClient()
        {
            MessageServiceClient client = new MessageServiceClient();
            client.ClientCredentials.UserName.UserName = SectionLogin.Ins.CurrentUser.UserName;
            client.ClientCredentials.UserName.Password = SectionLogin.Ins.Token;
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
            return client;
        }

        public static MessageServiceClient NewMessageServiceClient(string userName, string password)
        {
            MessageServiceClient client = new MessageServiceClient();
            client.ClientCredentials.UserName.UserName = userName;
            client.ClientCredentials.UserName.Password = password;
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
            return client;
        }
        internal static QLHS_DR.ChatAppServiceReference.Standard[] LoadStandards()
        {
            MessageServiceClient client = NewMessageServiceClient();
            QLHS_DR.ChatAppServiceReference.Standard[] ketqua;
            try
            {
                client.Open();
                ketqua = client.LoadStandards();
                client.Close();
                return ketqua;
            }
            catch (Exception ex)
            {
                client.Abort();
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        //private static WSHttpBinding BindingConfig()
        //{
        //    return new WSHttpBinding
        //    {
        //        MaxBufferPoolSize = 2147483647L,
        //        MaxReceivedMessageSize = 2147483647L,             
        //        Security =
        //        {
        //            Mode = SecurityMode.TransportWithMessageCredential,
        //            Transport = {ClientCredentialType = HttpClientCredentialType.None},
        //            Message = {ClientCredentialType = MessageCredentialType.UserName}
        //        },
        //        ReaderQuotas =
        //        {
        //            MaxDepth = int.MaxValue,
        //            MaxStringContentLength = int.MaxValue,
        //            MaxArrayLength = int.MaxValue,
        //            MaxBytesPerRead = int.MaxValue,                    
        //            MaxNameTableCharCount = int.MaxValue                  
        //        }
        //    };
        //}
    }


}
