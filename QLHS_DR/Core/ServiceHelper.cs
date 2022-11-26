using QLHS_DR.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EofficeClient.Core
{
    internal class ServiceHelper
    {
        private const string m_a = "https";

        private const string b = "192.168.11.12";

        private const int c = 8888;

        private const string d = "EEMCDRWcfService/service";

        public static EEMCDRWcfServiceClient NewServiceClient(string userName, string password)
        {

            EEMCDRWcfServiceClient eEMCDRWcfServiceClient = new EEMCDRWcfServiceClient(BindingConfig(), new EndpointAddress(string.Format("{0}://{1}:{2}/{3}", "https", "192.168.11.12", 8888, "EEMCDRWcfService/service")));
            eEMCDRWcfServiceClient.ClientCredentials.UserName.UserName = userName;
            eEMCDRWcfServiceClient.ClientCredentials.UserName.Password = password;
            return eEMCDRWcfServiceClient;
        }
        private static WSHttpBinding BindingConfig()
        {
            return new WSHttpBinding
            {
                MaxReceivedMessageSize = 2147483647L,
                MaxBufferPoolSize = 2147483647L,
                Security =
            {
                Mode = SecurityMode.TransportWithMessageCredential,
                Transport =
                {
                    ClientCredentialType = HttpClientCredentialType.None
                },
                Message =
                {
                    ClientCredentialType = MessageCredentialType.UserName
                }
            },
                ReaderQuotas =
                {
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxDepth = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue,
                    MaxStringContentLength = int.MaxValue
                }
            };
        }
    }


}
