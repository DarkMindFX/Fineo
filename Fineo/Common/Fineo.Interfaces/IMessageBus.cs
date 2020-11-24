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

        MessageBusDTO ReadNext();

        void Send(MessageBusDTO msg);

        bool Delete();

        IMessageBusParams CreateInitParams();

    }
}
