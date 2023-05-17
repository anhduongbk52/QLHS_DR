//using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.ChatAppServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace EofficeClient.Core
{
    internal class ServiceHelper
    {
        private const string protocol = "https";

        private const string ipAddress = "192.168.11.12";

        private const int port = 8888;

        private const string servicePath = "EEMCDRWcfService/service";

        //public static EofficeMainServiceClient NewEofficeMainServiceClient(string userName, string password)
        //{
        //    EofficeMainServiceClient client = new EofficeMainServiceClient();
        //    //EofficeMainServiceClient client = new EofficeMainServiceClient(BindingConfig(), new EndpointAddress(string.Format("{0}://{1}:{2}/{3}", protocol, ipAddress, port, "EofficeService/Service")));
        //    client.ClientCredentials.UserName.UserName = userName;
        //    client.ClientCredentials.UserName.Password = password;
        //    return client;
        //}

        public static MessageServiceClient NewMessageServiceClient(string userName, string password)
        {
            MessageServiceClient client = new MessageServiceClient();
            client.ClientCredentials.UserName.UserName = userName;
            client.ClientCredentials.UserName.Password = password;
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
            return client;
        }
        //public static ChannelFactory<IMessageService> NewChannelFactory1()
        //{
        //    ChannelFactory<IMessageService> ret = new ChannelFactory<IMessageService>("NetTcpBinding_IMessageService");
        //    ret.Credentials.UserName.UserName = SectionLogin.Ins.CurrentUser.UserName;
        //    ret.Credentials.UserName.Password = SectionLogin.Ins.Token;
           //ret.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
        //    return ret;
        //}

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
