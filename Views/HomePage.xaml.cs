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

namespace Portals.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();

            RecentConnections.ItemsSource = new[]
{
            new { Name = "Dev Server", Address = "192.168.1.100" },
            new { Name = "QA Server", Address = "10.0.0.15" },
                        new { Name = "Dev Server", Address = "192.168.1.100" },
            new { Name = "QA Server", Address = "10.0.0.15" },
                        new { Name = "Dev Server", Address = "192.168.1.100" },
            new { Name = "QA Server", Address = "10.0.0.15" },
                        new { Name = "Dev Server", Address = "192.168.1.100" },
            new { Name = "QA Server", Address = "10.0.0.15" },
                        new { Name = "Dev Server", Address = "192.168.1.100" },
            new { Name = "QA Server", Address = "10.0.0.15" },
                        new { Name = "Dev Server", Address = "192.168.1.100" },
            new { Name = "QA Server", Address = "10.0.0.15" }
        };

            FavoriteConnections.ItemsSource = new[]
            {
            new { Name = "Production", Address = "203.0.113.10" },
            new { Name = "Database", Address = "203.0.113.20" },
                        new { Name = "Production", Address = "203.0.113.10" },
            new { Name = "Database", Address = "203.0.113.20" }
        };
        }
    }
}
