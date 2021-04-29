using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TechnoShieldApp.Entities
{
    public partial class Worker
    {
        public string FIOAndPosition
        {
            get
            {
                return $"{LastName} {Name} {Patronymic} ({Role.Name})";
            }
        }
       
    }
}
