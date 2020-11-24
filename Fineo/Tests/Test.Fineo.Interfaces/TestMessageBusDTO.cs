using Fineo.Interfaces;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Test.Fineo.Interfaces
{
    public class TestMessageBusDTO
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_MessageBusDTO_SenderID()
        {
            string senderId = "test_sender_id";

            MessageBusDto msgBusDto = new MessageBusDto()
            {
                SenderID = senderId
            };

            Assert.AreEqual( msgBusDto.SenderID, senderId );
            
        }

        [Test]
        public void Test_MessageBusDTO_MessageID()
        {
            string messageId = "test_message_id";

            MessageBusDto msgBusDto = new MessageBusDto()
            {
                MessageID = messageId
            };

            Assert.AreEqual(msgBusDto.MessageID, messageId);

        }

        [Test]
        public void Test_MessageBusDTO_Body()
        {
            string body = "test_body";

            MessageBusDto msgBusDto = new MessageBusDto()
            {
                Body = body
            };

            Assert.AreEqual(msgBusDto.Body, body);

        }

        [Test]
        public void Test_MessageBusDTO_TwoEqual()
        {
            string senderId = "test_sender_id";
            string messageId = "test_message_id";
            string body = "test_body";

            MessageBusDto dto1 = new MessageBusDto()
            {
                SenderID = senderId,
                MessageID = messageId,
                Body = body
            };

            MessageBusDto dto2 = new MessageBusDto()
            {
                SenderID = senderId,
                MessageID = messageId,
                Body = body
            };

            Assert.IsTrue(dto1.Equals(dto2));

        }

        [Test]
        public void Test_MessageBusDTO_TwoNotEqual_SenderID()
        {
            string senderId1 = "test_sender_id1";
            string senderId2 = "test_sender_id2";
            string messageId = "test_message_id";
            string body = "test_body";

            MessageBusDto dto1 = new MessageBusDto()
            {
                SenderID = senderId1,
                MessageID = messageId,
                Body = body
            };

            MessageBusDto dto2 = new MessageBusDto()
            {
                SenderID = senderId2,
                MessageID = messageId,
                Body = body
            };

            Assert.IsFalse(dto1.Equals(dto2));

        }

        [Test]
        public void Test_MessageBusDTO_TwoNotEqual_MessageID()
        {
            string senderId = "test_sender_id";
            string messageId1 = "test_message_id1";
            string messageId2 = "test_message_id2";
            string body = "test_body";

            MessageBusDto dto1 = new MessageBusDto()
            {
                SenderID = senderId,
                MessageID = messageId1,
                Body = body
            };

            MessageBusDto dto2 = new MessageBusDto()
            {
                SenderID = senderId,
                MessageID = messageId2,
                Body = body
            };

            Assert.IsFalse(dto1.Equals(dto2));

        }

        [Test]
        public void Test_MessageBusDTO_TwoNotEqual_Body()
        {
            string senderId = "test_sender_id";
            string messageId = "test_message_id";
            string body1 = "test_body1";
            string body2 = "test_body2";

            MessageBusDto dto1 = new MessageBusDto()
            {
                SenderID = senderId,
                MessageID = messageId,
                Body = body1
            };

            MessageBusDto dto2 = new MessageBusDto()
            {
                SenderID = senderId,
                MessageID = messageId,
                Body = body2
            };

            Assert.IsFalse(dto1.Equals(dto2));

        }

        [Test]
        public void Test_MessageBusDTO_SerializeDeserialize()
        {
            string body = "test_body";
            string messageId = "messageId";
            string senderId = "senderId";

            MessageBusDto msgBusDto = new MessageBusDto()
            {
                MessageID = messageId,
                SenderID = senderId,
                Body = body
            };

            string serialized = JsonConvert.SerializeObject(msgBusDto);

            MessageBusDto deserialized = JsonConvert.DeserializeObject<MessageBusDto>(serialized);

            Assert.IsTrue(msgBusDto.Equals(deserialized));

        }
    }
}