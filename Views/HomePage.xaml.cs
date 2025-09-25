using AxMSTSCLib;
using MSTSCLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Media;


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

            RecentConnectionsList.ItemsSource = ConnectionManager.Instance.RecentConnections.Connections;
            FavoriteConnectionsList.ItemsSource = ConnectionManager.Instance.FavoriteConnections.Connections;


        }

        private void OpenConnectionDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new ConnectionDialog();
            if (dialog.ShowDialog() == true)
            {
                var host = CreateRdpHost(dialog.Server, dialog.Username);

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                var selectedTab = mainWindow.WorkspaceTabsControl.SelectedItem as TabItem;

                if (selectedTab != null)
                {
                    
                    selectedTab.Header = $"{dialog.Username}@{dialog.Server}";
                    mainWindow.MainContent.Content = host;
                }
            }
        }

        private WindowsFormsHost CreateRdpHost(string address, string username, string password = "", string domain = "OBC")
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

            // configure events, server, username, domain, password etc...
            rdpClient.Server = address;
            rdpClient.UserName = username;
            rdpClient.Domain = domain;
            rdpClient.AdvancedSettings8.EnableCredSspSupport = true;

            host.Loaded += (s, e) =>
            {
                try { rdpClient.Connect(); }
                catch (Exception ex) { Console.WriteLine("Connect failed: " + ex.Message); }
                finally { if(rdpClient.Connected == 1) 
                        ConnectionManager.Instance.RecentConnections.Add(
                            new Connection { Name = username, Password = password, Address = address, Domain = domain}); 
                }
            };

            return host;
        }

        //private TabItem CreateRdpTab(string server, string username, string password = null, string domain = "OBC")
        //{
        //    var rdpClient = new AxMsRdpClient9NotSafeForScripting();
        //    ((System.ComponentModel.ISupportInitialize)(rdpClient)).BeginInit();
        //    rdpClient.Dock = System.Windows.Forms.DockStyle.Fill;

        //    var host = new WindowsFormsHost
        //    {
        //        Child = rdpClient,
        //        HorizontalAlignment = HorizontalAlignment.Stretch,
        //        VerticalAlignment = VerticalAlignment.Stretch
        //    };
        //    ((System.ComponentModel.ISupportInitialize)(rdpClient)).EndInit();

        //    // Event hooks
        //    rdpClient.OnConnecting += (s, e) => Console.WriteLine("Event: Connecting...");
        //    rdpClient.OnConnected += (s, e) => Console.WriteLine("Event: Connected!");
        //    rdpClient.OnLoginComplete += (s, e) => Console.WriteLine("Event: Login complete");
        //    rdpClient.OnDisconnected += (s, e) => Console.WriteLine("Event: Disconnected, reason=" + e.discReason);
        //    rdpClient.OnFatalError += (s, e) => Console.WriteLine("Event: Fatal error=" + e.errorCode);
        //    rdpClient.OnWarning += (s, e) => Console.WriteLine("Event: Warning=" + e.warningCode);

        //    // Configure — but DO NOT call Connect yet
        //    rdpClient.Server = server;
        //    rdpClient.UserName = username;
        //    if (!string.IsNullOrEmpty(domain))
        //        rdpClient.Domain = domain;

        //    rdpClient.AdvancedSettings8.EnableCredSspSupport = true;
        //    rdpClient.ColorDepth = 32;
        //    rdpClient.DesktopWidth = (int)SystemParameters.PrimaryScreenWidth;
        //    rdpClient.DesktopHeight = (int)SystemParameters.PrimaryScreenHeight;

        //    //if (!string.IsNullOrEmpty(password))
        //    //{
        //    //    var secured = (IMsTscNonScriptable)rdpClient.GetOcx();
        //    //    secured.ClearTextPassword = password;
        //    //}

        //    // Connect only after the host is visible
        //    host.Loaded += (s, e) =>
        //    {

        //        rdpClient.AdvancedSettings8.SmartSizing = true;

        //        if (rdpClient.Connected == 1)
        //        {
        //            rdpClient.UpdateSessionDisplaySettings(
        //                (uint)host.ActualWidth,
        //                (uint)host.ActualHeight,
        //                (uint)host.ActualWidth,
        //                (uint)host.ActualHeight,
        //                0, 0, 0);
        //        }

        //        try
        //        {
        //            if (rdpClient.Connected != 1)
        //            {
        //                rdpClient.Connect();
        //            }
        //        }
        //        catch (COMException ex)
        //        {
        //            Console.WriteLine($"COMException: 0x{ex.HResult:X} - {ex.Message}");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Connect failed: " + ex.Message);
        //        }
        //    };

        //    // Wrap host in a tab item
        //    var tab = new TabItem { Header = $"{username}@{server}", Content = host };
        //    return tab;
        //}
    }
}
