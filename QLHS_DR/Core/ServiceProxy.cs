
using System.ServiceModel;
using QLHS_DR.EOfficeServiceReference;

namespace EofficeClient.Core
{
    public sealed class ServiceProxy
    {
        private static EofficeMainServiceClient _ins;
        public static EofficeMainServiceClient Ins
        {
            get
            {
                if (_ins == null) _ins = new EofficeMainServiceClient();
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
        ~ServiceProxy()
        {           
            if (_ins != null && _ins.State == System.ServiceModel.CommunicationState.Opened)
            {
                _ins.Close();
            }
        }
    }
}
