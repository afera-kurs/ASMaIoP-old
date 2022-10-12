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
        public static WaitLogins Instance = null;

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

        public void CardRecived(string CardId)
        {
            CardId = CardId.Substring(0, CardId.Length - 1); //Убираем один символ для соотвествие данных

            if(StaticApplication.Session.Auth(CardId)) //Отправляем card ID для авторизации
            {
                if (StaticApplication.Session.AccessLevel != 3)
                {
                    Action act = delegate
                    {
                        wnd.CreateProfile.Visibility = Visibility.Collapsed;
                        wnd.CreateList.Visibility = Visibility.Collapsed;
                    };
                    wnd.Dispatcher.Invoke(act);
                }
                WindowEnabled(); 
            }
            else
            {
                MessageBox.Show("Не удалось авторизоваться");
            }
            
        }
    }
}
