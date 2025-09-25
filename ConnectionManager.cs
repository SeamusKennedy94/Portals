using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portals
{
    public class ConnectionManager
    {
        private static readonly Lazy<ConnectionManager> _instance = new Lazy<ConnectionManager>(() => new ConnectionManager());
        public static ConnectionManager Instance => _instance.Value;

        public ConnectionStorage RecentConnections { get; private set; } = new ConnectionStorage("recent.json");
        public ConnectionStorage FavoriteConnections { get; private set; } = new ConnectionStorage("favorite.json");

        private ConnectionManager() { }
    }
}
