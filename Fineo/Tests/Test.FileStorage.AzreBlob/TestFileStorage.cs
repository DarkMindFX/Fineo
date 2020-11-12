using Fineo.AzureEmulatorHelper;
using Fineo.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [OneTimeSetUp]
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

        [Test]
        public void UploadFile_Success()
        {
            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var fi = new Fineo.Interfaces.FileInfo();
            fi.Path = "test.csv";
            fi.Content = GetTestFileContent("TestFiles\\test.csv");

            var t = fs.UploadAsync(fi);

            Assert.IsTrue(t.Result, "Failed to upload file - result FALSE");
        }

        [Test]
        public void UploadFile_WithFolder_Success()
        {
            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var fi = new Fineo.Interfaces.FileInfo();
            fi.Path = "TestSubFolder\\test.csv";
            fi.Content = GetTestFileContent("TestFiles\\test.csv");

            var t = fs.UploadAsync(fi);

            Assert.IsTrue(t.Result, "Failed to upload file - result FALSE");
        }

        [Test]
        public void DownloadFile_Success()
        {
            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var fi = new Fineo.Interfaces.FileInfo();
            fi.Path = "test.csv";
            fi.Content = GetTestFileContent("TestFiles\\test.csv");

            var t = fs.UploadAsync(fi);

            Assert.IsTrue(t.Result, "Failed to upload file - result FALSE");

            var content = fs.DownloadAsync(fi);

            content.Wait();

            Assert.IsTrue( fi.Content.Length == content.Result.Content.Length, "Contents' lengths are not equal" );
            for(int i = 0; i < fi.Content.Length; ++i)
            {
                Assert.IsTrue(fi.Content[i] == content.Result.Content[i], "Downloaded content is not equal to original");
            }
        }

        [Test]
        public void DownloadFile_WithFolder_Success()
        {
            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var fi = new Fineo.Interfaces.FileInfo();
            fi.Path = "TestSubFolder\\test.csv";
            fi.Content = GetTestFileContent("TestFiles\\test.csv");

            var t = fs.UploadAsync(fi);

            Assert.IsTrue(t.Result, "Failed to upload file - result FALSE");

            var content = fs.DownloadAsync(fi);

            content.Wait();

            Assert.IsTrue(fi.Content.Length == content.Result.Content.Length, "Contents' lengths are not equal");
            for (int i = 0; i < fi.Content.Length; ++i)
            {
                Assert.IsTrue(fi.Content[i] == content.Result.Content[i], "Downloaded content is not equal to original");
            }
        }

        [Test]
        public void DeleteFile_Success()
        {
            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var fi = new Fineo.Interfaces.FileInfo();
            fi.Path = "test.csv";
            fi.Content = GetTestFileContent("TestFiles\\test.csv");

            var t = fs.UploadAsync(fi);

            Assert.IsTrue(t.Result, "Failed to upload file - result FALSE");

            t = fs.DeleteAsync(fi);

            Assert.IsTrue(t.Result, "Failed to delete the file");
        }

        [Test]
        public void DeleteFile_WithFolder_Success()
        {
            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var fi = new Fineo.Interfaces.FileInfo();
            fi.Path = "TestSubFolder\\test.csv";
            fi.Content = GetTestFileContent("TestFiles\\test.csv");

            var t = fs.UploadAsync(fi);

            Assert.IsTrue(t.Result, "Failed to upload file - result FALSE");

            t = fs.DeleteAsync(fi);

            Assert.IsTrue(t.Result, "Failed to delete the file");
        }

        [Test]
        public void DownloadFile_FileNotExist()
        {
            try
            {
                string fileName = "file-" + Guid.NewGuid().ToString() + ".dat";

                IFileStorage fs = PrepaFileStorage("FileStorageConfig");

                var fi = new Fineo.Interfaces.FileInfo();
                fi.Path = fileName;

                var content = fs.DownloadAsync(fi);

                var fiResult = content.Result;

                Assert.Fail("Unexpected content was read");
            }
            catch(Exception ex)
            {
                Assert.IsTrue(ex.InnerException != null);
                Assert.IsTrue(ex.InnerException.GetType() == typeof(StorageException));
            }

        }

        [Test]
        public void DeleteFile_FileNotExist()
        {
            string fileName = "file-" + Guid.NewGuid().ToString() + ".dat";
            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var fi = new Fineo.Interfaces.FileInfo();
            fi.Path = fileName;

            var t = fs.DeleteAsync(fi);

            Assert.IsFalse(t.Result);
        }

        [Test]
        public void GetFileInfo_Success()
        {
            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var fi = new Fineo.Interfaces.FileInfo();
            fi.Path = "test.csv";
            fi.Content = GetTestFileContent("TestFiles\\test.csv");

            var t = fs.UploadAsync(fi);

            Assert.IsTrue(t.Result, "Failed to upload file - result FALSE");

            var fileInfo = fs.GetFileInfo(fi);

            Assert.AreEqual(fi.Path, fileInfo.Result.Path);
            Assert.AreEqual(fi.Content.Length, fileInfo.Result.Size);
            Assert.IsTrue(fi.LastModified <= DateTime.Now);
        }

        [Test]
        public void GetFolderContent_Success()
        {
            string[] fileNames = { "test1.csv", "test2.csv", "test3.csv" };

            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var fi = new Fineo.Interfaces.FileInfo();
            fi.Content = GetTestFileContent("TestFiles\\test.csv");

            foreach(var f in fileNames)
            {
                fi.Path = "TestSubFolder\\" + f;
                var t = fs.UploadAsync(fi);
            }

            var folder = new Fineo.Interfaces.FileInfo();
            folder.Path = "TestSubFolder";
            var content = fs.GetFolderContent(folder);

            var listOfFiles = content.Result.Select(x => fileNames.Contains(x.Path));

            Assert.IsTrue(listOfFiles.Count() >= 3);
        }

        [Test]
        public void GetFolderContent_InvalidFolder()
        {
            IFileStorage fs = PrepaFileStorage("FileStorageConfig");

            var folder = new Fineo.Interfaces.FileInfo();
            folder.Path = "TestSubFolder-" + Guid.NewGuid().ToString();
            var content = fs.GetFolderContent(folder);

            Assert.IsNotNull(content.Result);
            Assert.IsEmpty(content.Result);
        }

        #region Support methods

        private byte[] GetTestFileContent(string path)
        {
            byte[] result = File.ReadAllBytes(path);           

            return result;

        }

        private IFileStorage PrepaFileStorage(string configName)
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

            return fileStorage;
        }

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
