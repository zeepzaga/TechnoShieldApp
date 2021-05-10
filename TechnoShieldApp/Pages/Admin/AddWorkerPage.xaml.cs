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
    /// Interaction logic for AddWorkerPage.xaml
    /// </summary>
    public partial class AddWorkerPage : Page
    {
        private Worker _worker;
        private bool _isEdit;
        public AddWorkerPage(Worker worker, bool isEdit)
        {
            InitializeComponent();
            CbRole.ItemsSource = AppData.Context.Role.ToList();
            _isEdit = isEdit;
            _worker = worker;
            if (worker != null)
            {
                DataContext = worker;
                CbRole.SelectedItem = worker.Role;
                if (worker.User != null)
                    PbPassword.Password = worker.User.Login;
            }
        }

        private void ChbShowPass_Checked(object sender, RoutedEventArgs e)
        {
            if (ChbShowPass.IsChecked == true)
            {
                PbPassword.Visibility = Visibility.Collapsed;
                TbPassword.Text = PbPassword.Password;
            }
            else
            {
                PbPassword.Visibility = Visibility.Visible;
                PbPassword.Password = TbPassword.Text;
            }
        }

        private void CbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((CbRole.SelectedItem as Role).Id != 2 && (CbRole.SelectedItem as Role).Id != 1)
            {
                SpUser.IsEnabled = false;
                TbLogin.Text = PbPassword.Password = TbPassword.Text = "";
            }
            else
            {
                SpUser.IsEnabled = true;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ChbShowPass.IsChecked == true) PbPassword.Password = TbPassword.Text;
            if (_worker == null)
            {
                _worker = AppData.Context.Worker.Add(new Worker
                {
                    Address = TbAddress.Text,
                    Name = TbName.Text,
                    LastName = TbLastName.Text,
                    Patronymic = String.IsNullOrWhiteSpace(TbPatronymic.Text) ? null : TbPatronymic.Text,
                    Passport = TbPasport.Text,
                    PIPN = TbPIPN.Text,
                    TelephoneNumber = TbTelephoneNumber.Text,
                    Role = CbRole.SelectedItem as Role,
                });
                if ((CbRole.SelectedItem as Role).Id == 1 || (CbRole.SelectedItem as Role).Id == 2)
                {
                    CreateUser(null);
                }
                AppData.Context.SaveChanges();
                MessageBox.Show("Работник добавлен в БД", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                AppData.MainFrame.GoBack();
            }
            else
            {
                _worker.Address = TbAddress.Text;
                _worker.Name = TbName.Text;
                _worker.LastName = TbLastName.Text;
                _worker.Patronymic = String.IsNullOrWhiteSpace(TbPatronymic.Text) ? null : TbPatronymic.Text;
                _worker.Passport = TbPasport.Text;
                _worker.PIPN = TbPIPN.Text;
                _worker.TelephoneNumber = TbTelephoneNumber.Text;
                _worker.Role = CbRole.SelectedItem as Role;
                if (_worker.User != null && (CbRole.SelectedItem as Role).Id <= 2)
                {
                    if (String.IsNullOrWhiteSpace(TbLogin.Text) || String.IsNullOrWhiteSpace(PbPassword.Password))
                    {
                        MessageBox.Show("Введите все данные для создания аккаунта (Логин, Пароль)",
                   "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (IsLoginFree() == false && _isEdit == false)
                    {
                        MessageBox.Show("Такой логин уже существует",
                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    _worker.User.Login = TbLogin.Text;
                    _worker.User.Password = PbPassword.Password;
                }
                else if (_worker.User == null && (CbRole.SelectedItem as Role).Id <= 2)
                {
                    CreateUser(_worker);
                }
                AppData.Context.SaveChanges();
                MessageBox.Show("Данные о работнике в БД отредактированы", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                AppData.MainFrame.GoBack();
            }
        }
        private void CreateUser(Worker worker)
        {
            if (String.IsNullOrWhiteSpace(TbLogin.Text) && String.IsNullOrWhiteSpace(PbPassword.Password))
            {
                if (MessageBox.Show("Аккаунт для пользователя не создан\nПродолжить без аккаунта?",
                    "Вы уверены?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    _worker = worker;
                    return;
                }
            }
            else if (String.IsNullOrWhiteSpace(TbLogin.Text) || String.IsNullOrWhiteSpace(PbPassword.Password))
            {
                MessageBox.Show("Введите все данные для создания аккаунта (Логин, Пароль)",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (IsLoginFree() == false)
            {
                MessageBox.Show("Такой логин уже существует",
                 "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _worker.User = AppData.Context.User.Add(new User
            {
                Login = TbLogin.Text,
                Password = PbPassword.Password
            });
        }
        private bool IsLoginFree()
        {
            if (AppData.Context.User.ToList().FirstOrDefault(p => p.Login == TbLogin.Text) != null)
                return false;
            return true;
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отменить действия?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                AppData.MainFrame.GoBack();
        }
    }
}
