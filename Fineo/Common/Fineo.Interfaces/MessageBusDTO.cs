using Newtonsoft.Json;

namespace Fineo.Interfaces
{
    public class MessageBusDto
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
            MessageBusDto other = obj as MessageBusDto;
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
            unchecked
            {
                int result = (SenderID != null ? SenderID.GetHashCode() : 0);
                result = (result * 397) ^ (ReceiverID != null ? ReceiverID.GetHashCode() : 0);
                result = (result * 397) ^ (MessageID != null ? MessageID.GetHashCode() : 0);
                result = (result * 397) ^ (Body != null ? Body.GetHashCode() : 0);
                return result;
            }
        }


    }
}
