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
using System.Threading.Tasks;

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
                    string RowPacket = StaticApplication.Session.ReadString();
                    string[] Task = RowPacket.Split(';');
                    CreaterList.Items.Add(Task[0] + Task[1]);
                    string TaskDescription = Task[2].Split('.')[0];
                    DescriptionList.Items.Add(TaskDescription);
                    StatusList.Items.Add(Task[3]);
                    MemberList.Items.Add(Task[4]);
                    Button button = new Button();
                    button.Content = Task[5];
                    button.Click += OnClick;
                    OpenList.Items.Add(button);
                }
                StaticApplication.Session.Close();
            };
            Dispatcher.Invoke(act);

        }

        void OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MessageBox.Show(button.Content.ToString());

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)ASMaIoP.General.Client.ProtocolId.GetTaskInfo);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            string TaskId = button.Content.ToString();
            StaticApplication.Session.Write(TaskId);

            string[] Task = StaticApplication.Session.ReadString().Split(';');

            tb1.Text = $"{Task[0]} {Task[1]}";
            tb2.Text = Task[2];
            tb3.Text = Task[3];

            for(int i = 4; i < Task.Length; i++)
            {
                tb4.Text += $"{Task[i]}\n";
            }

            StaticApplication.Session.Close();
        }
    }
}
