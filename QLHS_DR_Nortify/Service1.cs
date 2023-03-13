using QLHS_DR.EOfficeServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Core;
using ToastNotifications.Messages;

namespace QLHS_DR_Nortify
{
    public partial class Service1 : ServiceBase
    {
        Notifier notifierForNormalUser;
        MessageOptions optionsForNormalUser;
        public Service1()
        {
            InitializeComponent();
            
            notifierForNormalUser = new Notifier(cfg =>
            {
                cfg.PositionProvider = new PrimaryScreenPositionProvider(corner: Corner.BottomRight, offsetX: 10, offsetY: 10);
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(notificationLifetime: TimeSpan.FromSeconds(3), maximumNotificationCount: MaximumNotificationCount.FromCount(5));
                cfg.DisplayOptions.TopMost = true; // set the option to show notifications over other windows
                cfg.DisplayOptions.Width = 200; // set the notifications width
               // cfg.Dispatcher = System.Windows.Application.Current.Dispatcher;
            });
            optionsForNormalUser = new MessageOptions
            {
                FontSize = 18, // set notification font size
                ShowCloseButton = true, // set the option to show or hide notification close button
                Tag = "Any object or value which might matter in callbacks",
                FreezeOnMouseEnter = true, // set the option to prevent notification dissapear automatically if user move cursor on it
                NotificationClickAction = n =>
                {
                    n.Close();
                    //window.Show();
                    //window.WindowState = System.Windows.WindowState.Normal;

                    //TabContainer item = Workspaces.Where(x => x.Header == "Yêu cầu chuyển tài liệu").FirstOrDefault();
                    //if (item != null)
                    //{
                    //    item.IsSelected = true;
                    //    item.IsVisible = true;
                    //}
                    //else
                    //{
                    //    ListRequestSendDocUC listRequestSendDocUC = new ListRequestSendDocUC();
                    //    TabContainer tabItemListSendDoc = new TabContainer()
                    //    {
                    //        Header = "Yêu cầu chuyển tài liệu",
                    //        IsSelected = true,
                    //        IsVisible = true,
                    //        Content = listRequestSendDocUC
                    //    };
                    //    Workspaces.Add(tabItemListSendDoc);
                    //}
                }
            };
        }
       
        string _ConnectionString = @"data source=192.168.12.10;initial catalog=EEMCDR;persist security info=true;user id=duongbkt;password=K4q1e0o@";
        private SqlTableDependency<UserTask> _TbRequestSendDocumentDependency;
        protected override void OnStart(string[] args)
        {
            _TbRequestSendDocumentDependency = new SqlTableDependency<UserTask>(_ConnectionString, tableName: "UserTask", schemaName: "dbo");
            _TbRequestSendDocumentDependency.OnChanged += RequestSendDocument_OnChanged;
            //_TbRequestSendDocumentDependency.OnError += RequestSendDocument_OnError;
            //_TbRequestSendDocumentDependency.Stop();                            
            _TbRequestSendDocumentDependency.Start();
        }

        private void RequestSendDocument_OnChanged(object sender, RecordChangedEventArgs<UserTask> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                
                var entry = e.Entity;
                if ((e.ChangeType == TableDependency.SqlClient.Base.Enums.ChangeType.Insert) )
                {
                    string message = "Có tài liệu vừa được gửi tới bạn: " + entry.Task.Subject;
                    
                    //System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    //{
                    notifierForNormalUser.ShowInformation(message, optionsForNormalUser);
                    //}));
                }
                if ((e.ChangeType == TableDependency.SqlClient.Base.Enums.ChangeType.Update) )
                {
                    //if (entry.IsDone == true)
                    //{
                    //    string message = "Tài liệu: " + entry.TransformerCode + " \\ " + entry.DocumentName + "đã được gửi đi";
                    //    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    //    {
                    //        notifierForNormalUser.ShowInformation(message, optionsForSender);
                    //    }));
                    //}
                }
            }
        }

        protected override void OnStop()
        {
        }
    }
}
