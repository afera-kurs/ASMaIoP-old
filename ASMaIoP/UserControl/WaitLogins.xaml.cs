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
using ASMaIoP;
using ASMaIoP.net;

namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для WaitLogins.xaml
    /// </summary>
    public partial class WaitLogins : System.Windows.Controls.UserControl
    {
        MainWindow wnd;//Объявляем переменную которая хранит MainWindow

        public WaitLogins(MainWindow wnd)// Принимаем объект класса MainWindow
        {
            InitializeComponent();
            this.wnd = wnd; //Присваем нашей переменной значение MainWindow 
            wnd.ContentView.HorizontalAlignment = HorizontalAlignment.Center; //Делаем оринтацию вертекали по центру
            wnd.ContentView.VerticalAlignment = VerticalAlignment.Center; //Делаем оринтацию горизантали по центру
            General.Client.ArduinoApplicationAPI.cardReceivedHandler += CardRecived; //Присваймаем делигату метод для считваение кард
        }
        public void WindowEnabled() //Метод активируйщий menu после авторизации
        {
            //Присваем переменной safeWrite анониманый метод  
            Action safeWrite = delegate {
                wnd.MainMenu.IsEnabled = true;
                wnd.ContentView.Content = null;
            };
            // Отправляем этот анонимный метод в поток окна чтобы избежать ошибки безопасности
            wnd.Dispatcher.Invoke(safeWrite); 
        }

        private void CardRecived(string CardId)
        {  
            if (StaticApplication.Session.Open() != General.ErrorCode.SUCCESS) //Провереям смогли мы подключиться
            {
                return;
            }

            StaticApplication.Session.Write((int)General.Client.ProtocolId.Auth); //Отправляем серверу протокол авторизации
            StaticApplication.Session.Write(CardId); //Отправляем в сервер ID приложенной карты

            int Code = StaticApplication.Session.ReadInt(); //Считваем код от сервера 
            if(Code == 1) WindowEnabled();
            if (Code == 0) MessageBox.Show("Не получилось произвести авторизацию");
            StaticApplication.Session.Close();
        }
    }
}
