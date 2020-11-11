using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Test.FileStorage.AzureBlob
{
    public class TestFileStorage
    {
        class MsgBusConfig
        {
            [JsonProperty("StorageAccountName")]
            public string StorageAccountName { get; set; }

            [JsonProperty("StorageAccountKey")]
            public string StorageAccountKey { get; set; }

            [JsonProperty("MessageQueue")]
            public string MessageQueue { get; set; }

            [JsonProperty("QueueEndpoint")]
            public string QueueEndpoint { get; set; }

        }

        [SetUp]
        public void Setup()
        {
        }

        #region Support methods
        private IConfiguration GetConfiguration()
        {
            var codebase = Assembly.GetExecutingAssembly().GetName().CodeBase;

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json", optional: false, reloadOnChange: true)
                .Build();

            return config;
        }
        #endregion
    }
}
