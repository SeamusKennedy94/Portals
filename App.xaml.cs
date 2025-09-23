using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Portals
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load saved Theme preference or fallback to Light
            string savedTheme = LoadSavedTheme() ?? "Light";

            ThemeManager.ApplyTheme(savedTheme);
        }

        private string LoadSavedTheme()
        {
            return Application.Current.Properties.Contains("UserTheme") ? Application.Current.Properties["UserTheme"].ToString() : "Light";
        }
    }

    public static class ThemeManager
    {
        public static void ApplyTheme(string themeName)
        {
            var dict = new ResourceDictionary();

            switch(themeName)
            {
                case "Dark":
                        dict.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
                    break;
                case "Light":
                        dict.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
                    break;
            }

            //Clear existing and apply
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
