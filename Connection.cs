using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO;
using Newtonsoft.Json;

namespace Portals
{
    public class Connection
    {
        public Connection(string name, string password, string address, string domain) 
        {
            Name = name; 
            Password = password;        
            Address = address; 
            Domain = domain; 
        }

        public Connection() { }

        public string Name { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Domain { get; set; }
    }

    public class ConnectionStorage
    {
        private string _fileName;
        public ObservableCollection<Connection> Connections { get; private set; } = new ObservableCollection<Connection>();

        public ConnectionStorage(string fileName)
        { 
            _fileName = fileName;
            Load();
        }

        public void Add(Connection connection)
        {
            Connections.Add(connection);
        }

        public void Save()
        {
            File.WriteAllText(_fileName, JsonConvert.SerializeObject(Connections, Formatting.Indented));
        }

        public void Load() 
        {
            if (File.Exists(_fileName))
            {
                var loaded = JsonConvert.DeserializeObject<ObservableCollection<Connection>>(File.ReadAllText(_fileName));
                Connections = loaded ?? new ObservableCollection<Connection>();
            }
        }
    }

}
