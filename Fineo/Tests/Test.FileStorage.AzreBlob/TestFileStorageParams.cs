using Fineo.FileStorage.AzureBlob;
using NUnit.Framework;

namespace Test.FileStorage.AzreBlob
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParametersList_Success()
        {
            FileStorageParams busParams = new FileStorageParams();

            Assert.IsNotEmpty(busParams.Parameters);
            Assert.AreEqual(busParams.Parameters.Count, 4);
            Assert.IsTrue(busParams.Parameters.ContainsKey("StorageAccountName"));
            Assert.IsTrue(busParams.Parameters.ContainsKey("StorageAccountKey"));
            Assert.IsTrue(busParams.Parameters.ContainsKey("ContainerName"));
            Assert.IsTrue(busParams.Parameters.ContainsKey("BlobEndpoint"));
        }
    }
}