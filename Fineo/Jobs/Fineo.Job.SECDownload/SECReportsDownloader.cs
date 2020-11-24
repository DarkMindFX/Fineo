using Fineo.DTOs.MessagesBus;
using Fineo.Interfaces;
using Newtonsoft.Json;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Fineo.Job.SECDownload
{
    class SECReportsDownloader
    {
        private readonly IFileStorage fileStorage = default;
        private readonly IMessageBus msbInFiles = default;
        private readonly IMessageBus msbOutNotification = default;
        private bool isRunning = false;
        private Thread listenThread = null;

        public SECReportsDownloader(CompositionContainer compContainer)
        {
            fileStorage = compContainer.GetExportedValue<IFileStorage>("FileStorage");
            msbInFiles = compContainer.GetExportedValue<IMessageBus>("InDownloadFiles");
            msbInFiles.ID = "SEC"; // 
            msbOutNotification = compContainer.GetExportedValue<IMessageBus>("OutFilesDownloaded");
        }

        public void Start()
        {
            if (isRunning)
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
                    if (msgBusDto != null && !string.IsNullOrEmpty(msgBusDto.Body))
                    {
                        DownloadFiling msgDownload = JsonConvert.DeserializeObject<DownloadFiling>(msgBusDto.Body);
                        if (msgDownload != null && msgDownload.RegulatorCode == "SEC")
                        {
                            Task.Run(() => DownloadSECFiling(msgDownload));
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // Thread was aborted
            }
        }

        private void DownloadSECFiling(DownloadFiling msgDownload)
        {
            var secApi = new Fineo.SEC.Api.SECApi();
            var submission = secApi.ArchivesEdgarDataCIKSubmission(msgDownload.CompanyCode, msgDownload.Filing);
            if (submission != null && submission.Files != null)
            {
                var filingDownloadedDto = new FilingDownloaded()
                {
                    RegulatorCode = msgDownload.RegulatorCode,
                    CompanyCode = msgDownload.CompanyCode,
                    Filing = msgDownload.Filing
                };


                var loopResult = Parallel.ForEach(submission.Files, f =>
                {
                    var subFile = secApi.ArchivesEdgarDataCIKSubmissionFile(msgDownload.CompanyCode, msgDownload.Filing, f.Name);
                    if (subFile != null)
                    {
                        string blobPath = string.Format($"{msgDownload.RegulatorCode}\\{msgDownload.CompanyCode}\\{msgDownload.Filing}\\{subFile.Name}");
                        var newFile = new Fineo.Interfaces.FileInfo();
                        newFile.Path = blobPath;
                        newFile.Content = subFile.Content.ToArray();

                        if (fileStorage.UploadAsync(newFile).Result)
                        {
                            filingDownloadedDto.Docs.Add(blobPath);
                        }
                    }
                });


                var notificationDto = new MessageBusDTO();
                notificationDto.Body = JsonConvert.SerializeObject(filingDownloadedDto);
                notificationDto.MessageID = Guid.NewGuid().ToString();
                msbOutNotification.Send(notificationDto);

            }

        }
    }
}
