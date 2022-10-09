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
using ASMaIoP.net;

namespace ASMaIoP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public void AlignmentTopLeft()
        {
            ContentView.HorizontalAlignment = HorizontalAlignment.Left;
            ContentView.VerticalAlignment = VerticalAlignment.Top;
        }

        public MainWindow()
        {

            if (!StaticApplication.ApplicationStart())// Вызываем метод для ожидания авторизации
            {

                System.Windows.Application.Current.Shutdown();
            }

            InitializeComponent();
            ContentView.Content = new WaitLogins(this); //Отправляем в форму WaitLogins ссылку на эту форму
        }

        private void Menu_MyProfile_Click(object sender, RoutedEventArgs e)
        {
            AlignmentTopLeft();
            ContentView.Content = new MyProfile();
        }
        private void Menu_CreateProfile_Click(object sender, RoutedEventArgs e)
        {
            AlignmentTopLeft();
            ContentView.Content = new CreateProfile();
        }
        private void Menu_ListView_Click(object sender, RoutedEventArgs e)
        {
            AlignmentTopLeft();
            ContentView.Content = new TaskList();
        }
        private void Menu_CreateList_Click(object sender, RoutedEventArgs e)
        {
            AlignmentTopLeft();
            Content = new CreateTask();
        }
        private void Menu_InvetaryOpen_Click(object sender, RoutedEventArgs e)
        {
            AlignmentTopLeft();
            Content = new Inventory();
        }
        private void Menu_AddInvetary_CLick(object sender, RoutedEventArgs e)
        {
            AlignmentTopLeft();
            Content = new AddItemsInventory();
        }
    }
}

