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

namespace ASMaIoP.UserControl
{
    /// <summary>
    /// Логика взаимодействия для WaitLogins.xaml
    /// </summary>
    public partial class WaitLogins : System.Windows.Controls.UserControl
    {
        MainWindow wnd;

        public WaitLogins(MainWindow wnd)// Принимаем
        {
            InitializeComponent();
            this.wnd = wnd;
            wnd.ContentView.HorizontalAlignment = HorizontalAlignment.Center;
            wnd.ContentView.VerticalAlignment = VerticalAlignment.Center;
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            wnd.WindowEnabled();
        }
    }
}
