using AxMSTSCLib;
using MSTSCLib;
using Portals.Views;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;

namespace Portals
{
    public partial class MainWindow : Window
    {
        //private TabItem _lastSelectedTab = null;
        private bool _isDarkMode = false;

        private readonly HomePage _homePage = new HomePage();
        private readonly WorkspacePage _workspacePage = new WorkspacePage();

        public MainWindow()
        {
            InitializeComponent();
            AddNewWorkspaceTab("New Connection");
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

        private void WorkspaceTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkspaceTabs.SelectedItem == AddTabItem)
            {
                AddNewWorkspaceTab($"New Connection");
            }
            else if (WorkspaceTabs.SelectedItem is TabItem tab && tab.Tag is UserControl content)
            {
                MainContent.Content = content;
            }
        }

        private void AddNewWorkspaceTab(string header)
        {
            var homePage = new Portals.Views.HomePage(); // default page

            var tab = new TabItem
            {
                Tag = homePage // store page
            };

            // Create header with text + close button
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0) };

            var title = new TextBlock
            {
                Text = header,
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var closeButton = new Button
            {
                Content = "✕",
                Width = 18,
                Height = 18,
                Padding = new Thickness(0),
                Margin = new Thickness(0, 0, 0, 0),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };
            closeButton.Click += (s, e) => CloseTab(tab);

            headerPanel.Children.Add(title);
            headerPanel.Children.Add(closeButton);

            tab.Header = headerPanel;
            tab.Content = null; // real content shown in MainContent
            WorkspaceTabs.Items.Insert(WorkspaceTabs.Items.Count - 1, tab);
            WorkspaceTabs.SelectedItem = tab;
            MainContent.Content = homePage;
        }

        private void CloseTab(TabItem tab)
        {
            // If you’re closing the active tab, switch content
            bool isActive = (WorkspaceTabs.SelectedItem == tab);

            WorkspaceTabs.Items.Remove(tab);

            if (isActive)
            {
                if (WorkspaceTabs.Items.Count > 1) // at least one tab + "+"
                {
                    WorkspaceTabs.SelectedIndex = 0;
                    if (WorkspaceTabs.SelectedItem is TabItem selected && selected.Tag is UserControl content)
                    {
                        MainContent.Content = content;
                    }
                }
                else
                {
                    MainContent.Content = null; // no open tabs
                }
            }
        }

        private void OnThemeToggle(object sender, RoutedEventArgs e)
        {
            _isDarkMode = !_isDarkMode;
            ThemeManager.ApplyTheme(_isDarkMode ? "Dark" : "Light");
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
