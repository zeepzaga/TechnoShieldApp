using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TechnoShieldApp.Entities
{
    public static class AppData
    {
        public static Frame MainFrame;
        public static TechnoShieldBaseEntities Context = new TechnoShieldBaseEntities();
        public static User currentUser;
    }
}
