using QLHS_DR.EOfficeServiceReference;
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
        private const string protocol = "https";

        private const string ipAddress = "192.168.11.12";

        private const int port = 8888;

        private const string servicePath = "EEMCDRWcfService/service";

        public static EEMCDRWcfServiceClient NewServiceClient(string userName, string password)
        {

            EEMCDRWcfServiceClient eEMCDRWcfServiceClient = new EEMCDRWcfServiceClient(BindingConfig(), new EndpointAddress(string.Format("{0}://{1}:{2}/{3}", protocol, ipAddress, port, "EEMCDRWcfService/service")));
            eEMCDRWcfServiceClient.ClientCredentials.UserName.UserName = userName;
            eEMCDRWcfServiceClient.ClientCredentials.UserName.Password = password;
            return eEMCDRWcfServiceClient;
        }

        public static EofficeMainServiceClient NewEofficeMainServiceClient(string userName, string password)
        {

            EofficeMainServiceClient client = new EofficeMainServiceClient(BindingConfig(), new EndpointAddress(string.Format("{0}://{1}:{2}/{3}", protocol, ipAddress, port, "EofficeService/Service")));
            client.ClientCredentials.UserName.UserName = userName;
            client.ClientCredentials.UserName.Password = password;
            return client;
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
