using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ASMaIoP.UserControl;

namespace ASMaIoP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void toolBarUse()
        {
            MessageBox.Show("фывфыв");
        }

        public MainWindow()
        {
            InitializeComponent();
            ContentView.Content = new WaitLogins(this); //Отправляем в форму WaitLogins ссылку на эту форму
        }

        public void WindowEnabled() //Метод активируйщий menu после авторизации
        {
            MainMenu.IsEnabled = true;
            ContentView.Content = null;
        }

        private void Menu_MyProfile_Click(object sender, RoutedEventArgs e)
        {
            ContentView.HorizontalAlignment = HorizontalAlignment.Left;
            ContentView.VerticalAlignment = VerticalAlignment.Top;
            ContentView.Content = new MyProfile();
        }
        private void Menu_CreateProfile_Click(object sender, RoutedEventArgs e)
        {
            ContentView.HorizontalAlignment = HorizontalAlignment.Left;
            ContentView.VerticalAlignment = VerticalAlignment.Top;
            ContentView.Content = new CreateProfile();
        }
        private void Menu_ListView_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Menu_CreateList_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Menu_InvetaryOpen_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Menu_AddInvetary_CLick(object sender, RoutedEventArgs e)
        {

        }
    }
}

