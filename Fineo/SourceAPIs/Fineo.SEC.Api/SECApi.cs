
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace Fineo.SEC.Api
{
    public class SECApi
    {
        private static string BaseURL = "https://www.sec.gov/";

        #region Json responses

        
        class ArchivesEdgarDirectoryItem
        {
            [JsonProperty("name")]
            public string name
            {
                get;
                set;
            }

            [JsonProperty("last-modified")]
            public string last_modified
            {
                get;
                set;
            }

            [JsonProperty("type")]
            public string type
            {
                get;
                set;
            }

            [JsonProperty("size")]
            public string size
            {
                get;
                set;
            }
        }

        
        class ArchivesEdgarDirectory
        {
            [JsonProperty("item")]
            public List<ArchivesEdgarDirectoryItem> item
            {
                get;
                set;
            }

            [JsonProperty("name")]
            public string name
            {
                get;
                set;
            }

            [JsonProperty("parent-dir")]
            public string parent_dir
            {
                get;
                set;
            }
        }

        
        class ArchivesEdgarDataCIKResponse
        {
            [JsonProperty("directory")]
            public ArchivesEdgarDirectory directory
            {
                get; set;
            }
        }

        
        class ArchivesEdgarDataCIKSubmissionResponse
        {
            [JsonProperty("directory")]
            public ArchivesEdgarDirectory directory
            {
                get; set;
            }
        }
        #endregion

        private static DateTime _LastCall = DateTime.UtcNow;
        private static object _lock = new object();

        // Call: /Archives/edgar/data/<CIK>
        public Submissions ArchivesEdgarDataCIK(string cik)
        {
            AvoidBlocking();

            string Command = "/Archives/edgar/data/{0}/index.json";

            Submissions submissions = null;

            Uri baseUri = new Uri(BaseURL);

            using (var client = new WebClient())
            {
                string request = string.Format(Command, cik);

                string content = client.DownloadString(new Uri(baseUri, request));
                                
                ArchivesEdgarDataCIKResponse model = JsonConvert.DeserializeObject<ArchivesEdgarDataCIKResponse>(content);

                submissions = Convert(model);
                if (submissions != null)
                {
                    submissions.CIK = cik;
                    submissions.TimeStamp = DateTime.UtcNow;
                }
            }

            return submissions;
        }

        // Call: /Archives/edgar/data/<CIK>/<Submission Access Number>
        public Submission ArchivesEdgarDataCIKSubmission(string cik, string accessNumber)
        {
            AvoidBlocking();

            string Command = "/Archives/edgar/data/{0}/{1}/index.json";

            Submission submission = null;

            Uri baseUri = new Uri(BaseURL);

            using (var client = new WebClient())
            {
                string request = string.Format(Command, cik, accessNumber);

                string content = client.DownloadString(new Uri(baseUri, request));

                ArchivesEdgarDataCIKSubmissionResponse model = JsonConvert.DeserializeObject<ArchivesEdgarDataCIKSubmissionResponse>(content);

                submission = Convert(model);
                submission.Name = accessNumber;
            }

            return submission;
        }

        // Call: /Archives/edgar/data/<CIK>/<Submission Access Number>/<File Name>
        public SubmissionFile ArchivesEdgarDataCIKSubmissionFile(string cik, string accessNumber, string fileName)
        {
            AvoidBlocking();

            string Command = "/Archives/edgar/data/{0}/{1}/{2}";

            SubmissionFile submission = null;

            Uri baseUri = new Uri(BaseURL);

            using (var client = new WebClient())
            {
                string request = string.Format(Command, cik, accessNumber, fileName);

                byte[] fileContent = client.DownloadData(request);

                submission = Convert(fileName, fileContent);

            }

            return submission;
        }

        #region Support methods

        /// <summary>
        /// There is a limitation in SEC APi: there can be only 10 requests per second from single client. 
        /// This function records the time of last call and if delta is less then 0.1 sec - performs the delay to avoid blocking
        /// </summary>
        private static void AvoidBlocking()
        {
            lock (_lock)
            {
                Thread.Sleep(150);
            }
        }

        private Submissions Convert(ArchivesEdgarDataCIKResponse model)
        {
            Submissions submissions = new Submissions();

            foreach (var item in model.directory.item)
            {
                SubmissionFolderInfo folder = new SubmissionFolderInfo(item.name, !string.IsNullOrEmpty(item.last_modified) ? DateTime.Parse(item.last_modified) : DateTime.MinValue);
                submissions.Folders.Add(folder);
            }

            return submissions;
        }

        private Submission Convert(ArchivesEdgarDataCIKSubmissionResponse model)
        {
            Submission submission = new Submission(model.directory.name, DateTime.MinValue);

            foreach (var item in model.directory.item)
            {
                SubmissionFileInfo folder = new SubmissionFileInfo(item.name, !string.IsNullOrEmpty(item.last_modified) ? DateTime.Parse(item.last_modified) : DateTime.MinValue, !string.IsNullOrEmpty(item.size) ? UInt32.Parse(item.size) : 0);
                submission.Files.Add(folder);
            }

            return submission;
        }

        private SubmissionFile Convert(string fileName, byte[] fileContent)
        {
            SubmissionFile file = new SubmissionFile(fileName);

            file.Content.AddRange(fileContent);

            return file;
        }
        #endregion

    }
}
