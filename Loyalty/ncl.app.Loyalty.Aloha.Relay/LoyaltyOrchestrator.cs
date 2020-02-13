using ncl.app.Loyalty.Aloha.Relay;
using ncl.app.Loyalty.Aloha.Relay.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCL.Loyalty
{
    public class LoyaltyOrchestrator
    {

        public LoyaltyOrchestrator()
        {

        }

        public async Task<bool> SendTransactionsAsync(string cardLogPath, string retryListPath, string transactionId)
        {
            return await Task.Run(() =>
            {
                var relay = new LoyaltyRelay(cardLogPath, retryListPath);
                var fileKeys = new List<string> { LoyaltyRelay.TransactionFile_Keys_CardNumber, LoyaltyRelay.TransactionFile_Keys_TimeStamp };

                var data = relay.ReadTransactionFile(fileKeys, "1048577" /*transactionId*/); //TODO:[ADAM] remove this after test

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

                return relay.SendTransactionDetails(transactions);
            });
        }
    }
}