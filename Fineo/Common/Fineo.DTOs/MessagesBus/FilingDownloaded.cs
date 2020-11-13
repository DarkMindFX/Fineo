using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fineo.DTOs.MessagesBus
{
    public class FilingDownloaded
    {
        public FilingDownloaded()
        {
            Docs = new List<string>();
        }

        [JsonProperty(PropertyName = "regulator_code")]
        public string RegulatorCode
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "company_code")]
        public string CompanyCode
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "filing")]
        public string Filing
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "docs")]
        public List<string> Docs
        {
            get;
            set;
        }
    }
}
