using System.Windows.Controls;

namespace Portals.Views
{
    public partial class WorkspacePage : UserControl
    {
        public WorkspacePage()
        {
            InitializeComponent();
        }

        // Fired when switching tabs
        private void WorkspaceTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If user clicks the "+" tab → open new connection
            if (WorkspaceTabs.SelectedItem == NewConnectionTab)
            {
                AddNewConnectionTab("New Connection");
            }
        }

        // Handle button click inside "+" tab
        private void NewConnection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddNewConnectionTab("Custom Connection");
        }

        // Utility to add a new connection tab
        private void AddNewConnectionTab(string header)
        {
            var tab = new TabItem
            {
                Header = header,
                Content = new Grid
                {
                    Background = (System.Windows.Media.Brush)FindResource("SecondaryBrush"),
                    Children =
                    {
                        new TextBlock
                        {
                            Text = $"Connected to {header}",
                            Foreground = System.Windows.Media.Brushes.White,
                            FontSize = 16,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                        }
                    }
                }
            };

            // Insert before the "+" tab
            WorkspaceTabs.Items.Insert(WorkspaceTabs.Items.Count - 1, tab);
            WorkspaceTabs.SelectedItem = tab;
        }
    }
}
