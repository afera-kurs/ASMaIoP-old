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
    /// Логика взаимодействия для MyProfile.xaml
    /// </summary>
    public partial class MyProfile : System.Windows.Controls.UserControl
    {
        public MyProfile()
        {
            InitializeComponent();
            ASMaIoP.General.Config cfg = new ASMaIoP.General.Config();
            WriteChtoto(/* cfg[initials], cfg[ID], cfg[lvl] */);
        }
        public void WriteChtoto(/* string initials, int ID, int lvl */)
        {
            ListBoxPersonData.Items[1] = "imy"/*initials*/;
            ListBoxPersonData.Items[2] = "imy"/*ID*/;
            ListBoxPersonData.Items[3] = "imy"/*lvl*/;
        }
        private void UpdatePhoto_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
