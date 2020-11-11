using Fineo.AzureEmulatorHelper;
using NUnit.Framework;
using System.Collections.Generic;

namespace Test.AzureEmulatorHelper
{
    public class TestAzureEmulatorHelper
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetStatus_Success()
        {
            AzureStorageEmulator ase = new AzureStorageEmulator();
            // starting
            var tStart = ase.StartAsync();
            tStart.Wait();
            bool isStarted = tStart.Result;

            // getting status
            var tStatus = ase.StatusAsync();
            tStatus.Wait();
            Dictionary<string, string> s = tStatus.Result;

            // stopping
            var tStop = ase.StopAsync();
            tStop.Wait();
            bool isStopped = tStop.Result;


        }
    }
}