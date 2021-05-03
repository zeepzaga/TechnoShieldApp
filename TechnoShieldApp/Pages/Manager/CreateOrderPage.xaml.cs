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
        private List<ServiceOfProductInOrder> _listServiceOfProductInOrderDelete = new List<ServiceOfProductInOrder>();
        private decimal _totalOrderPrice = 0;
        Order _order = new Order();

        public CreateOrderPage(Order order)
        {
            InitializeComponent();
            DpDateTimeOfWork.DisplayDateStart = DateTime.Now;
            try
            {
                CbOrganizationName.ItemsSource = AppData.Context.Organization.ToList();
                CbService.IsEnabled = false;
                CbTypeOfService.ItemsSource = AppData.Context.TypeOfService.ToList();
                _listService = AppData.Context.Service.ToList();
                _listProduct = AppData.Context.Product.ToList();
                ICAllProducts.ItemsSource = _listProduct;
                _order = order;
                if (order != null)
                {
                    Title = "Редактирование заказа";
                    CbOrganizationName.SelectedItem = _order.Organization;
                    SpOrganization.IsEnabled = false;
                    foreach (var item in _order.ServiceOfProductInOrder.GroupBy(p => p.Service))
                    {
                        _listSelectedService.Add(item.Key);
                        _listService.Remove(item.Key);
                    }
                    _listServiceOfProductInOrder = _order.ServiceOfProductInOrder.ToList();
                    foreach (var item in _order.Purchase)
                    {
                        _listPurchase.Add(new Purchase
                        {
                            Count = item.Count,
                            Order = _order,
                            Product = item.Product,
                        });
                    }
                    ICSelectedProducts.ItemsSource = _listPurchase;
                    UpdateServicesCollection();
                    SumTotalOrderPrice();
                }
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
                    ServiceOfProductInOrder serviceOfProductInOrder = _listServiceOfProductInOrder.FirstOrDefault(p => p.Service == LbSelectedService.SelectedItem as Service && p.Product == item);
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
            SumTotalOrderPrice();
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

        private void SumTotalOrderPrice()
        {
            _totalOrderPrice = 0;
            foreach (var item in _listSelectedService)
            {
                _totalOrderPrice += item.Price;
            }
            foreach (var item in _listServiceOfProductInOrder)
            {
                if (item.Product != null)
                    _totalOrderPrice += item.Product.Price * item.Count;
            }
            foreach (var item in _listPurchase)
            {
                _totalOrderPrice += item.Count * item.Product.Price;
            }
            TbTotalPrice.Text = $"Общая стоимость заказа: {_totalOrderPrice} р.";
        }

        private void TbCountProductOnOrder_TextChanged(object sender, TextChangedEventArgs e)
        {

            var product = (sender as TextBox).DataContext as Product;
            try
            {
                int count = 0;
                ServiceOfProductInOrder serviceOfProductInOrder = _listServiceOfProductInOrder
                    .FirstOrDefault(p => p.Product == product && p.Service == LbSelectedService.SelectedItem as Service);
                int.TryParse((sender as TextBox).Text, out count);
                if (count == 0 && serviceOfProductInOrder != null)
                {
                    _listServiceOfProductInOrder.Remove(serviceOfProductInOrder);
                    _listServiceOfProductInOrderDelete.Add(serviceOfProductInOrder);
                }
                else
                {
                    if (serviceOfProductInOrder != null)
                    {
                        serviceOfProductInOrder.Count = count;
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
                    if (_listServiceOfProductInOrderDelete
                        .FirstOrDefault(p => p.Product == product && p.Service == LbSelectedService.SelectedItem as Service) != null)
                    {
                        _listServiceOfProductInOrderDelete.Remove(_listServiceOfProductInOrderDelete
                        .FirstOrDefault(p => p.Product == product && p.Service == LbSelectedService.SelectedItem as Service));
                    }
                }

                SumTotalOrderPrice();
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
            SumTotalOrderPrice();
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
            else
            {
                organization = CbOrganizationName.SelectedItem as Organization;
            }
            DateTime? dateTimeOfWork = null;
            if (DpDateTimeOfWork.SelectedDate != null)
                dateTimeOfWork = new DateTime(DpDateTimeOfWork.SelectedDate.Value.Year, DpDateTimeOfWork.SelectedDate.Value.Month,
                    DpDateTimeOfWork.SelectedDate.Value.Day, Convert.ToInt32(TbHoursOrder.Text), int.Parse(TbMinutes.Text), 0);
            if (_order == null)
            {
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
                        Count = item.Count,
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
                MessageBox.Show("Заказ успешно создан\nВас перенаправит на страницу добалвения бригады для заказа", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                AppData.MainFrame.Navigate(new OrderDetailViewPage(_order));
            }
            else
            {
                foreach (var item in _listServiceOfProductInOrder)
                {
                    ServiceOfProductInOrder serviceOfProductInOrder
                        = AppData.Context.ServiceOfProductInOrder.ToList()
                                            .FirstOrDefault(p => p.Service == item.Service
                                            && p.Product == item.Product);
                    if (serviceOfProductInOrder != null)
                    {
                        serviceOfProductInOrder.Count = item.Count;
                        AppData.Context.SaveChanges();
                    }
                    else
                    {
                        AppData.Context.ServiceOfProductInOrder.Add(new ServiceOfProductInOrder
                        {
                            Service = item.Service,
                            Count = item.Count,
                            Order = _order,
                            Product = item.Product,
                        });
                        AppData.Context.SaveChanges();
                    }
                }
                foreach (var item in _listServiceOfProductInOrderDelete)
                {
                    ServiceOfProductInOrder serviceOfProductInOrder =
                        AppData.Context.ServiceOfProductInOrder.ToList()
                        .FirstOrDefault(p => p.Product == item.Product && p.Service == item.Service);
                    if (serviceOfProductInOrder != null)
                    {
                        AppData.Context.ServiceOfProductInOrder.Remove(serviceOfProductInOrder);
                        AppData.Context.SaveChanges();
                    }
                }
                foreach (var item in _listPurchase)
                {
                    Purchase purchase = AppData.Context.Purchase.ToList()
                        .FirstOrDefault(p => p.Product == item.Product);
                    if (purchase != null)
                    {
                        purchase.Count = item.Count;
                        AppData.Context.SaveChanges();
                    }
                    else
                    {
                        AppData.Context.Purchase.Add(new Purchase
                        {
                            Count = item.Count,
                            Order = _order,
                            Product = item.Product
                        });
                        AppData.Context.SaveChanges();
                    }
                }
                List<Purchase> purchasesForDelete = new List<Purchase>();
                foreach (var item in AppData.Context.Purchase.ToList().Where(p=>p.Order == _order))
                {
                    if (_listPurchase.FirstOrDefault(p => p.Product == item.Product) == null)
                        purchasesForDelete.Add(item);
                }
                foreach (var item in purchasesForDelete)
                {
                    AppData.Context.Purchase.Remove(item);
                    AppData.Context.SaveChanges();
                }
                _order.DateTimeOfWork = dateTimeOfWork;
                AppData.Context.SaveChanges();
                AppData.MainFrame.GoBack();
            }

        }
        private void TbMinutes_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
            if (e.Key == Key.Enter)
            {
                TbProductPurchase_LostFocus(null, null);
            }
        }

        private void TbHoursOrder_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.Text, 0);
        }

        private void CbOrganizationName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SpOrganization.DataContext = CbOrganizationName.SelectedItem as Organization;
        }

        private void TbProductPurchase_TextChanged(object sender, TextChangedEventArgs e)
        {
            var purchase = (sender as TextBox).DataContext as Purchase;
            try
            {
                _listPurchase.FirstOrDefault(p => p.Product == purchase.Product).Count = int.Parse((sender as TextBox).Text);
            }
            catch
            {

            }
            SumTotalOrderPrice();
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
                {
                    _listPurchase.FirstOrDefault(p => p.Product == product).Count += 1;
                }
            }
            ICSelectedProducts.ItemsSource = null;
            ICSelectedProducts.ItemsSource = _listPurchase;
            SumTotalOrderPrice();
        }

        private void TblDeleteProductFromOrder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _listPurchase.Remove((sender as TextBlock).DataContext as Purchase);
            ICSelectedProducts.ItemsSource = null;
            ICSelectedProducts.ItemsSource = _listPurchase;
            SumTotalOrderPrice();
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
            SumTotalOrderPrice();
        }

        private void TbProductPurchase_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Purchase purchase = (sender as TextBox).DataContext as Purchase;
                if (purchase.Count == 0)
                {
                    _listPurchase.Remove(purchase);
                }
            }
            catch
            {
            }
            ICSelectedProducts.Items.Refresh();
            SumTotalOrderPrice();
        }
    }
}
