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
using ASMaIoP.General;
using ASMaIoP.General.Client;
using ASMaIoP.net;
using static ASMaIoP.General.ErrorCode;
using System.Threading;

namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для MyProfile.xaml
    /// </summary>
    public partial class MyProfile : System.Windows.Controls.UserControl
    {
        void LoadData()
        {
            if (StaticApplication.Session.Open() != SUCCESS)
            {
                return;
            }
            StaticApplication.Session.Write((int)ProtocolId.DataTransfer_MyProfile);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);
            string ProfileInfo = StaticApplication.Session.ReadString();

            StaticApplication.Session.Close();

            ASMaIoP.General.Config cfg = new ASMaIoP.General.Config();  
            cfg.ParseFromString(ProfileInfo);

            Action act = delegate
            {
                WriteChtoto(cfg["Name"], cfg["Surname"], cfg["Patronimyc"], cfg["role"], cfg["lvl"], cfg["ID"]);
            };
            Dispatcher.Invoke(act);
        }
        public MyProfile()
        {
            Thread thrd = new Thread(LoadData);
            thrd.Start();
            InitializeComponent();
        }

        public void WriteChtoto(string name, string surname, string patronymic, string role, string lvl, string ID)
        {
            ListBoxPersonData.Items[1] = name;
            ListBoxPersonData.Items[2] = surname;
            ListBoxPersonData.Items[3] = patronymic;
            ListBoxPersonData.Items[4] = role;
            ListBoxPersonData.Items[5] = lvl;
            ListBoxPersonData.Items[6] = ID;
        }

        private void UpdatePhoto_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
