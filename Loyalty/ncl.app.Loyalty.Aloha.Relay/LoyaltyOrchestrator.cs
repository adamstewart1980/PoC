using ncl.app.Loyalty.Aloha.Relay;
using ncl.app.Loyalty.Aloha.Relay.Interfaces;
using ncl.app.Loyalty.Aloha.Relay.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCL.Loyalty
{
    public class LoyaltyOrchestrator
    {
        private ILogWriter LogWriter { get; set; }
        private Configuration InterceptConfiguration { get; set; }

        public LoyaltyOrchestrator(Configuration interceptConfiguration, ILogWriter logWriter)
        {
            this.LogWriter = logWriter;
            this.InterceptConfiguration = interceptConfiguration;
        }

        public async Task<bool?> SendTransactionsAsync(string transactionId)
        {
            this.LogWriter.WriteLog($"Starting {nameof(SendTransactionsAsync)}:: with {nameof(this.InterceptConfiguration.AppSettings.CardLogPath)}:{this.InterceptConfiguration.AppSettings.CardLogPath}, {nameof(this.InterceptConfiguration.AppSettings.RetryListPath)}:{this.InterceptConfiguration.AppSettings.RetryListPath}, {nameof(transactionId)}:{transactionId}");
            
            return await Task.Run(() =>
            {
                var relay = new LoyaltyRelay(this.InterceptConfiguration, this.LogWriter);
                var fileKeys = new List<string> { LoyaltyRelay.TransactionFile_Keys_CardNumber, LoyaltyRelay.TransactionFile_Keys_TimeStamp };

                var data = relay.ReadTransactionFile(fileKeys, "1048577" /*transactionId*/); //TODO:[ADAM] remove this after test

                //make sure we found the card and transactio details in the log file
                if (data != null)
                {
                    this.LogWriter.WriteLog($"{nameof(SendTransactionsAsync)}:: Data from transaction file is not null");

                    var transactions = new TransactionList
                    {
                        Transactions = new List<Transaction>
                        {
                            new Transaction
                            {
                                CardNumber = data[LoyaltyRelay.TransactionFile_Keys_CardNumber],
                                TimeStamp = DateTime.Parse(data[LoyaltyRelay.TransactionFile_Keys_TimeStamp]),
                                HostName = Environment.MachineName,// "DUMMY_HOST_NAME", //TODO:[ADAM] Get the host name from somewhere
                                TransactionID = transactionId
                            }
                        }
                    };

                    this.LogWriter.WriteLog($"{nameof(SendTransactionsAsync)}:: Sending Transaction");

                    return relay.SendTransactionDetails(transactions);
                }

                this.LogWriter.WriteLog($"{nameof(SendTransactionsAsync)}:: returning null because transaction log didnt find any data");
                //returning null here will simply indicate no loyalty card number found
                return (bool?)null;
            });
        }
    }
}