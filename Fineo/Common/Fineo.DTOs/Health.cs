using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fineo.DTOs
{
    public class HealthRequest : RequestBase
    {
    }

    public class HealthResponse : ResponseBase
    {
        public HealthResponse()
        {
            Data = new Dictionary<string, string>();
        }

        [JsonProperty( PropertyName = "health_data" )]
        public Dictionary<string, string> Data
        {
            get;
            set;
        }
    }
}
