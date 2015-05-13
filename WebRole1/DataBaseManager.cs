using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;

namespace SignalRChat
{
    public class DataBaseManager : IDataBaseManager
    {
        private readonly string accountName = "ripasstorage1";
        private readonly string accountKey = "0M78LT+YVu7/omJULHgjTiRvbhQVOoadnI4kpl5ykDESqY1Z/PW5uqEj4pLLX2k1u7k0zQAsFO3PCJduZwcszw==";

        public void TableInsert(IMessageData dbMessage)
        {
            try
            {
                StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

                CloudTableClient client = account.CreateCloudTableClient();

                CloudTable table = client.GetTableReference("chatMessages");

                //Create table if not exisiting.
                if (!table.Exists(null, null))
                    table.CreateIfNotExists();

                TableOperation tableOperation = TableOperation.Insert(dbMessage);
                TableResult result = table.Execute(tableOperation);
            }
            catch (Exception ex)
            {
            }
        }
    }

    internal class MessageData : TableEntity, IMessageData
    {
        public string message { get; set; }
        public string user { get; set; }
    }

    public interface IMessageData : ITableEntity
    {
        string message { get; set; }
        string user { get; set; }
    }

    public interface IDataBaseManager
    {
        void TableInsert(IMessageData dbMessage);
    }
}
