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

namespace TechnoShieldApp.Pages.Manager
{
    /// <summary>
    /// Interaction logic for WorkerOfOrderPage.xaml
    /// </summary>
    public partial class WorkerOfOrderPage : Page
    {
        private List<Worker> _listWorkers = new List<Worker>();
        private List<Worker> _listSelectedWorker = new List<Worker>();
        private Order _order;
        private List<Role> _roleList = new List<Role>();
        public WorkerOfOrderPage(Order order)
        {
            InitializeComponent();
            _order = order;
            try
            {
                _listWorkers = AppData.Context.Worker.ToList().Where(p => p.RoleId != 1).ToList();
                _listSelectedWorker = order.Worker.ToList();
                foreach (var item in _listSelectedWorker)
                {
                    if (_listWorkers.FirstOrDefault(p => p == item) != null)
                        _listWorkers.Remove(item);
                }
                _roleList = AppData.Context.Role.ToList().Where(p => p.Id != 1).ToList();
                _roleList.Insert(0, new Role
                {
                    Name = "Все должности"
                });
                CbRole.ItemsSource = _roleList;
                CbRole.SelectedIndex = 0;
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отменить дейсвия?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                AppData.MainFrame.GoBack();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _listWorkers)
            {
                if (_order.Worker.ToList().FirstOrDefault(p => p == item) != null)
                {
                    _order.Worker.Remove(item);
                    AppData.Context.SaveChanges();
                }
            }
            foreach (var item in _listSelectedWorker)
            {
                if (_order.Worker.Count != 0)
                {
                    if (_order.Worker.ToList().FirstOrDefault(p => p == item) == null)
                    {
                        _order.Worker.Add(item);
                        AppData.Context.SaveChanges();
                    }
                }
                else
                {
                    _order.Worker.Add(item);
                    AppData.Context.SaveChanges();
                }
            }
            MessageBox.Show("Бригада собрана", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            AppData.MainFrame.GoBack();
        }

        private void CbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateListViews();
        }

        private void BtnAddWorker_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in LVWorkers.SelectedItems)
            {
                _listWorkers.Remove(item as Worker);
                _listSelectedWorker.Add(item as Worker);
            }
            UpdateListViews();
        }

        private void BtnDeleteWorker_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in LvSelectedWorkers.SelectedItems)
            {
                _listWorkers.Add(item as Worker);
                _listSelectedWorker.Remove(item as Worker);
            }
            UpdateListViews();
        }
        private void UpdateListViews()
        {
            var searchList = _listWorkers;
            var searchSelectedList = _listSelectedWorker;
            LVWorkers.ItemsSource = null;
            LvSelectedWorkers.ItemsSource = null;
            if (CbRole.SelectedIndex > 0)
            {
                searchList = _listWorkers.Where(p => p.Role == CbRole.SelectedItem as Role).ToList().OrderBy(p => p.Id).ToList();
                searchSelectedList = _listSelectedWorker.Where(p => p.Role == CbRole.SelectedItem as Role).ToList();
            }
            LVWorkers.ItemsSource = searchList.Where(p => p.FIO.ToLower().Contains(TbFio.Text.ToLower())).ToList();
            LvSelectedWorkers.ItemsSource = searchSelectedList;
        }

        private void LV_MouseDow(object sender, ListView LV)
        {
            if (LV.ItemsSource != null)
            {
                if ((sender as Border).DataContext is Worker currentWorker)
                {
                    List<Worker> data = new List<Worker>();
                    LV.SelectedItems.Add(currentWorker);
                    foreach (Worker item in LV.SelectedItems)
                        data.Add(item);
                    DragDrop.DoDragDrop(LV, data, DragDropEffects.Move);
                }
            }
        }

        private void LvSelectedWorkers_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(List<Worker>)) as List<Worker>;
            int count = _listWorkers.Count;
            foreach (var item in data)
            {
                if (_listWorkers.Contains(item))
                {
                    _listWorkers.Remove(item);
                    _listSelectedWorker.Add(item);
                }
            }
            if (count != _listWorkers.Count)
                UpdateListViews();
        }

        private void LVWorkers_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(List<Worker>)) as List<Worker>;
            int count = _listSelectedWorker.Count;
            foreach (var item in data)
            {
                if (_listSelectedWorker.Contains(item))
                {
                    _listSelectedWorker.Remove(item);
                    _listWorkers.Add(item);
                }
            }
            if (count != _listSelectedWorker.Count)
                UpdateListViews();
        }

        private void BdWorkers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LV_MouseDow(sender, LVWorkers);
        }

        private void BdSelectedWorkers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LV_MouseDow(sender, LvSelectedWorkers);
        }

        private void TbFio_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateListViews();
        }
    }
}
