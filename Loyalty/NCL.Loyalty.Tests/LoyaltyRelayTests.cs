using Moq;
using ncl.app.Loyalty.Aloha.Relay;
using ncl.app.Loyalty.Aloha.Relay.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ncl.app.Loyalty.Aloha.Tests
{
    [TestFixture]
    public class LoyaltyRelayTests
    {
        #region Payload Definition
        /*
        {
          transactions: [
            {
	          cardNumber: "12345678901234",
	          transactionID: "abc123",
              hostName: "",
              timeStamp: ""
	        },
	        {
	          cardNumber: "12345678909876",
	          transactionID: "def456",
              hostName: "",
              timeStamp: ""
	        }
          ]
        }
        */
        #endregion

        private string retryFilePath = @"";
        private string cardLogPath = @"";

        [SetUp]
        public void SetUp()
        {
            retryFilePath = @"";
            cardLogPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Data\\loyalty.2019-11-18.log";
        }

        [Test]
        public void EnsureTransactionsConvertsToJSONCorrectly()
        {
            var time1 = DateTime.Now;

            //just to keep some difference in the times
            Thread.Sleep(2);
            
            var time2 = DateTime.UtcNow;
            
            var transactions = new TransactionList
            {
                Transactions = new List<Transaction>
                {
                    new Transaction { CardNumber = "abc12345", TransactionID = "1234567890123", HostName = "N988-HNS", TimeStamp = time1 },
                    new Transaction { CardNumber = "abc98765", TransactionID = "9876543210987", HostName = "N432-FSD", TimeStamp = time2}
                }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(transactions, Newtonsoft.Json.Formatting.None);
            var expectedResult = "{\"transactions\":[{\"transactionID\":\"1234567890123\",\"cardNumber\":\"abc12345\",\"timeStamp\":\"" + time1.ToString("o") + "\",\"hostName\":\"N988-HNS\"},{\"transactionID\":\"9876543210987\",\"cardNumber\":\"abc98765\",\"timeStamp\":\"" + time2.ToString("o") + "\",\"hostName\":\"N432-FSD\"}]}";

            System.Diagnostics.Debug.WriteLine(json);
            System.Diagnostics.Debug.WriteLine(expectedResult);

            Assert.IsTrue(expectedResult == json);
        }

        [Test]
        public void EnsureCanReadLoyaltyLog()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = "1048577";

            var keys = new List<string> 
            {
                LoyaltyRelay.TransactionFile_Keys_CardNumber, 
                LoyaltyRelay.TransactionFile_Keys_TimeStamp,  
                LoyaltyRelay.TransactionFile_Keys_TransactionId,
                LoyaltyRelay.TransactionFile_Keys_TransactionType
            };

            var logFileData = relay.ReadTransactionFile(keys, transactionId);

            Assert.NotNull(logFileData);
            Assert.IsTrue(logFileData.Keys.Count > 0);
            Assert.IsTrue(logFileData.ContainsKey(LoyaltyRelay.TransactionFile_Keys_CardNumber));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(logFileData[LoyaltyRelay.TransactionFile_Keys_CardNumber]));
            Assert.IsTrue("18329202421833" == logFileData[LoyaltyRelay.TransactionFile_Keys_CardNumber]);
        }

        [Test]
        public void EnsureExceptionIfLogFileMissing()
        {
            this.cardLogPath = string.Empty;
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = It.IsAny<string>();

            var keys = new List<string>
            {
                LoyaltyRelay.TransactionFile_Keys_TransactionType
            };

            Assert.Throws<FileNotFoundException>(() => relay.ReadTransactionFile(keys, "sldkjfslkfj"));
        }

        [Test]
        public void EnsureExceptionIfTransactionIdNotFoundInLog()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = "iu794857645897";

            var keys = new List<string>
            {
                LoyaltyRelay.TransactionFile_Keys_CardNumber,
                LoyaltyRelay.TransactionFile_Keys_TimeStamp,
                LoyaltyRelay.TransactionFile_Keys_TransactionId,
                LoyaltyRelay.TransactionFile_Keys_TransactionType
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => relay.ReadTransactionFile(keys, transactionId));
        }

        [TestCase("")]
        [TestCase("     ")]
        public void EnsureExceptionIfTransactionIdIsNullOrEmpty(string transactionId)
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var keys = new List<string>
            {
                LoyaltyRelay.TransactionFile_Keys_CardNumber,
            };

            Assert.Throws<ArgumentNullException>(() => relay.ReadTransactionFile(keys, transactionId));
        }

        [Test]
        public void EnsureExceptionIfNoFileKeysSent()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = "1048577";
            
            Assert.Throws<ArgumentException>(() => relay.ReadTransactionFile(null, transactionId));
            Assert.Throws<ArgumentException>(() => relay.ReadTransactionFile(new List<string> { }, transactionId));
        }

        [Test]
        public void EnsureExceptionIfUnknownKeySent()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            Assert.Throws<ArgumentOutOfRangeException>(() => relay.ReadTransactionFile(new List<string> { "UNKNOWN_KEY"}, "sdlkjfsdlkjfsl"));
        }

        [Test]
        public void EnsureTransactionDetailsSentToEndpoint()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);
            
            var transactionId = "1000001";
            var cardNumber = "10000000000001";
            var hostName = "NDO-TEST";
            var timeStamp = DateTime.Now;

            Assert.IsTrue(relay.SendTransactionDetails(new Transaction { TransactionID = transactionId, CardNumber = cardNumber, HostName = hostName, TimeStamp = timeStamp }));
        }


        [Ignore("because")]
        [Test]
        public void EnsureTransactionDetailsLoggedForRetryOnEndpointFailure()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = string.Empty;
            var cardNumber = string.Empty;

            Assert.IsFalse(relay.SendTransactionDetails(new Transaction { TransactionID = transactionId, CardNumber = cardNumber }));

            Assert.IsTrue(relay.GetTransactionsForRetry()?.Transactions?.Any(x => x.TransactionID == transactionId));
        }


        [Ignore("because")]
        [Test]
        public void EnsureRetryTransactionsSentToEndpoint()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionsForRelay = relay.GetTransactionsForRetry();

            Assert.IsTrue(relay.SendTransactionDetails(transactionsForRelay));
        }
    }
}
