using Fineo.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Fineo.DTOs
{
    public abstract class RequestBase
    {
        [JsonProperty(PropertyName = "sent_datetime")]
        DateTime SentDt
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "sender_id")]
        string SenderId
        {
            get;
            set;
        }
    }

    public abstract class ResponseBase
    {
        protected ResponseBase()
        {
            Errors = new List<Error>();
        }

        [JsonProperty(PropertyName = "sent_datetime")]
        public DateTime SentDt
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "errors")]
        public List<Error> Errors
        {
            get;
            set;
        }
    }
}
