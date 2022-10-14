using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ASMaIoP.net;

namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для TaskList.xaml
    /// </summary>
    public partial class TaskList : System.Windows.Controls.UserControl
    {
        public TaskList()
        {
            InitializeComponent();

            Thread thrd = new Thread(LoadThreadTasks);
            thrd.Start();
        }

        public void LoadThreadTasks()
        {
            if(StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)ASMaIoP.General.Client.ProtocolId.DataTransfer_Tasks);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            int nCount = StaticApplication.Session.ReadInt();
            Action act = delegate
            {
                for (int i = 0; i < nCount; i++)
                {

                    string[] Task = StaticApplication.Session.ReadString().Split(';');
                    CreaterList.Items.Add(Task[0] + Task[1]);
                    StatusList.Items.Add(Task[3]);
                    NameList.Items.Add(Task[2]);
                    MemberList.Items.Add(Task[4]);
                }
            };
            Dispatcher.Invoke(act);
            //NameListItem
            //StatusListBox
            //CreaterListBox    
            //DiscriptionListItem
            //MemberListItem
            StaticApplication.Session.Close();
        }
    }
}
