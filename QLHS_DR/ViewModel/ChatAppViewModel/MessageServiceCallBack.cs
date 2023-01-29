using QLHS_DR.ChatAppServiceReference;

namespace QLHS_DR.ViewModel.ChatAppViewModel
{
    public class MessageServiceCallBack : IMessageServiceCallback
    {
        public void ForwardToClient(Message message)
        {
            throw new System.NotImplementedException();
        }

        public void UserConnected(User[] users)
        {
            throw new System.NotImplementedException();
        }
    }
}
