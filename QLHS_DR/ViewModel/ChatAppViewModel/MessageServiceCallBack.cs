namespace QLHS_DR.ViewModel.ChatAppViewModel
{
    //public class MessageServiceCallBack : IMessageServiceCallback
    //{
    //    private static Action<string> _changeText = null;
    //    public static void SetDelegate(Action<string> nAction)
    //    {
    //        _changeText = nAction;
    //    }
    //    public void ForwardToClient(ChatAppServiceReference.Message message)
    //    {
    //        throw new System.NotImplementedException();
    //    }


    //    public void UserConnected(User[] users)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public void UserTaskChanged()
    //    {            
    //        var uri = "net.tcp://192.168.11.12:8080/EofficeService/Service";
    //        var callBack = new InstanceContext(new MessageServiceCallBack());
    //        var binding = new NetTcpBinding(SecurityMode.None);
    //        var channel = new DuplexChannelFactory<IMessageService>(callBack, binding);
    //        var endPoint = new EndpointAddress(uri);
    //        var proxy = channel.CreateChannel(endPoint);
    //        proxy?.Connect(SectionLogin.Ins.CurrentUser.Id);
    //    }
    //}
}
