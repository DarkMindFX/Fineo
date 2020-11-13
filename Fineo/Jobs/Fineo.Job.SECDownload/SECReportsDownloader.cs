using Fineo.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Text;

namespace Fineo.Job.SECDownload
{
    class SECReportsDownloader
    {
        private IFileStorage fileStorage = default;
        private IMessageBus msbInFiles = default;
        private IMessageBus msbOutNotification = default;
        private bool isRunning = false;

        public SECReportsDownloader(CompositionContainer compContainer)
        {
            fileStorage = compContainer.GetExportedValue<IFileStorage>("FileStorage");
            msbInFiles = compContainer.GetExportedValue<IMessageBus>("InDownloadFiles");
            msbOutNotification = compContainer.GetExportedValue<IMessageBus>("OutFilesDownloaded");
        }

        public void Start()
        {
            isRunning = true;

        }

        public void Stop()
        {

        }

        private void ListenInQueue()
        {
            while(isRunning)
            {
                var msgBusDto = msbInFiles.ReadNext();
            }
        }
    }
}
