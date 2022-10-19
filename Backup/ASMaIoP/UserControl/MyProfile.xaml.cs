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
using System.IO;
using Microsoft.Win32;

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

            byte[] bytes = StaticApplication.Session.ReadBytes();

            StaticApplication.Session.Close();

            ASMaIoP.General.Config cfg = new ASMaIoP.General.Config();  
            cfg.ParseFromString(ProfileInfo);

            Action act = delegate
            {
                //Тут происходить WFP'тическая магия
                Image newImg = new Image();
                WriteChtoto(cfg["Name"], cfg["Surname"], cfg["Patronimyc"], cfg["role"]);

                MemoryStream ms = new MemoryStream(bytes);
                newImg.Source = BitmapFrame.Create(ms,
                BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                PhotoEmployee.Content = newImg;
                PhotoEmployee.UpdateLayout();
            };
            Dispatcher.Invoke(act);

        }
        public MyProfile()
        {
            Thread thrd = new Thread(LoadData);
            thrd.Start();
            InitializeComponent();
        }

        public void WriteChtoto(string name, string surname, string patronymic, string role)
        {
            ListBoxPersonData.Items[1] = name;
            ListBoxPersonData.Items[2] = surname;
            ListBoxPersonData.Items[3] = patronymic;
            ListBoxPersonData.Items[4] = role;
        }

        private void UpdatePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.JPG)| *.JPG| All files(*.*) | *.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file

                //byte[] bytes = File.ReadAllBytes(filePath);

                Stream fileStream = openFileDialog.OpenFile();
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);

                if (StaticApplication.Session.Open() != SUCCESS)
                {
                    return;
                }

                StaticApplication.Session.Write((int)ProtocolId.DataTransfer_UpdateImage_CreateProfile);
                StaticApplication.Session.Write(StaticApplication.Session.SessionId);
                StaticApplication.Session.Write(bytes);

                StaticApplication.Session.Close();

                Thread thrd = new Thread(LoadData);
                thrd.Start();
            }
        }
    }
}
