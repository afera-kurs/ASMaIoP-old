using System;
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
using ASMaIoP.General.Client;
using ASMaIoP.net;
using System.Threading;


namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для Inventory.xaml
    /// </summary>
    
    public partial class Inventory : System.Windows.Controls.UserControl
    {
        public Inventory()
        {
            
            Thread thrd = new Thread(LoadDataAndViewInvetory);
            thrd.SetApartmentState(ApartmentState.STA);
            thrd.Start();

            InitializeComponent();
        }
        public void LoadDataAndViewInvetory()
        {
            TreeViewItem Item = new TreeViewItem();
            //Item.Header = ;
            //Item.Items.Add();


            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
                return;
            StaticApplication.Session.Write((int)ProtocolId.DataTransfer_Inventory);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            int column = StaticApplication.Session.ReadInt();
            
            for(int i = 0; i < column; i++)
            {
                string[] Data = StaticApplication.Session.ReadString().Split(';');

                Action Act = delegate
                {
                    Item = new TreeViewItem();
                    Item.Header = Data[0];
                    Item.Items.Add(Data[1]);
                    YourItems.Items.Add(Item);
                };

                Dispatcher.Invoke(Act);
            }

       

            StaticApplication.Session.Close();
        }
    }
}
