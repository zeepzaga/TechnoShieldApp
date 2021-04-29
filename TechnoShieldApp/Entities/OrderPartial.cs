using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TechnoShieldApp.Entities
{
    public partial class Order
    {
        public List<ServiceOrder> OrderServiceList
        {
            get
            {
                List<ServiceOrder> serviceOrders = new List<ServiceOrder>();
                foreach (var service in ServiceOfProductInOrder.GroupBy(p => p.Service))
                {
                    List<ProductService> productServices = new List<ProductService>();
                    foreach (var product in ServiceOfProductInOrder.Where(p => p.Service == service.Key))
                    {
                        if (product.Service == service.Key)
                            if (product.ProductId != null)
                            {
                                productServices.Add(new ProductService
                                {
                                    Product = product.Product,
                                    CountInService = product.Count
                                });
                            }
                    }
                    serviceOrders.Add(new ServiceOrder
                    {
                        Service = service.Key,
                        ListOfProduct = productServices
                    });
                }
                return serviceOrders;
            }
        }
        public Visibility Visibility
        {
            get
            {
                if (Worker.Count > 0)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }
        public Visibility DateStartWorkVisible
        {
            get
            {
                if (DateTimeOfWork != null)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }
        public GridLength WorkerGridLength
        {
            get
            {
                if (Worker.Count > 0)
                    return new GridLength(1, GridUnitType.Star);
                return new GridLength(0);
            }
        }
        public decimal Height
        {
            get
            {
                if (Worker.Count > 0)
                    return 1;
                return 0;
            }
        }
        public class ServiceOrder
        {
            public Service Service { get; set; }
            public List<ProductService> ListOfProduct { get; set; }
        }
        public class ProductService
        {
            public Product Product { get; set; }
            public int CountInService { get; set; }
        }
    }
}
