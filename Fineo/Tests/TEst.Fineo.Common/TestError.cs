using Fineo.Common;
using Fineo.Constants;
using NUnit.Framework;

namespace Test.Fineo.Common
{
    public class Test_Error
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Error_Trace_Success()
        {
            string message = "Trace info";
            Error error = new Error(EErrorType.Trace, message);

            Assert.AreEqual(error.Type, EErrorType.Trace);
            Assert.AreEqual(error.Message, message);
        }

        [Test]
        public void Error_Info_Success()
        {
            string message = "Info message";
            Error error = new Error(EErrorType.Info, message);

            Assert.AreEqual(error.Type, EErrorType.Info);
            Assert.AreEqual(error.Message, message);
        }

        [Test]
        public void Error_Warning_Success()
        {
            string message = "Warning info";
            Error error = new Error(EErrorType.Warning, message);

            Assert.AreEqual(error.Type, EErrorType.Warning);
            Assert.AreEqual(error.Message, message);
        }

        [Test]
        public void Error_Error_Success()
        {
            string message = "Error message";
            Error error = new Error(EErrorType.Error, message);

            Assert.AreEqual(error.Type, EErrorType.Error);
            Assert.AreEqual(error.Message, message);
        }
    }
}