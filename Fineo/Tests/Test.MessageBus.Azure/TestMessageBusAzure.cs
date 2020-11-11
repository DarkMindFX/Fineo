using Fineo.Interfaces;
using Newtonsoft.Json;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System;
using Fineo.AzureEmulatorHelper;
using System.Threading.Tasks;

namespace Test.MessageBus.Azure
{
    public class TestMessageBusAzure
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
            AzureStorageEmulator ase = new AzureStorageEmulator();
            ase.Start();

            // letting emulator start
            Task.Delay(5000).Wait();
        }

        
        [Test]
        public void CreateInitParams_Success()
        {
            IMessageBus msgBus = new Fineo.MessageBus.Azure.MessageBus();
            IMessageBusParams msgBusParams = msgBus.CreateInitParams();

            Assert.IsNotNull(msgBusParams);
            Assert.IsNotEmpty(msgBusParams.Parameters);
            Assert.AreEqual(msgBusParams.Parameters.Count, 4);
            Assert.IsTrue(msgBusParams.Parameters.ContainsKey("StorageAccountName"));
            Assert.IsTrue(msgBusParams.Parameters.ContainsKey("StorageAccountKey"));
            Assert.IsTrue(msgBusParams.Parameters.ContainsKey("MessageQueue"));
            Assert.IsTrue(msgBusParams.Parameters.ContainsKey("QueueEndpoint"));
        }

        [Test]
        public void InitMessageBus_Success()
        {
            IConfiguration config = GetConfiguration();
            var cfg = config.GetSection("MsgBusConfig").Get<MsgBusConfig>();

            IMessageBus msgBus = new Fineo.MessageBus.Azure.MessageBus();
            IMessageBusParams msgBusParams = msgBus.CreateInitParams();

            msgBusParams.Parameters["MessageQueue"] = cfg.MessageQueue;
            msgBusParams.Parameters["StorageAccountKey"] = cfg.StorageAccountKey;
            msgBusParams.Parameters["StorageAccountName"] = cfg.StorageAccountName;
            msgBusParams.Parameters["QueueEndpoint"] = cfg.QueueEndpoint;

            msgBus.Init(msgBusParams);

            msgBus.Delete();
        }

        [Test]
        public void InitMessageBus_InvalidAccountName()
        {
            try
            {
                IConfiguration config = GetConfiguration();
                var cfg = config.GetSection("MsgBusConfig_InvalidAccount").Get<MsgBusConfig>();

                IMessageBus msgBus = new Fineo.MessageBus.Azure.MessageBus();
                IMessageBusParams msgBusParams = msgBus.CreateInitParams();

                msgBusParams.Parameters["MessageQueue"] = cfg.MessageQueue;
                msgBusParams.Parameters["StorageAccountKey"] = cfg.StorageAccountKey;
                msgBusParams.Parameters["StorageAccountName"] = cfg.StorageAccountName;

                msgBus.Init(msgBusParams);

                Assert.Fail("MessageBus initialized with invalud account");

            }
            catch (Exception ex)
            {
                Assert.Pass(); // OK - exception is expected
            }
        }

        [Test]
        public void InitMessageBus_InvalidAccountKey()
        {
            try
            {
                IConfiguration config = GetConfiguration();
                var cfg = config.GetSection("MsgBusConfig_InvalidKey").Get<MsgBusConfig>();

                IMessageBus msgBus = new Fineo.MessageBus.Azure.MessageBus();
                IMessageBusParams msgBusParams = msgBus.CreateInitParams();

                msgBusParams.Parameters["MessageQueue"] = cfg.MessageQueue;
                msgBusParams.Parameters["StorageAccountKey"] = cfg.StorageAccountKey;
                msgBusParams.Parameters["StorageAccountName"] = cfg.StorageAccountName;

                msgBus.Init(msgBusParams);

                Assert.Fail("MessageBus initialized with invalud key");

            }
            catch (Exception ex)
            {
                Assert.Pass(); // OK - exception is expected
            }
        }

        [Test]
        public void MessageBus_SendMessage_Success()
        {
            string test_queue = $"test-queue-{Guid.NewGuid().ToString().ToLower().Replace("{", "").Replace("}", "")}";
            IConfiguration config = GetConfiguration();
            var cfg = config.GetSection("MsgBusConfig").Get<MsgBusConfig>();

            IMessageBus msgBus = new Fineo.MessageBus.Azure.MessageBus();
            IMessageBusParams msgBusParams = msgBus.CreateInitParams();

            msgBusParams.Parameters["MessageQueue"] = test_queue;
            msgBusParams.Parameters["StorageAccountKey"] = cfg.StorageAccountKey;
            msgBusParams.Parameters["StorageAccountName"] = cfg.StorageAccountName;
            msgBusParams.Parameters["QueueEndpoint"] = cfg.QueueEndpoint;

            msgBus.Init(msgBusParams);

            var section = config.GetSection("TestMessage");
            MessageBusDTO busDto = config.GetSection("TestMessage").Get<MessageBusDTO>();
            busDto.Body = Guid.NewGuid().ToString();

            msgBus.Send(busDto);

            MessageBusDTO receivedDto = msgBus.ReadNext();

            Assert.IsTrue(busDto.Equals(receivedDto));

            msgBus.Delete();
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