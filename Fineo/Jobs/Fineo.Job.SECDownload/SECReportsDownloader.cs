using Fineo.DTOs.MessagesBus;
using Fineo.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fineo.Job.SECDownload
{
    class SECReportsDownloader
    {
        private IFileStorage fileStorage = default;
        private IMessageBus msbInFiles = default;
        private IMessageBus msbOutNotification = default;
        private bool isRunning = false;
        private Thread listenThread = null;

        public SECReportsDownloader(CompositionContainer compContainer)
        {
            fileStorage = compContainer.GetExportedValue<IFileStorage>("FileStorage");
            msbInFiles = compContainer.GetExportedValue<IMessageBus>("InDownloadFiles");
            msbOutNotification = compContainer.GetExportedValue<IMessageBus>("OutFilesDownloaded");
        }

        public void Start()
        {
            if(isRunning)
            {
                Stop();
            }
            isRunning = true;

            listenThread = new Thread(ListenInQueue);
            listenThread.Start();

        }

        public void Stop()
        {
            isRunning = false;
            listenThread.Abort();
            listenThread = null;
        }

        private void ListenInQueue()
        {
            try
            {
                while (isRunning)
                {
                    var msgBusDto = msbInFiles.ReadNext();
                    if(msgBusDto != null && !string.IsNullOrEmpty(msgBusDto.Body))
                    {
                        DownloadFiling msgDownload = JsonConvert.DeserializeObject<DownloadFiling>(msgBusDto.Body);
                        if(msgDownload != null)
                        {
                            Task.Run(() => DownloadSECFiling(msgDownload));
                        }
                    }
                }
            }
            catch(ThreadAbortException exTA)
            {
                // Thread was aborted
            }
        }

        private void DownloadSECFiling(DownloadFiling msgDownload)
        {

        }
    }
}
