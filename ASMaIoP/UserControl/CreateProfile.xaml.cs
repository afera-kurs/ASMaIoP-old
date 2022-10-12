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
using System.Threading;
using ASMaIoP.net;
using ASMaIoP.General.Client;

namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для CreateProfile.xaml
    /// </summary>
    public partial class CreateProfile : System.Windows.Controls.UserControl
    {
        public CreateProfile()
        {
            InitializeComponent();

            Thread thrd = new Thread(LoadThread);
            thrd.Start();
        }

        private void LoadThread()
        {
            if(StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)ProtocolId.DataTransfer_CreateProfile);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);

            int nCount = StaticApplication.Session.ReadInt();

            for(int i = 0; i < nCount; i++)
            {
                string NewElementText = StaticApplication.Session.ReadString();

                Action ComboBoxAdd = delegate
                {
                    cmsearch.Items.Add(NewElementText);
                };

                this.Dispatcher.Invoke(ComboBoxAdd);
            }

            StaticApplication.Session.Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }


            StaticApplication.Session.Write((int)ProtocolId.DataWriteEmpl_CreateProfile);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);
            StaticApplication.Session.Write($"{tb1.Text};{tb2.Text};{tb3.Text};{tb4.Text};{tx1.Text}");

            int nCode = StaticApplication.Session.ReadInt();
            MessageBox.Show(nCode > 0 ? "успешно" : "хреново");

            StaticApplication.Session.Close();
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            string sEmployeeID = cmsearch.SelectedItem.ToString().Split(' ')[0];
            
            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }
            StaticApplication.Session.Write((int)ProtocolId.DataTransfer_MyProfile);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);
            StaticApplication.Session.Write("");

            string ProfileInfo = StaticApplication.Session.ReadString();

            StaticApplication.Session.Close();

            ASMaIoP.General.Config cfg = new ASMaIoP.General.Config();
            cfg.ParseFromString(ProfileInfo);

            Action act = delegate
            {
                WriteChtoto(cfg["Name"], cfg["Surname"], cfg["Patronimyc"], cfg["role"]);
            };
            Dispatcher.Invoke(act);

        }

        public void WriteChtoto(string name, string surname, string patronymic, string role)
        {
            ListBoxPersonData.Items[1] = name;
            ListBoxPersonData.Items[2] = surname;
            ListBoxPersonData.Items[3] = patronymic;
            ListBoxPersonData.Items[4] = role;
        }

        bool IsReadCard = false;
        private void ButtonStartReadCard_Click(object sender, RoutedEventArgs e)
        {
            if(IsReadCard)
            {
                ButtonStartReadCard.Content = "Начать считывание карты";
                ArduinoApplicationAPI.cardReceivedHandler -= CustomCardReciver;
                ArduinoApplicationAPI.cardReceivedHandler += WaitLogins.Instance.CardRecived;
            }
            else
            {
                ArduinoApplicationAPI.cardReceivedHandler -= WaitLogins.Instance.CardRecived;
                ArduinoApplicationAPI.cardReceivedHandler += CustomCardReciver;
                ButtonStartReadCard.Content = "остановить считывание карты";
            }

            IsReadCard = !IsReadCard;
        }
        
        private void CustomCardReciver(string CardId)
        {
            CardId = CardId.Substring(0, CardId.Length - 1); //Убираем один символ для соотвествие данных
            Action tx1Add = delegate
            {
                tx1.Text = CardId;
            };

            this.Dispatcher.Invoke(tx1Add);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AddPhoto_Click(object sender, RoutedEventArgs e)
        {

        }
        private void UpdatePhoto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
