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
    /// Interaction logic for CreateOrderPage.xaml
    /// </summary>
    public partial class CreateOrderPage : Page
    {
        private List<Service> _listSelectedService = new List<Service>();
        private List<Service> _listService = new List<Service>();
        private List<Product> _listProduct = new List<Product>();
        private List<Purchase> _listPurchase = new List<Purchase>();
        private List<ServiceOfProductInOrder> _listServiceOfProductInOrder = new List<ServiceOfProductInOrder>();
        Order order = new Order();

        public CreateOrderPage()
        {
            InitializeComponent();

            try
            {
                CbOrganizationName.ItemsSource = AppData.Context.Organization.ToList();
                CbService.IsEnabled = false;
                CbTypeOfService.ItemsSource = AppData.Context.TypeOfService.ToList();
                _listService = AppData.Context.Service.ToList();
                _listProduct = AppData.Context.Product.ToList();
                ICAllProducts.ItemsSource = _listProduct;
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                BtnSave.IsEnabled = false;
            }
        }

        private void LbSelectedService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ICProduct.ItemsSource = null;
                foreach (var item in _listProduct.Where(p => p.TypeOfService == (LbSelectedService.SelectedItem as Service).TypeOfService).ToList())
                {
                    ServiceOfProductInOrder serviceOfProductInOrder = _listServiceOfProductInOrder.FirstOrDefault(p => p.Service == LbSelectedService.SelectedItem as Service && p.Product.Id == item.Id);
                    if (serviceOfProductInOrder != null)
                        item.CountOnOrder = serviceOfProductInOrder.Count;
                    else
                        item.CountOnOrder = 0;
                }
                ICProduct.ItemsSource = _listProduct.Where(p => p.TypeOfService == (LbSelectedService.SelectedItem as Service).TypeOfService).ToList();
            }
            catch
            {

            }
        }

        private void BtnAddService_Click(object sender, RoutedEventArgs e)
        {
            _listSelectedService.Add(CbService.SelectedItem as Service);
            _listService.Remove(CbService.SelectedItem as Service);
            UpdateServicesCollection();
        }

        private void UpdateServicesCollection()
        {
            CbService.ItemsSource = null;
            CbService.ItemsSource = _listService.Where(p => p.TypeOfService == CbTypeOfService.SelectedItem as TypeOfService).ToList().OrderBy(p => p.Id).ToList();
            CbService.IsEnabled = true;
            BtnAddService.IsEnabled = false;
            LbSelectedService.ItemsSource = null;
            LbSelectedService.ItemsSource = _listSelectedService;
        }

        private void TbCountProductOnOrder_TextChanged(object sender, TextChangedEventArgs e)
        {

            var product = (sender as TextBox).DataContext as Product;
            try
            {
                ServiceOfProductInOrder serviceOfProductInOrder = _listServiceOfProductInOrder.FirstOrDefault(p => p.Product == product && p.Service == LbSelectedService.SelectedItem as Service);
                if (serviceOfProductInOrder != null)
                {
                    serviceOfProductInOrder.Count = int.Parse((sender as TextBox).Text);
                }
                else
                {
                    _listServiceOfProductInOrder.Add(new ServiceOfProductInOrder
                    {
                        Count = int.Parse((sender as TextBox).Text),
                        Product = product,
                        Service = LbSelectedService.SelectedItem as Service
                    });
                }


                //product.CountOnOrder = int.Parse((sender as TextBox).Text);
                //var productOnService = _listServiceOfProductInOrder.Where(p => p.Product.Id == product.Id && p.Service == LbSelectedService.SelectedItem as Service).ToList();
                //if (productOnService.Count < product.CountOnOrder)
                //{
                //    var diff = product.CountOnOrder - productOnService.Count;
                //    for (int i = 0; i < diff; i++)
                //    {
                //        _listServiceOfProductInOrder.Add(new ServiceOfProductInOrder
                //        {
                //            Service = LbSelectedService.SelectedItem as Service,
                //            Product = product,
                //        });
                //    }
                //}
                //else if (productOnService.Count > product.CountOnOrder)
                //{
                //    var diff = productOnService.Count - product.CountOnOrder;
                //    for (int i = 0; i < diff; i++)
                //    {
                //        _listServiceOfProductInOrder.Remove(_listServiceOfProductInOrder.FirstOrDefault(p => p.Service == LbSelectedService.SelectedItem as Service
                //         && p.Product == (sender as TextBox).DataContext as Product));
                //    }
                //}
            }
            catch
            {

            }
        }

        private void CbTypeOfService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServicesCollection();
        }

        private void CbService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnAddService.IsEnabled = true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Organization organization = new Organization();
            if (CbOrganizationName.SelectedItem == null)
            {
                organization.Name = CbOrganizationName.Text;
                organization.TelephoneNumber = TbOrganizationTelephone.Text;
                organization.Address = TbOrganizationAddress.Text;
            }
            DateTime? dateTimeOfWork = null;
            if (DpDateTimeOfWork.SelectedDate != null)
                dateTimeOfWork = new DateTime(DpDateTimeOfWork.SelectedDate.Value.Year, DpDateTimeOfWork.SelectedDate.Value.Month,
                    DpDateTimeOfWork.SelectedDate.Value.Day, Convert.ToInt32(TbHoursOrder.Text), int.Parse(TbMinutes.Text), 0);

            Order order = AppData.Context.Order.Add(new Order
            {
                StatusOfOrderId = 1,
                DateTimeOfCreate = DateTime.Now,
                Organization = organization,
                DateTimeOfWork = dateTimeOfWork,
            });
            foreach (var item in _listServiceOfProductInOrder)
            {
                AppData.Context.ServiceOfProductInOrder.Add(new ServiceOfProductInOrder
                {
                    Product = item.Product,
                    Service = item.Service,
                    Order = order
                });
            }
            foreach (var item in _listPurchase)
            {
                AppData.Context.Purchase.Add(new Purchase
                {
                    Product = item.Product,
                    Count = item.Count,
                    Order = order
                });
            }
            foreach (var item in _listSelectedService)
            {
                if (_listServiceOfProductInOrder.FirstOrDefault(p => p.Service == item) == null)
                {
                    AppData.Context.ServiceOfProductInOrder.Add(new ServiceOfProductInOrder
                    {
                        Product = null,
                        Count = 0,
                        Order = order,
                        Service = item,
                    });
                }
            }
            AppData.Context.SaveChanges();
        }
        private void TbMinutes_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        private void TbHoursOrder_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.Text, 0);
        }

        private void CbOrganizationName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TbProductPurchase_TextChanged(object sender, TextChangedEventArgs e)
        {
            var product = (sender as TextBox).DataContext as Product;
            try
            {
                _listPurchase.FirstOrDefault(p => p.Product == product).Count = int.Parse((sender as TextBox).Text);
            }
            catch
            {

            }
        }

        private void BtnAddToPurchase_Click(object sender, RoutedEventArgs e)
        {
            Product product = (sender as Button).DataContext as Product;
            if (_listPurchase.Where(p => p.Product == product).Count() == 0)
            {
                _listPurchase.Add(new Purchase
                {
                    Product = product,
                    Count = 1
                });
            }
            else
            {
                if (_listPurchase.FirstOrDefault(p => p.Product == product).Count + 1 <= 999)
                    _listPurchase.FirstOrDefault(p => p.Product == product).Count += 1;
            }
            ICSelectedProducts.ItemsSource = null;
            ICSelectedProducts.ItemsSource = _listPurchase;
        }

        private void TblDeleteProductFromOrder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _listPurchase.Remove((sender as TextBlock).DataContext as Purchase);
            ICSelectedProducts.ItemsSource = null;
            ICSelectedProducts.ItemsSource = _listPurchase;
        }

        private void TbSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICAllProducts.ItemsSource = null;
            ICAllProducts.ItemsSource = _listProduct.Where(p => p.Name.ToLower().Contains(TbSearchProduct.Text.ToLower())).ToList();
        }

        private void BtnDeleteServiceFromOrder_Click(object sender, RoutedEventArgs e)
        {
            _listSelectedService.Remove((sender as Button).DataContext as Service);
            _listServiceOfProductInOrder.RemoveAll(p => p.Service == (sender as Button).DataContext as Service);
            _listService.Add((sender as Button).DataContext as Service);
            UpdateServicesCollection();
        }
    }
}
