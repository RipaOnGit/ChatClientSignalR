using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;

namespace SignalRChat
{
    //Instructions: https://www.macaw.nl/weblog/2013/8/setting-up-website-deployment-to-windows-azure
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        private IMessageData _dbMessage;
        private IDataBaseManager _dbManager;

        public ChatHub(IDataBaseManager dbmanager)
        {
            _dbManager = dbmanager;
        }

        public void Send(string name, string message)
        {
            //Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);

            //Create message for store.
            _dbMessage = new MessageData()
            {
                message = message,
                user = name,
                PartitionKey = "Messages",
                RowKey = Guid.NewGuid().ToString()
            };

            //Then record message to table storage.
            _dbManager.TableInsert(_dbMessage);
        }

        public void addUser(string User)
        {
            _connections.Add(Context.ConnectionId, User);
            Clients.All.getConnectedUsers(_connections.GetAllConnections());
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var id = _connections.GetConnections(Context.ConnectionId).FirstOrDefault();
            _connections.Remove(Context.ConnectionId, id);
            Clients.All.getConnectedUsers(_connections.GetAllConnections());

            return base.OnDisconnected(stopCalled);
        }
    }

    /// <summary>
    /// Connection book keeping and management module, which maintains user connection mapping.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public List<string> GetAllConnections()
        {
            List<string> outs = new List<string>();
            foreach (var con in _connections)
            {
                outs.Add(con.Value.ToList()[0]);
            }
            return outs;
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}