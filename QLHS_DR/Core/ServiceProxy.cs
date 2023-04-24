
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using QLHS_DR.Core;
using QLHS_DR.EOfficeServiceReference;
using static DevExpress.Data.Filtering.Helpers.SubExprHelper.ThreadHoppingFiltering;

namespace EofficeClient.Core
{
    public sealed class ServiceProxy
    {
        private static readonly Lazy<ServiceProxy> _Instance= new Lazy<ServiceProxy>(() => new ServiceProxy());
        private readonly ChannelFactory<IEofficeMainService> _ChannelFactory;
        private IEofficeMainService _Proxy;       
        private ServiceProxy()
        {
            _ChannelFactory = new ChannelFactory<IEofficeMainService>("WSHttpBinding_IEofficeMainService");
            _ChannelFactory.Credentials.UserName.UserName = SectionLogin.Ins.CurrentUser.UserName;
            _ChannelFactory.Credentials.UserName.Password = SectionLogin.Ins.Token;
        }
        public static ServiceProxy Instance => _Instance.Value;
        public IEofficeMainService Proxy
        {
            get
            {
                if (_Proxy == null)
                {
                    _Proxy = _ChannelFactory.CreateChannel();
                    ((IClientChannel)_Proxy).Open();
                }
                return _Proxy;
            }
        }
        public void CloseProxy()
        {
            if (_Proxy != null)
            {
                ((IClientChannel)_Proxy).Close();
                _Proxy = null;
            }
        }
        public void RenewProxy()
        {
            if (_Proxy != null)
            {
                _Proxy = _ChannelFactory.CreateChannel();
                ((IClientChannel)_Proxy).Open();
            }
            else
            {
                _Proxy = _ChannelFactory.CreateChannel();
                ((IClientChannel)_Proxy).Open();
            }
        }
    }
}
