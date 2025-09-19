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
using System.Windows.Shapes;

namespace Portals
{
    /// <summary>
    /// Interaction logic for ConnectionDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : Window
    {
        public string Server => ServerBox.Text;
        public string Username => UserBox.Text;
        public ConnectionDialog()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if ((string.IsNullOrWhiteSpace(Server) || string.IsNullOrWhiteSpace(Username)))
            {
                MessageBox.Show("Please enter server and username.");
                return;
            }

            DialogResult = true; // Close dialog and indicate success
            Close();
        }
    }
}
