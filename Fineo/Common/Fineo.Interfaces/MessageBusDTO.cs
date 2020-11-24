using Newtonsoft.Json;

namespace Fineo.Interfaces
{
    public class MessageBusDTO
    {
        [JsonProperty("sender_id")]
        public string SenderID { get; set; }

        [JsonProperty("receiver_id")]
        public string ReceiverID { get; set; }

        [JsonProperty("message_id")]
        public string MessageID { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        public override bool Equals(object obj)
        {
            MessageBusDTO other = obj as MessageBusDTO;
            if(other != null)
            {
                return  (other.SenderID != null ? other.SenderID.Equals(SenderID) : SenderID == null) &&
                        (other.ReceiverID != null ? other.ReceiverID.Equals(ReceiverID) : ReceiverID == null) &&
                        (other.MessageID != null ? other.MessageID.Equals(MessageID) : MessageID == null) &&
                        (other.Body != null ? other.Body.Equals(Body) : Body == null);
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }
}
