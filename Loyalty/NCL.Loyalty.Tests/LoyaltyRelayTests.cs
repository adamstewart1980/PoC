using Moq;
using NCL.Loyalty.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCL.Loyalty.Tests
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
	          transactionID: "abc123"
	        },
	        {
	          cardNumber: "12345678909876",
	          transactionID: "def456"
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
            cardLogPath = @"";
        }

        [Ignore("because")]
        [Test]
        public void EnsureTransactionsConvertsToJSONCorrectly()
        {
            var transactions = new TransactionList { Transactions = new List<Transaction> { new Transaction { CardNumber = "abc12345", TransactionID = "1234567890123" } } };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(transactions, Newtonsoft.Json.Formatting.Indented);

            System.Diagnostics.Debug.Write(json);

            Assert.IsTrue(true);
        }

        [Test]
        public void EnsureCanReadLoyaltyLog()
        {
            //loyalty.2019-11-18.log
            this.cardLogPath = @"C:\Dev\WIP\Loyalty\NCL.Loyalty.Tests\Data\loyalty.2019-11-18.log";

            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = "1048577";

            var cardNumber = relay.GetCardNumberForTransaction(transactionId);

            Assert.IsTrue("18329202421833" == cardNumber);
        }

        [Ignore("becasue")]
        [Test]
        public void EnsureCardNumberIsFoundInLog()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = It.IsAny<string>();

            var cardNumber = relay.GetCardNumberForTransaction(transactionId);

            Assert.IsNotNull(cardNumber);
        }

        [Ignore("becasue")]
        [Test]
        public void EnsureExceptionIfLogFileMissing()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = It.IsAny<string>();

            Assert.Throws<Exception>(() => relay.GetCardNumberForTransaction(It.IsAny<string>()));
        }


        [Ignore("becasue")]
        [Test]
        public void EnsureExceptionIfTransactionIdNotFoundInLog()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = "";

            Assert.Throws<KeyNotFoundException>(() => relay.GetCardNumberForTransaction(transactionId));
        }


        [Ignore("becasue")]
        [Test]
        public void EnsureTransactionDetailsSentToEndpoint()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = string.Empty;
            var cardNumber = string.Empty;

            Assert.IsTrue(relay.SendTransactionDetails(new Transaction { TransactionID = transactionId, CardNumber = cardNumber }));
        }


        [Ignore("becasue")]
        [Test]
        public void EnsureTransactionDetailsLoggedForRetryOnEndpointFailure()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionId = string.Empty;
            var cardNumber = string.Empty;

            Assert.IsFalse(relay.SendTransactionDetails(new Transaction { TransactionID = transactionId, CardNumber = cardNumber }));

            Assert.IsTrue(relay.GetTransactionsForRetry()?.Transactions?.Any(x => x.TransactionID == transactionId));
        }


        [Ignore("becasue")]
        [Test]
        public void EnsureRetryTransactionsSentToEndpoint()
        {
            var relay = new LoyaltyRelay(cardLogPath, retryFilePath);

            var transactionsForRelay = relay.GetTransactionsForRetry();

            Assert.IsTrue(relay.SendTransactionDetails(transactionsForRelay));
        }
    }
}
