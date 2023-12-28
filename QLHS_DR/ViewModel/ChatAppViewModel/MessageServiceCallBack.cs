using QLHS_DR.EofficeMainServiceReference;
using System;
using System.ServiceModel;
using ToastNotifications.Core;
using ToastNotifications;
using System.Windows;

namespace QLHS_DR.ViewModel.ChatAppViewModel
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]

    internal class MessageServiceCallBack : IEofficeMainServiceCallback
    {
        private static Action<RequestSendDocument> _NortifyRequestSendDocument = null;
        public static void SetDelegateNortifyRequestSendDocument(Action<RequestSendDocument> nAction)
        {
            _NortifyRequestSendDocument = nAction;
        }

        public void ForwardToClient(EofficeMainServiceReference.Message message)
        {
            throw new NotImplementedException();
        }

        public void NortifyRequestSendDocument(RequestSendDocument requestSendDocument)
        {
            _NortifyRequestSendDocument(requestSendDocument);
        }

        public void UserConnected(User[] users)
        {
            throw new NotImplementedException();
        }

        public void UserTaskChanged()
        {
            throw new NotImplementedException();
        }
    }
}
