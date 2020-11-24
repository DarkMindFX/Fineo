using System.Collections.Generic;

namespace Fineo.Interfaces
{
    public interface IMessageBusParams
    {
        Dictionary<string, string> Parameters
        {
            get;
            set;
        }
    }

    

    public interface IMessageBus
    {
        void Init(IMessageBusParams initParams);

        string ID { get; set; }

        MessageBusDto ReadNext();

        void Send(MessageBusDto msg);

        bool Delete();

        IMessageBusParams CreateInitParams();

    }
}
