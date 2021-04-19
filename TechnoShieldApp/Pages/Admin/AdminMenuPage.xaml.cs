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
using TechnoShieldApp.Entities;

namespace TechnoShieldApp.Pages.Admin
{
    /// <summary>
    /// Interaction logic for AdminMenuPage.xaml
    /// </summary>
    public partial class AdminMenuPage : Page
    {
        public AdminMenuPage()
        {
            InitializeComponent();
        }

        private void BtnProductsAndServices_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new ProductsAndServicesPage());
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnWorkers_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStat_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
