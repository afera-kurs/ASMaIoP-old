using System;
using System.IO;
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
            Action ComboBoxClear = delegate
            {
                cmsearch.Items.Clear();
                cm4.Items.Clear();
                tb11.Content = "Имя: ";
                tb22.Content = "Фамилия: ";
                tb33.Content = "Отчество: ";
                tb44.Content = "Должность: ";
                tb1.Text = "";
                tb2.Text = "";
                tb3.Text = "";
            };

            this.Dispatcher.Invoke(ComboBoxClear);

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
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

            nCount = StaticApplication.Session.ReadInt();

            for (int i = 0; i < nCount; i++)
            {
                string NewElementText = StaticApplication.Session.ReadString();

                Action ComboBoxAdd = delegate
                {
                    cm4.Items.Add(NewElementText);
                };

                this.Dispatcher.Invoke(ComboBoxAdd);
            }

            StaticApplication.Session.Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (cm4.SelectedItem == null) return;
            string sRoleID = cm4.SelectedItem.ToString().Split(' ')[0];
            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }


            StaticApplication.Session.Write((int)ProtocolId.DataWriteEmpl_CreateProfile);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);
            StaticApplication.Session.Write($"{tb1.Text};{tb2.Text};{tb3.Text};{sRoleID};{tx1.Text}");

            int nCode = StaticApplication.Session.ReadInt();
            MessageBox.Show(nCode > 0 ? "успешно" : "хреново");

            StaticApplication.Session.Close();
            Thread thrd = new Thread(LoadThread);
            thrd.Start();
        }
        private void ButtonChangeSave_Click(object sender, RoutedEventArgs e)
        {
            if (cm4.SelectedItem == null) return;
            string sRoleID = cm4.SelectedItem.ToString().Split(' ')[0];
            if (cmsearch.SelectedItem == null) return;
            string sEmployeeID = cmsearch.SelectedItem.ToString().Split(' ')[0];

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }

            StaticApplication.Session.Write((int)ProtocolId.DataUpdateEmpl_CreateProfile);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);
            StaticApplication.Session.Write($"{sEmployeeID};{sRoleID};{tb1.Text};{tb2.Text};{tb3.Text}");

            int nCode = StaticApplication.Session.ReadInt();
            MessageBox.Show(nCode > 0 ? "успешно" : "хреново");

            StaticApplication.Session.Close();
            Thread thrd = new Thread(LoadThread);
            thrd.Start();
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            string sEmployeeID = cmsearch.SelectedItem.ToString().Split(' ')[0];
            
            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }
            StaticApplication.Session.Write((int)ProtocolId.DataSearchEmpl_CreateProfile);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);
            StaticApplication.Session.Write(sEmployeeID);

            string ProfileInfo = StaticApplication.Session.ReadString();
            byte[] bytes = StaticApplication.Session.ReadBytes();

            StaticApplication.Session.Close();

            ASMaIoP.General.Config cfg = new ASMaIoP.General.Config();
            cfg.ParseFromString(ProfileInfo);

            Action act = delegate
            {
                WriteNeChtoto(cfg["Name"], cfg["Surname"], cfg["Patronymic"], cfg["role"]);
                Image newImg = new Image();

                MemoryStream ms = new MemoryStream(bytes);
                newImg.Source = BitmapFrame.Create(ms,
                BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                PhotoEmployee.Content = newImg;
                PhotoEmployee.UpdateLayout();
            };
            Dispatcher.Invoke(act);


            sTargetId = sEmployeeID;

        }

        string sTargetName;
        string sTargetSurName;
        string sTargetPatronymic;
        string sTargetId;

        public void WriteNeChtoto(string name, string surname, string patronymic, string role)
        {
            sTargetName = name;
            sTargetSurName = surname;
            sTargetPatronymic = patronymic;

            tb11.Content = "Имя: " + name;
            tb22.Content = "Фамилия: " + surname;
            tb33.Content = "Отчество: " + patronymic;
            tb44.Content = "Должность: " + role;
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
                tx1.Text = General.Crypto.sha256(CardId);
            };

            this.Dispatcher.Invoke(tx1Add);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            string sEmployeeID = cmsearch.SelectedItem.ToString().Split(' ')[0];

            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS)
            {
                return;
            }
            StaticApplication.Session.Write((int)ProtocolId.DataDeleteEmpl_CreateProfile);
            StaticApplication.Session.Write(StaticApplication.Session.SessionId);
            StaticApplication.Session.Write(sEmployeeID);

            int nCode = StaticApplication.Session.ReadInt();
            MessageBox.Show(nCode > 0 ? "успешно" : "ай больно в ноге");

            StaticApplication.Session.Close();

            ASMaIoP.General.Config cfg = new ASMaIoP.General.Config();

            Thread thrd = new Thread(LoadThread);
            thrd.Start();
        }
    }
}
