using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fineo.Interfaces
{
    public class MessageBusDTO
    {
        [JsonProperty(PropertyName = "sender_id")]
        public string SenderID { get; set; }

        [JsonProperty(PropertyName = "message_id")]
        public string MessageID { get; set; }

        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }

        public override bool Equals(object obj)
        {
            MessageBusDTO other = obj as MessageBusDTO;
            if(other != null)
            {
                return other.SenderID.Equals(this.SenderID) &&
                        other.MessageID.Equals(this.MessageID) &&
                        other.Body.Equals(this.Body);
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
}
