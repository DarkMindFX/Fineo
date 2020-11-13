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

        public SECReportsDownloader(CompositionContainer compContainer)
        {
            fileStorage = compContainer.GetExportedValue<IFileStorage>();
        }
    }
}
