using Fineo.Constants;
using Newtonsoft.Json;
using System;

namespace Fineo.Common
{
    public class Error
    {
        public Error(EErrorType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        public Error(Exception ex)
        {
            this.Type = EErrorType.Error;
            this.Message = ex.Message;
        }

        [JsonProperty(PropertyName = "type")]
        public EErrorType Type
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "message")]
        public string Message
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"Type: {this.Type}\r\nMessage: {this.Message}";
        }
    }
}
