using Fineo.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;

namespace Fineo.MessageBus.Azure
{
    public class MessageBus : IMessageBus
    {

        CloudStorageAccount account;
        CloudQueueClient client;
        CloudQueue queue;

        public string ID 
        { 
            get; set; 
        }

        public IMessageBusParams CreateInitParams()
        {
            return new MessageBusParams();
        }

        public void Init(IMessageBusParams initParams)
        {
            account = createCloudStorageAccount(initParams);
            client = account.CreateCloudQueueClient();
            queue = client.GetQueueReference(initParams.Parameters["MessageQueue"]);
            queue.CreateIfNotExistsAsync();
        }

        public MessageBusDto ReadNext()
        {
            MessageBusDto result = null;
            CloudQueueMessage newMessage = queue.GetMessageAsync().Result;
            if (newMessage != null)
            {
                result = JsonConvert.DeserializeObject<MessageBusDto>(newMessage.AsString);

                if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(result.ReceiverID) || ID.Equals(result.ReceiverID))
                {
                    queue.DeleteMessageAsync(newMessage);
                }
            }

            return result;
        }

        public void Send(MessageBusDto msg)
        {
            CloudQueueMessage queueMsg = new CloudQueueMessage(JsonConvert.SerializeObject(msg));
            queue.AddMessageAsync(queueMsg).Wait();
        }

        public bool Delete()
        {
            bool deleted = queue.DeleteIfExistsAsync().Result;
            return deleted;
        }

        #region Support method
        CloudStorageAccount createCloudStorageAccount(IMessageBusParams ctxParams)
        {
            string accountName = ctxParams.Parameters["StorageAccountName"];
            string accountKey = ctxParams.Parameters["StorageAccountKey"];
            string queueEndpoint = ctxParams.Parameters["QueueEndpoint"];
            string connString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};", accountName, accountKey) 
                                + (!string.IsNullOrEmpty(queueEndpoint) ? "QueueEndpoint=" + queueEndpoint : string.Empty);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);

            return storageAccount;
        }
        #endregion
    }
}
