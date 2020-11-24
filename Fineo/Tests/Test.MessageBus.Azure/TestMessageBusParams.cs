using Fineo.MessageBus.Azure;
using NUnit.Framework;

namespace Test.MessageBus.Azure
{
    public class TestMessageBusParams
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParametersList_Success()
        {
            MessageBusParams busParams = new MessageBusParams();

            Assert.IsNotEmpty(busParams.Parameters);
            Assert.AreEqual(busParams.Parameters.Count, 4);
            Assert.IsTrue(busParams.Parameters.ContainsKey("StorageAccountName"));
            Assert.IsTrue(busParams.Parameters.ContainsKey("StorageAccountKey"));
            Assert.IsTrue(busParams.Parameters.ContainsKey("MessageQueue"));
            Assert.IsTrue(busParams.Parameters.ContainsKey("QueueEndpoint"));
        }
    }
}
