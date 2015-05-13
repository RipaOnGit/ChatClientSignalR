using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]
namespace SignalRChat
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            CreateTableIFNotExist();
            app.MapSignalR();
        }
        private void CreateTableIFNotExist()
        {
            string accountName = "ripasstorage1";
            string accountKey = "0M78LT+YVu7/omJULHgjTiRvbhQVOoadnI4kpl5ykDESqY1Z/PW5uqEj4pLLX2k1u7k0zQAsFO3PCJduZwcszw==";
            try
            {
                StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

                CloudTableClient client = account.CreateCloudTableClient();

                CloudTable table = client.GetTableReference("chatMessages");

                if (!table.Exists(null, null))
                    table.CreateIfNotExists(); ;
            }
            catch (Exception ex)
            {
            }
        }

    }
}
