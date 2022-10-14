using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ASMaIoP.net;
using ASMaIoP.General.Client;


namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для AddItemsInventory.xaml
    /// </summary>
    public partial class AddItemsInventory : System.Windows.Controls.UserControl
    {
        public AddItemsInventory()
        {   
            InitializeComponent();

            Thread thrd = new Thread(LoadThread);
            thrd.Start();
        }

        void LoadThread()
        {
            int nCount = 0;
            int nSelected = -1;

            Action act = delegate
            {
                nCount = SelectEmployeeComboBX.Items.Count;
                if(nCount > 0)
                {
                    nSelected = SelectEmployeeComboBX.SelectedIndex;
                }
                SelectEmployeeComboBX.Items.Clear();
                ItemIDTextBLC.Text = null;
                YourItems.Items.Clear();
            };
            Dispatcher.Invoke(act);

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.DataTransfer_AppItems);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            nCount = StaticApplication.Session.ReadInt();
            for(int i = 0; i < nCount; i++)
            {
                string Employee = StaticApplication.Session.ReadString();
                act = delegate
                {
                    SelectEmployeeComboBX.Items.Add(Employee);
                };
                Dispatcher.Invoke(act);
            }

            StaticApplication.Session.Close();

            if(nSelected > -1)
            {
                act = delegate
                {
                    SelectEmployeeComboBX.SelectedIndex = nSelected;
                };
                Dispatcher.Invoke(act);
            }

        }

        void LoadEmployeeData()
        {
            Action act = delegate
            {
                YourItems.Items.Clear();
            };
            Dispatcher.Invoke(act);

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.DataLoadFromEmployeeID_AppItems);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            string SendData = "0";

            act = delegate
            {
                SendData = SelectEmployeeComboBX.SelectedItem.ToString().Split(' ')[0];
            };
            Dispatcher.Invoke(act);

            StaticApplication.Session.Write(SendData);

            int nCount = StaticApplication.Session.ReadInt();
            for (int i = 0; i < nCount; i++)
            {
                string[] Data = StaticApplication.Session.ReadString().Split(';');
                act = delegate
                {
                    TreeViewItem Item = new TreeViewItem();
                    Item.MouseDoubleClick += TreeViewItem_DoubleClick;
                    Item.Header = CreateTextBox(Data[0]);
                    //string sad = Data[1].Replace(":", "");
                    //string asd;
                    Item.Items.Add(CreateTextBox($"Название:{Data[1]}"));
                    Item.Items.Add(CreateTextBox($"Описание:{Data[2]}"));
                    YourItems.Items.Add(Item);
                };
                Dispatcher.Invoke(act);
            }

            StaticApplication.Session.Close();

            TextBox CreateTextBox(string text)
            {
                TextBox tbx = new TextBox();
                tbx.Text = text;
                tbx.BorderBrush = new SolidColorBrush(Colors.Gray);
                tbx.BorderThickness = new Thickness(3);
                tbx.FontSize = 17;

                return tbx;
            }
        }

        string GetData(string str)
        {
            int i = 0;
            bool Write = false;
            char[] newString = new char[str.Length];
            foreach (char ch in str)
            {
                if (ch == ':')
                {
                    Write = true;
                    continue;
                }

                if (Write)
                {
                    newString[i] = ch;
                    i++;
                }
            }

            return new string(newString);
        }

        private void TreeViewItem_DoubleClick(Object sender, MouseEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            TextBox box = (TextBox)item.Header;
            TextBox box2 = (TextBox)item.Items[0];
            TextBox box3 = (TextBox)item.Items[1];

            ItemIDTextBLC.Text = box.Text;
            string some_data = box2.Text;
            txb1.Text = GetData(some_data);

            txb2.Text = GetData(box3.Text);
        }

        private void SelectEmployeeComboBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectEmployeeComboBX.SelectedItem == null) return;
            Thread thrd = new Thread(LoadEmployeeData);
            thrd.Start();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.DataUpdate_AppItems);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            StaticApplication.Session.Write($"{ItemIDTextBLC.Text};{txb1.Text};{txb2.Text}");

            StaticApplication.Session.Close();
            Thread thrd = new Thread(LoadThread);
            thrd.Start();
        }

        private void Creste_Click(object sender, RoutedEventArgs e)
        {
            if (SelectEmployeeComboBX.SelectedItem == null)
                return;

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.DataWrite_AppItems);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);
            
            StaticApplication.Session.Write($"{SelectEmployeeComboBX.SelectedItem.ToString().Split(' ')[0]};{txb1.Text};{txb2.Text}");

            StaticApplication.Session.Close();
            Thread thrd = new Thread(LoadThread);
            thrd.Start();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.DataDelete_AppItems);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            StaticApplication.Session.Write(ItemIDTextBLC.Text);

            StaticApplication.Session.Close();
            Thread thrd = new Thread(LoadThread);
            thrd.Start();
        }
    }
}
