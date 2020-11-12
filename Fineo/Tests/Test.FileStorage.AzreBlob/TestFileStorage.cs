using Fineo.AzureEmulatorHelper;
using Fineo.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Test.FileStorage.AzureBlob
{
    public class TestFileStorage
    {
        class FileStorageConfig
        {
            [JsonProperty("StorageAccountName")]
            public string StorageAccountName { get; set; }

            [JsonProperty("StorageAccountKey")]
            public string StorageAccountKey { get; set; }

            [JsonProperty("ContainerName")]
            public string ContainerName { get; set; }

            [JsonProperty("BlobEndpoint")]
            public string BlobEndpoint { get; set; }

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
        public void InitFileStorage_Success()
        {
            IConfiguration config = GetConfiguration();
            var cfg = config.GetSection("FileStorageConfig").Get<FileStorageConfig>();

            IFileStorage fileStorage = new Fineo.FileStorage.AzureBlob.FileStorage();
            IFileStorageParams fileStorageParams = fileStorage.CreateParams();

            fileStorageParams.Parameters["ContainerName"] = cfg.ContainerName;
            fileStorageParams.Parameters["StorageAccountKey"] = cfg.StorageAccountKey;
            fileStorageParams.Parameters["StorageAccountName"] = cfg.StorageAccountName;
            fileStorageParams.Parameters["BlobEndpoint"] = cfg.BlobEndpoint;

            fileStorage.Init(fileStorageParams);

            
        }

        [Test]
        public void InitFileStorage_InvalidAccountName()
        {
            try
            {
                IConfiguration config = GetConfiguration();
                var cfg = config.GetSection("FileStorageConfig_InvalidAccount").Get<FileStorageConfig>();

                IFileStorage fileStorage = new Fineo.FileStorage.AzureBlob.FileStorage();
                IFileStorageParams fileStorageParams = fileStorage.CreateParams();

                fileStorageParams.Parameters["ContainerName"] = cfg.ContainerName;
                fileStorageParams.Parameters["StorageAccountKey"] = cfg.StorageAccountKey;
                fileStorageParams.Parameters["StorageAccountName"] = cfg.StorageAccountName;

                fileStorage.Init(fileStorageParams);

                Assert.Fail("FileStorage initialized with invalud account");

            }
            catch (Exception ex)
            {
                Assert.Pass(); // OK - exception is expected
            }
        }

        [Test]
        public void InitFileStorage_InvalidAccountKey()
        {
            try
            {
                IConfiguration config = GetConfiguration();
                var cfg = config.GetSection("FileStorageConfig_InvalidKey").Get<FileStorageConfig>();

                IFileStorage fileStorage = new Fineo.FileStorage.AzureBlob.FileStorage();
                IFileStorageParams fileStorageParams = fileStorage.CreateParams();

                fileStorageParams.Parameters["ContainerName"] = cfg.ContainerName;
                fileStorageParams.Parameters["StorageAccountKey"] = cfg.StorageAccountKey;
                fileStorageParams.Parameters["StorageAccountName"] = cfg.StorageAccountName;

                fileStorage.Init(fileStorageParams);

                Assert.Fail("FileStorage initialized with invalud key");

            }
            catch (Exception ex)
            {
                Assert.Pass(); // OK - exception is expected
            }
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
