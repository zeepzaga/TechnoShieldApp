using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoShieldApp.Entities
{
    public partial class ServiceOfProductInOrder
    {
        public decimal TotalPriceForOneService
        {
            get
            {
                decimal result = 0;
                result += Service.Price;
                foreach (var item in AppData.Context.ServiceOfProductInOrder.ToList().Where(p => p.Service == Service && p.Order == Order).ToList())
                {
                    if (item.Product != null)
                        result += item.Product.Price * item.Count;
                }
                return result;
            }
        }
    }
}
