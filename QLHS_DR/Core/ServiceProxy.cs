
using System.ServiceModel;
using QLHS_DR.ServiceReference1;

namespace EofficeClient.Core
{
    public sealed class ServiceProxy
    {
        private static EEMCDRWcfServiceClient _ins;
        public static EEMCDRWcfServiceClient Ins
        {
            get
            {
                if (_ins == null) _ins = new EEMCDRWcfServiceClient();
                if (_ins.State != CommunicationState.Opened)
                {
                    _ins.Open();
                }
                return _ins;
            }
            set
            {
                _ins = value;
            }
        }
    }
}
