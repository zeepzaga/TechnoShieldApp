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
using System.Windows.Threading;
using TechnoShieldApp.Entities;

namespace TechnoShieldApp.Pages
{
    /// <summary>
    /// Interaction logic for AutorizationPage.xaml
    /// </summary>
    public partial class AutorizationPage : Page
    {
        int tryCount = 0;
        DispatcherTimer DispatcherTimer = new DispatcherTimer();
        public AutorizationPage()
        {
            InitializeComponent();
            DispatcherTimer.Tick += DispatcherTimer_Tick;
            DispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.Login) && !String.IsNullOrWhiteSpace(Properties.Settings.Default.Password))
            {
                TbLogin.Text = Properties.Settings.Default.Login;
                PbPassword.Password = Properties.Settings.Default.Password;
                ChbIsRemember.IsChecked = true;
            }
            Properties.Settings.Default.Login = "";
            Properties.Settings.Default.Password = "";
            Properties.Settings.Default.Save();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer.Stop();
            BtnLogin.IsEnabled = true;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string error = "";
            if (String.IsNullOrWhiteSpace(TbLogin.Text)) error += "• Введите логин\n";
            if (String.IsNullOrWhiteSpace(PbPassword.Password)) error += "• Введите пароль\n";
            if (!String.IsNullOrWhiteSpace(error))
            {
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            User user = null;
            try
            {
                user = AppData.Context.User.ToList().FirstOrDefault(p => p.Login == TbLogin.Text && p.Password == PbPassword.Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (user == null)
            {
                MessageBox.Show("• Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                tryCount++;
                if (tryCount >= 3)
                {
                    DispatcherTimer.Start();
                    MessageBox.Show("Было совершенно 3 неверные попытки входа, поробуйте через 10 секунд", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    BtnLogin.IsEnabled = false;
                }
                return;
            }
            if (ChbIsRemember.IsChecked == true)
            {
                Properties.Settings.Default.Login = TbLogin.Text;
                Properties.Settings.Default.Password = PbPassword.Password;
                Properties.Settings.Default.Save();
            }
            AppData.currentUser = user;
            switch (user.Worker.FirstOrDefault().RoleId)
            {
                case 1:
                    AppData.MainFrame.Navigate(new Admin.AdminMenuPage());
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }
    }
}
