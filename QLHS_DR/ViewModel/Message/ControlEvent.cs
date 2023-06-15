using Prism.Events;


namespace QLHS_DR.ViewModel.Message
{
    public class ReloadNewTasksTabEvent : PubSubEvent<object>
    {

    }
    public class ReloadFinishTasksTabEvent : PubSubEvent<object>
    {

    }
    public class ReloadRevokedTasksTabEvent : PubSubEvent<object>
    {

    }
    public class ReloadAllTaskTabEvent : PubSubEvent<object>
    {

    }
    public class NewTasksTabTitleChangedEvent : PubSubEvent<TitletabControlMessage>
    {

    }
    public class FinishTasksTabTitleChangedEvent : PubSubEvent<TitletabControlMessage>
    {

    }
    public class RevokedTasksTabTitleChangedEvent : PubSubEvent<TitletabControlMessage>
    {

    }
    public class AllTaskTabTitleChangedEvent : PubSubEvent<TitletabControlMessage>
    {

    }
}
