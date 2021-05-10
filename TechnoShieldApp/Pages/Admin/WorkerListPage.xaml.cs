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
    /// Interaction logic for WorkerListPage.xaml
    /// </summary>
    public partial class WorkerListPage : Page
    {
        List<Role> roles = new List<Role>();
        public WorkerListPage()
        {
            InitializeComponent();
            roles = AppData.Context.Role.ToList();
            roles.Insert(0, new Role
            {
                 Name = "Все должности"
            });
            CbRole.ItemsSource = roles;
        }

        private void BtnAboutWorker_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new AddWorkerPage((sender as Button).DataContext as Worker, true));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }
        private void UpdateList()
        {
            var _searchList = AppData.Context.Worker.ToList();
            if (CbRole.SelectedIndex > 0)
            {
                _searchList = _searchList.Where(p => p.Role == CbRole.SelectedItem as Role).ToList();
            }
            _searchList = _searchList.Where(p => p.FIO.ToLower().Contains(TbFio.Text.ToLower())).ToList();
            DgWorkers.ItemsSource = null;
            DgWorkers.ItemsSource = _searchList;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить работника из БД и все данные связанные с ним?\nВ том числе и численность в заказах?",
                "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Worker currentWorker = (sender as Button).DataContext as Worker;
                currentWorker.Order.Clear();
                AppData.Context.Worker.Remove(currentWorker);
                AppData.Context.SaveChanges();
            }
        }

        private void TbFio_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList();
        }

        private void CbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }

        private void BtnAddWorker_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.Navigate(new AddWorkerPage(null, false));
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            TbFio.Text = "";
            CbRole.SelectedIndex = 0;
        }
    }
}
