using AxMSTSCLib;
using MSTSCLib;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace Portals
{
    public partial class MainWindow : Window
    {
        private TabItem _lastSelectedTab = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Title Bar
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove(); // allow dragging the window
        }
        #endregion

        private void NavButton_NewConnection(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("New connection button clicked");

            var dialog = new ConnectionDialog { Owner = this };
            if (dialog.ShowDialog() == true)
            {

                var newTab = CreateRdpTab(dialog.Server, dialog.Username);
                var plusTab = MainTabs.Items.OfType<TabItem>().FirstOrDefault(t => t.Header?.ToString() == "+");

                if (plusTab != null)
                {
                    int plusIndex = MainTabs.Items.IndexOf(plusTab);
                    MainTabs.Items.Insert(plusIndex, newTab);
                }
                else
                {
                    MainTabs.Items.Add(newTab);
                }

                MainTabs.SelectedItem = newTab;
            }
            else
            {
                Console.WriteLine("Dialog cancelled");
                if (_lastSelectedTab != null)
                    MainTabs.SelectedItem = _lastSelectedTab;
                else
                    MainTabs.SelectedIndex = 0;
            }
        }


        private void MainTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (TabItem removed in e.RemovedItems.OfType<TabItem>())
            {
                if (removed.Content is WindowsFormsHost host &&
                    host.Child is AxMsRdpClient9NotSafeForScripting client)
                {
                    if (client.Connected == 1)
                    {
                        //Console.WriteLine("Disconnecting hidden tab...");
                        //client.Disconnect();
                    }
                }
            }

            foreach (TabItem added in e.AddedItems.OfType<TabItem>())
            {
                if (added.Content is WindowsFormsHost host &&
                    host.Child is AxMsRdpClient9NotSafeForScripting client)
                {
                    if (client.Connected != 1)
                    {
                        //Console.WriteLine("Reconnecting visible tab...");
                        try { client.Connect(); }
                        catch (Exception ex) { Console.WriteLine("Reconnect failed: " + ex.Message); }
                    }
                }
            }
        }

        private TabItem CreateRdpTab(string server, string username, string password = null, string domain = "OBC")
        {
            var rdpClient = new AxMsRdpClient9NotSafeForScripting();
            ((System.ComponentModel.ISupportInitialize)(rdpClient)).BeginInit();
            rdpClient.Dock = System.Windows.Forms.DockStyle.Fill;

            var host = new WindowsFormsHost
            {
                Child = rdpClient,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            ((System.ComponentModel.ISupportInitialize)(rdpClient)).EndInit();

            // Event hooks
            rdpClient.OnConnecting += (s, e) => Console.WriteLine("Event: Connecting...");
            rdpClient.OnConnected += (s, e) => Console.WriteLine("Event: Connected!");
            rdpClient.OnLoginComplete += (s, e) => Console.WriteLine("Event: Login complete");
            rdpClient.OnDisconnected += (s, e) => Console.WriteLine("Event: Disconnected, reason=" + e.discReason);
            rdpClient.OnFatalError += (s, e) => Console.WriteLine("Event: Fatal error=" + e.errorCode);
            rdpClient.OnWarning += (s, e) => Console.WriteLine("Event: Warning=" + e.warningCode);

            // Configure — but DO NOT call Connect yet
            rdpClient.Server = server;
            rdpClient.UserName = username;
            if (!string.IsNullOrEmpty(domain))
                rdpClient.Domain = domain;

            rdpClient.AdvancedSettings8.EnableCredSspSupport = true;
            rdpClient.ColorDepth = 32;
            rdpClient.DesktopWidth = (int)SystemParameters.PrimaryScreenWidth;
            rdpClient.DesktopHeight = (int)SystemParameters.PrimaryScreenHeight;

            //if (!string.IsNullOrEmpty(password))
            //{
            //    var secured = (IMsTscNonScriptable)rdpClient.GetOcx();
            //    secured.ClearTextPassword = password;
            //}

            // Connect only after the host is visible
            host.Loaded += (s, e) =>
            {

                rdpClient.AdvancedSettings8.SmartSizing = true;

                if (rdpClient.Connected == 1)
                {
                    rdpClient.UpdateSessionDisplaySettings(
                        (uint)host.ActualWidth,
                        (uint)host.ActualHeight,
                        (uint)host.ActualWidth,
                        (uint)host.ActualHeight,
                        0, 0, 0);
                }

            try
                {
                    if (rdpClient.Connected != 1)
                    {
                        rdpClient.Connect();
                    }
                }
                catch (COMException ex)
                {
                    Console.WriteLine($"COMException: 0x{ex.HResult:X} - {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connect failed: " + ex.Message);
                }
            };

            // Wrap host in a tab item
            var tab = new TabItem { Header = $"{username}@{server}", Content = host };
            return tab;
        }

    }
}
