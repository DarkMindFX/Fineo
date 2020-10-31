using Fineo.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fineo.MessageBus.Azure
{
    public class MessageBus : IMessageBus
    {
        IMessageBusParams msgBusParams;

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
            msgBusParams = initParams;

            account = createCloudStorageAccount(initParams);
            client = account.CreateCloudQueueClient();
            queue = client.GetQueueReference((string)initParams.Parameters["MessageQueue"]);
            queue.CreateIfNotExistsAsync();
        }

        public MessageBusDTO ReadNext()
        {
            MessageBusDTO result = null;
            CloudQueueMessage newMessage = queue.GetMessageAsync().Result;
            if (newMessage != null)
            {
                result = JsonConvert.DeserializeObject<MessageBusDTO>(newMessage.AsString);

                if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(result.ReceiverID) || ID.Equals(result.ReceiverID))
                {
                    queue.DeleteMessageAsync(newMessage);
                }
            }

            return result;
        }

        public void Send(MessageBusDTO msgDto)
        {
            CloudQueueMessage msg = new CloudQueueMessage(JsonConvert.SerializeObject(msgDto));
            queue.AddMessageAsync(msg).Wait();
        }

        public bool Delete()
        {
            bool deleted = queue.DeleteIfExistsAsync().Result;
            return deleted;
        }

        #region Support method
        CloudStorageAccount createCloudStorageAccount(IMessageBusParams ctxParams)
        {
            String accountName = (string)ctxParams.Parameters["StorageAccountName"];
            String accountKey = (string)ctxParams.Parameters["StorageAccountKey"];
            String queueEndpoint = (string)ctxParams.Parameters["QueueEndpoint"];
            String connString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};", accountName, accountKey) 
                                + (!string.IsNullOrEmpty(queueEndpoint) ? "QueueEndpoint=" + queueEndpoint : string.Empty);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);

            return storageAccount;
        }
        #endregion
    }
}
