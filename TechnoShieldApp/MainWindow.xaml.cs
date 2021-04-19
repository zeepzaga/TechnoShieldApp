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
using TechnoShieldApp.Pages;
using TechnoShieldApp.Pages.Manager;

namespace TechnoShieldApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppData.MainFrame = MainFrame;
            AppData.MainFrame.Navigate(new CreateOrderPage());
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            Page page = AppData.MainFrame.Content as Page;
            if (page is AutorizationPage)
            {
                TblBack.Visibility = Visibility.Collapsed;
                BtnLogout.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (AppData.currentUser != null)
                    BtnLogout.Visibility = Visibility.Visible;
                TblBack.Visibility = Visibility.Visible;
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Properties.Settings.Default.Login = "";
                Properties.Settings.Default.Password = "";
                Properties.Settings.Default.Save();
                AppData.MainFrame.Navigate(new AutorizationPage());
            }
        }

        private void TblBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AppData.MainFrame.CanGoBack)
                AppData.MainFrame.GoBack();
        }
    }
}
