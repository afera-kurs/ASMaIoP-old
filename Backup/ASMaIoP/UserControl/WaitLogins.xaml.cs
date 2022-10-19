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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using ASMaIoP;
using ASMaIoP.net;
using System.Diagnostics;

namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для WaitLogins.xaml
    /// </summary>


    public partial class WaitLogins : System.Windows.Controls.UserControl
    {

        public class AdminInputWindow : Window
        {
            public AdminInputWindow(WaitLogins wnd)
            {
                Width = 250;
                Height = 200;
                ResizeMode = ResizeMode.NoResize;

                Grid grid = new Grid();
                Content = grid;

                RowDefinition rowDefinition = new RowDefinition();
                RowDefinition rowDefinition2 = new RowDefinition();
                RowDefinition rowDefinition3 = new RowDefinition();

                grid.RowDefinitions.Add(rowDefinition);
                grid.RowDefinitions.Add(rowDefinition2);
                grid.RowDefinitions.Add(rowDefinition3);

                TextBlock text = new TextBlock();
                text.Text = "Режим администратора!";
                text.FontSize = 20.0f;
                text.TextAlignment = TextAlignment.Center;
                Grid.SetRow(text, 0);

                PasswordBox textBox = new PasswordBox();
                textBox.Width = 170;
                textBox.Height = 22;
                textBox.IsEnabled = true;
                Button button1 = new Button();
                button1.Content = "Подвердить";
                textBox.PasswordChar = '*';
                button1.Height = 25;
                button1.Width = 75;
                button1.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                button1.Click += new RoutedEventHandler(button1_Click);
                Grid.SetRow(button1, 2);
                Grid.SetRow(textBox, 1);

                grid.Children.Add(text);
                grid.Children.Add(button1);
                grid.Children.Add(textBox);

                void button1_Click(object sender, RoutedEventArgs e)
                {
                    if (textBox.Password.Length > 0)
                    {
                        wnd.CardRecived(textBox.Password + "\r");
                        this.Close();
                    }
                }
            }
        }

        public static WaitLogins Instance = null;

        MainWindow wnd;//Объявляем переменную которая хранит MainWindow
        Button adminLogin = new Button();

        AdminInputWindow input = null;
        public WaitLogins(MainWindow wnd)// Принимаем объект класса MainWindow
        {
            InitializeComponent();

            this.wnd = wnd; //Присваем нашей переменной значение MainWindow 

            wnd.ContentView.HorizontalAlignment = HorizontalAlignment.Center; //Делаем оринтацию вертекали по центру
            wnd.ContentView.VerticalAlignment = VerticalAlignment.Center; //Делаем оринтацию горизантали по центру
            General.Client.ArduinoApplicationAPI.cardReceivedHandler += CardRecived; //Присваймаем делигату метод для считваение кард
            
            if(StaticApplication.IsEnabledAdminInput)
            {
                input = new AdminInputWindow(this);
                input.Show();
            }
            //input.Owner = wnd;
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

            if(StaticApplication.Session.Auth(General.Crypto.sha256(CardId))) //Отправляем card ID для авторизации
            {
                if (StaticApplication.Session.AccessLevel != 3 && StaticApplication.Session.AccessLevel != 999)
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
