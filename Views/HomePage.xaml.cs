using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO;
using Newtonsoft.Json;


namespace Portals.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        public ObservableCollection<Connection> RecentConnections { get; set; } = new ObservableCollection<Connection>();

        public ObservableCollection<Connection> FavoriteConnections { get; set; } = new ObservableCollection<Connection>();

        private readonly string recentPath = "recent.json";
        private readonly string favoritePath = "favorite.json";

        public HomePage()
        {
            InitializeComponent();

            RecentConnectionsList.ItemsSource = RecentConnections;
            FavoriteConnectionsList.ItemsSource = FavoriteConnections;

            // Load Persistant data here
            LoadConnections();
        }

        private void AddConnection(Connection connection)
        {
            this.RecentConnections.Add(connection);
        }

        private void SaveConnections()
        {
            File.WriteAllText(recentPath, JsonConvert.SerializeObject(RecentConnections));
            File.WriteAllText(favoritePath, JsonConvert.SerializeObject(FavoriteConnections));
        }

        private void LoadConnections()
        {
            if (File.Exists(recentPath))
            {
                var recent = JsonConvert.DeserializeObject<ObservableCollection<Connection>>(File.ReadAllText(recentPath));
                if (recent != null) RecentConnections = recent;
            }

            if (File.Exists(favoritePath))
            {
                var favs = JsonConvert.DeserializeObject<ObservableCollection<Connection>>(File.ReadAllText(favoritePath));
                if (favs != null) FavoriteConnections = favs;
            }
        }
    }
}
