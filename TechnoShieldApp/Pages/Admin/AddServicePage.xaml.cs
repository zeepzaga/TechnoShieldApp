using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AddServicePage.xaml
    /// </summary>
    public partial class AddServicePage : Page
    {
        List<Product> _listProduct = new List<Product>();
        List<Product> _listSelectedProduct = new List<Product>();
        Service _service;
        public AddServicePage(Service service)
        {
            InitializeComponent();
            try
            {
                CbTypeOfService.ItemsSource = AppData.Context.TypeOfService.ToList();
                _listProduct = AppData.Context.Product.ToList().Where(p => p.TypeOfService == null).ToList();
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _service = service;
            if (service != null)
            {
                CbTypeOfService.SelectedItem = service.TypeOfService;
                BtnSave.Content = "Редактировать";
                Title = "Редактирование услуги";
                DataContext = service;
                CbTypeOfService.SelectedItem = service.TypeOfService;
            }
            UpdateList();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отменить действия?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                AppData.MainFrame.GoBack();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            decimal _price = 0;
            if (!String.IsNullOrWhiteSpace(TbName.Text) || CbTypeOfService.SelectedIndex < 0)
            {
                //ДОДЕЛАЙ ФОРЧ НА ДОБАВЕНИЕ НОВОЙ ПОДОБНОЙ СВЗЯИ У ТЕБЯ ЕСТЬ ТОЛЬКО ЕЁ ОТЧИЩЕНИЕ
                foreach (var item in _listProduct)
                {
                    var product = AppData.Context.Product.ToList().FirstOrDefault(p => p.TypeOfService == item.TypeOfService && p.Id == item.Id);
                    product.TypeOfService = null;
                    AppData.Context.SaveChanges();
                }
                foreach (var item in _listSelectedProduct)
                {
                    TypeOfService typeOfService = CbTypeOfService.SelectedItem as TypeOfService;
                    if (typeOfService.Product.ToList().Where(p => p.Id == item.Id) == null)
                    {
                        var product = AppData.Context.Product.ToList().FirstOrDefault(p=>p.Id == item.Id);
                        product.TypeOfService = typeOfService;
                        AppData.Context.SaveChanges();
                    }
                }
                if (_service == null)
                {
                    try
                    {
                        AppData.Context.Service.Add(new Service
                        {
                            Name = TbName.Text,
                            Price = decimal.TryParse(TbPrice.Text, out _price) == false ? 0 : _price,
                            TypeOfService = CbTypeOfService.SelectedItem as TypeOfService,
                            Description = TbDexription.Text,
                        });
                        MessageBox.Show("Улуга добавлена", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        AppData.Context.SaveChanges();
                        AppData.MainFrame.GoBack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    _service.Name = TbName.Text;
                    _service.Price = decimal.TryParse(TbPrice.Text, out _price) == false ? 0 : _price;
                    _service.TypeOfService = CbTypeOfService.SelectedItem as TypeOfService;
                    _service.Description = TbDexription.Text;
                    AppData.Context.SaveChanges();
                    MessageBox.Show("Улуга отрдактрована", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppData.MainFrame.GoBack();
                }
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            Product product = CbProduct.SelectedItem as Product;
            if (product != null)
            {
                _listProduct.Remove(product);
                _listSelectedProduct.Add(product);
                UpdateList();
            }
        }
        private void UpdateList()
        {
            CbProduct.ItemsSource = null;
            CbProduct.ItemsSource = _listProduct;
            ICSelectedProduct.ItemsSource = null;
            ICSelectedProduct.ItemsSource = _listSelectedProduct.OrderBy(p=>p.Id).ToList();
        }



        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Product;
            _listProduct.Add(product);
            _listSelectedProduct.Remove(product);
            UpdateList();
        }

        private void CbTypeOfService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var bufflist = AppData.Context.Product.ToList();
                _listProduct = bufflist.Where(p => p.TypeOfService == null).ToList();
                _listSelectedProduct = bufflist.Where(p => p.TypeOfService == CbTypeOfService.SelectedItem as TypeOfService).ToList();
                UpdateList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    }
}
