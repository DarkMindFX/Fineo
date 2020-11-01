using Fineo.SEC.Api;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Net;
using System.Reflection;

namespace Test.Fineo.SEC.Api
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SubmissionsByCIK_Success()
        {
            SECApi api = new SECApi();

            IConfiguration config = GetConfiguration();

            string cik = config.GetValue(typeof(string), "SEC_CIK_AAPL").ToString();

            Submissions submissions = api.ArchivesEdgarDataCIK(cik);
            Assert.AreNotEqual(submissions, null, "Submissions are NULL");
            Assert.AreEqual(submissions.CIK, cik, "Invalid CIK returned");
            Assert.IsTrue(submissions.Folders.Count > 0, "List of folders is EMPTY");
        }

        [Test]
        public void SubmissionByAccessNumber_Success()
        {
            SECApi api = new SECApi();

            IConfiguration config = GetConfiguration();

            string cik = config.GetValue(typeof(string), "SEC_CIK_AAPL").ToString();
            string accessNum = config.GetValue(typeof(string), "SEC_CIK_AAPL_SUBMISSION_FOLDER_20170701").ToString();

            Submission submission = api.ArchivesEdgarDataCIKSubmission(cik, accessNum);
            Assert.AreNotEqual(submission, null, "Submission is NULL");
            Assert.AreEqual(submission.Name, accessNum, "Invalid submission name");
            Assert.IsTrue(submission.Files.Count > 0, "List of files is EMPTY");
        }

        [Test]
        public void SubmissionByAccessNumberFileXml_Success()
        {
            SECApi api = new SECApi();

            IConfiguration config = GetConfiguration();

            string cik = config.GetValue(typeof(string), "SEC_CIK_AAPL").ToString();
            string accessNum = config.GetValue(typeof(string), "SEC_CIK_AAPL_SUBMISSION_FOLDER_20170701").ToString();
            string fileName = config.GetValue(typeof(string), "SEC_CIK_AAPL_10Q_FILE_XML").ToString();

            SubmissionFile file = api.ArchivesEdgarDataCIKSubmissionFile(cik, accessNum, fileName);
            Assert.AreNotEqual(file, null, "Submission is NULL");
            Assert.AreEqual(file.Name, fileName, "Invalid file name");
            Assert.IsTrue(file.Content.Count > 0, "File content is empty");
        }

        [Test]
        public void SubmissionByAccessNumberFileZip_Success()
        {
            SECApi api = new SECApi();

            IConfiguration config = GetConfiguration();

            string cik = config.GetValue(typeof(string), "SEC_CIK_AAPL").ToString();
            string accessNum = config.GetValue(typeof(string), "SEC_CIK_AAPL_SUBMISSION_FOLDER_20170701").ToString();
            string fileName = config.GetValue(typeof(string), "SEC_CIK_AAPL_10Q_FILE_ZIP").ToString();

            SubmissionFile file = api.ArchivesEdgarDataCIKSubmissionFile(cik, accessNum, fileName);
            Assert.AreNotEqual(file, null, "Submission is NULL");
            Assert.AreEqual(file.Name, fileName, "Invalid file name");
            Assert.IsTrue(file.Content.Count > 0, "File content is empty");
        }

        [Test]
        public void SubmissionByAccessNumberFile_InvalidName()
        {
            SECApi api = new SECApi();

            IConfiguration config = GetConfiguration();

            string cik = config.GetValue(typeof(string), "SEC_CIK_AAPL").ToString();
            string accessNum = config.GetValue(typeof(string), "SEC_CIK_AAPL_SUBMISSION_FOLDER_20170701").ToString();
            string fileName = "2FA82DB1-8BFF-4363-B947-5A3BC70AA89D.xml";

            try
            {
                SubmissionFile file = api.ArchivesEdgarDataCIKSubmissionFile(cik, accessNum, fileName);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError &&
                                 ex.Response != null)
                {
                    var resp = (HttpWebResponse)ex.Response;
                    if (resp.StatusCode != HttpStatusCode.NotFound)
                    {
                        Assert.Fail(string.Format("Invalid error code returned - {0}", resp.StatusCode));
                    }
                    
                }
                else
                {
                    Assert.Fail(string.Format("Invalid exception returned - {0}", ex.ToString()));
                }
                
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