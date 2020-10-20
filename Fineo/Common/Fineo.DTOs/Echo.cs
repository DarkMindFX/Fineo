using Newtonsoft.Json;
using System;

namespace Fineo.DTOs
{
    public class EchoRequest : RequestBase
    {
        [JsonProperty(PropertyName = "message")]
        public string Message
        {
            get;
            set;
        }
    }

    public class EchoResponse : ResponseBase
    {
        [JsonProperty(PropertyName = "message")]
        public string Message
        {
            get;
            set;
        }
    }
}
