using Fineo.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;


namespace Fineo.Job.SECDownload
{
    class Program
    {
        SECReportsDownloader downloader = default;
        private CompositionContainer CompositionContainer
        {
            get;
            set;
        }


        private IMessageBus InitMessageBus(string configName)
        {
            IMessageBus result = new Fineo.MessageBus.Azure.MessageBus();

            IMessageBusParams msgBusParams = result.CreateInitParams();
            IConfiguration config = GetConfiguration();

            var section = config.GetSection(configName);
            msgBusParams.Parameters = section.GetChildren()
                    .Select(item => new KeyValuePair<string, string>(item.Key, item.Value))
                    .ToDictionary(x => x.Key, x => x.Value); 

            result.Init(msgBusParams);

            return result;

        }

        private IFileStorage InitFileStorage(string configName)
        {
            IFileStorage result = new Fineo.FileStorage.AzureBlob.FileStorage();

            IFileStorageParams fsParams = result.CreateParams();
            IConfiguration config = GetConfiguration();

            var section = config.GetSection(configName);
            fsParams.Parameters = section.GetChildren()
                    .Select(item => new KeyValuePair<string, string>(item.Key, item.Value))
                    .ToDictionary(x => x.Key, x => x.Value);

            result.Init(fsParams);

            return result;
        }

        private IConfiguration GetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string configFile = string.IsNullOrEmpty(environmentName) ? $"appconfig.json" : $"appconfig.{environmentName}.json";

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(configFile, optional: false, reloadOnChange: true)
                .Build();

            return config;
        }

        public void Start()
        {
            CompositionContainer = new CompositionContainer();
            CompositionContainer.ComposeExportedValue<IMessageBus>("InDownloadFiles", InitMessageBus("MsgBusConfig_InFiles"));
            CompositionContainer.ComposeExportedValue<IMessageBus>("OutFilesDownloaded", InitMessageBus("MsgBusConfig_OutFiles"));
            CompositionContainer.ComposeExportedValue<IFileStorage>("FileStorage", InitFileStorage("FileStorageConfig"));

            if(downloader != null)
            {
                downloader.Stop();
            }

            downloader = new SECReportsDownloader(CompositionContainer);

            downloader.Start();
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Start();
            
        }
    }
}
