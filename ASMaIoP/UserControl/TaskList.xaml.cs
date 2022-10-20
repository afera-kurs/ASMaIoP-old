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
using System.Collections;
using ASMaIoP.net;
using System.Threading.Tasks;

namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для TaskList.xaml
    /// </summary>
    public partial class TaskList : System.Windows.Controls.UserControl
    {
        int? nSelected = null;
        public TaskList()
        {
            InitializeComponent();

            Thread thrd = new Thread(LoadThreadTasks);
            thrd.Start();
        }

        Hashtable hStates = new Hashtable();


        public void LoadThreadTasks()
        {
            if(StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)ASMaIoP.General.Client.ProtocolId.DataTransfer_Tasks);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            Action act = delegate
            {
                if (StaticApplication.Session.AccessLevel == 3 || StaticApplication.Session.AccessLevel == 999)
                {
                    tbCreater.IsEnabled = true; 
                    tbDescrption.IsEnabled = true; 
                    tbStatus.Visibility = Visibility.Collapsed;
                    cmbStatus.Visibility = Visibility.Visible;
                    tbMembers.Visibility = Visibility.Collapsed;
                    cmbMember.Visibility = Visibility.Visible;
                    tb5.IsEnabled = true;
                    ButtonDeleteMember.Visibility = Visibility.Visible;

                    ButtonCreate.Visibility = Visibility.Visible;
                    ButtonDelete.Visibility = Visibility.Visible;
                    ButtonUpdate.Visibility = Visibility.Visible;
                }

                string RowStatePacket = StaticApplication.Session.ReadString();
                string[] RowStates = RowStatePacket.Split(';');
                foreach (string state in RowStates)
                {
                    string[] data = state.Split('.');
                    int Id = int.Parse(data[0]);
                    string StateTitle = data[1];
                    cmbStatus.Items.Add(new KeyValuePair<int, string>(Id, StateTitle));
                    hStates.Add(Id, new KeyValuePair<int, string>(cmbStatus.Items.Count - 1, StateTitle));
                }

                int nCount = StaticApplication.Session.ReadInt();
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
            ButtonJoin.IsEnabled = true;
            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)ASMaIoP.General.Client.ProtocolId.GetTaskInfo);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            string TaskId = button.Content.ToString();
            nSelected = int.Parse(TaskId);
            StaticApplication.Session.Write(TaskId);

            string[] Task = StaticApplication.Session.ReadString().Split(';');

            tbCreater.Text = $"{Task[0]} {Task[1]}";
            tbDescrption.Text = $"{Task[2]}";
            //cmbStatus.Text=Task[3];

            int SelectedInx = 0;

            foreach(DictionaryEntry de in hStates)
            {
                KeyValuePair<int, string> ValuePair = (KeyValuePair<int, string>)de.Value;
                if (ValuePair.Value == Task[3])
                {
                    SelectedInx = ValuePair.Key;
                }
            }
            cmbStatus.SelectedIndex = SelectedInx;
            //cmbStatus.Items.Add(Task[3]);
            tbStatus.Text = Task[3];

            //foreach(ItemCollection item in cmbStatus.ItemsSource)
            cmbMember.Items.Clear();
            for (int i = 4; i < Task.Length; i++)
            {
                string executant = Task[i];;
                i++;
                cmbMember.Items.Add(new KeyValuePair<int, string>(int.Parse(Task[i]), executant));
            }
            StaticApplication.Session.Close();
        }

        private void ButtonJoin_Click(object sender, RoutedEventArgs e)
        {
            if (nSelected == null) return;
            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.TaskExecutantJoin);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            StaticApplication.Session.Write($"{nSelected.ToString()}");


            StaticApplication.Session.Close();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            if (nSelected != null) return;

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.DataWrite_Tasks);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);
            
            if(tbDescrption.Text == null)
                return;

            StaticApplication.Session.Write($"{tbDescrption.Text}");

            CheckStatus(StaticApplication.Session.ReadInt());

            StaticApplication.Session.Close();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (nSelected == null) return;

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }
            
            StaticApplication.Session.Write((int)General.Client.ProtocolId.DataDelete_Tasks);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            StaticApplication.Session.Write($"{nSelected.ToString()}");

            CheckStatus(StaticApplication.Session.ReadInt());

            StaticApplication.Session.Close();
            nSelected = null;
        }

        void CheckStatus(int Status)
        {
            if (Status == 1)
            {
                MessageBox.Show("Успешно");
            }
            else
            {
                MessageBox.Show("Ошибочка вышла");
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (nSelected == null) return;

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.DataSave_Tasks);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            int StateId = ((KeyValuePair<int,string>)cmbStatus.SelectedItem).Key;

            StaticApplication.Session.Write($"{tbDescrption.Text};{StateId};{nSelected.ToString()}");

            CheckStatus(StaticApplication.Session.ReadInt());

            StaticApplication.Session.Close();
        }

        private void ButtonDeleteMember_Click(object sender, RoutedEventArgs e)
        {
            if (nSelected == null) return;

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.TaskDeleteMembers);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            int EmployeeID = ((KeyValuePair<int, string>)cmbMember.SelectedItem).Key;
            StaticApplication.Session.Write($"{EmployeeID}");

            CheckStatus(StaticApplication.Session.ReadInt());

            StaticApplication.Session.Close();
            nSelected = null;
        }
    }
}
