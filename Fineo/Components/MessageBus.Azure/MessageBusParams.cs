using Fineo.Interfaces;
using System.Collections.Generic;

namespace Fineo.MessageBus.Azure
{
    public class MessageBusParams : IMessageBusParams
    {
        public MessageBusParams()
        {
            Parameters = new Dictionary<string, string>();
            Parameters["StorageAccountName"] = null;
            Parameters["StorageAccountKey"] = null;
            Parameters["MessageQueue"] = null;
            Parameters["QueueEndpoint"] = null;
        }

        public Dictionary<string, string> Parameters 
        {
            get;
            set;
        }
    }
}
