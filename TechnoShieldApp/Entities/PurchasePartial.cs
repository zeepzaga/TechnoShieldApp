using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoShieldApp.Entities
{
    public partial class Purchase
    {
        public decimal TotalPrice
        {
            get
            {
                return Count * Product.Price;
            }
            set
            {

            }
        }
    }
}
