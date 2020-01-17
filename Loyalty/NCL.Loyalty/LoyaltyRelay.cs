using NCL.Loyalty.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCL.Loyalty
{
    public class LoyaltyRelay
    {
        private string CardLogPath { get; set; }
        private string RetryListPath { get; set; }

        public LoyaltyRelay(string cardLogPath, string retryListPath)
        {
            this.CardLogPath = cardLogPath;
            this.RetryListPath = retryListPath;
        }

        public string GetCardNumberForTransaction(string transactionId)
        {
            if(File.Exists(this.CardLogPath))
            {
                using(var reader = new StreamReader(this.CardLogPath))
                {
                    var magicString = $",\"type\":\"loyalty.nfc_capture\",\"transaction_id\":\"{transactionId}\",\"loyalty_card\":\"";
                    var line = reader.ReadLine();
                    var cardNumber = "";

                    while (line != null)
                    {
                        //find the one line we are looking for
                        if (line.Contains(magicString))
                        {
                            var startIndex = line.IndexOf("loyalty_card");
                            startIndex += "loyalty_card\":\"".Length;
                            var snippet = line.Substring(startIndex, line.Length - startIndex);
                            snippet = snippet.Substring(0, snippet.IndexOf("\""));
                            cardNumber = snippet;
                            line = null;
                        }
                        else
                        {
                            //Read the next line
                            line = reader.ReadLine();
                        }
                    }

                    if(string.IsNullOrWhiteSpace(cardNumber))
                    {
                        throw new ArgumentOutOfRangeException("TransactionId");
                    }

                    return cardNumber;
                }
            }

            throw new FileNotFoundException($"{nameof(CardLogPath)}");
            //read the file

            //look for the line with the token

            //get the card number from the selected line

            //return the card number

            //throw new argument not found exception
            throw new NotImplementedException();
        }

        public bool SendTransactionDetails(Transaction transaction)
        {
            var transactions = new TransactionList { Transactions = new List<Transaction> { transaction } };

            return SendTransactionDetails(transactions);
        }

        public bool SendTransactionDetails(TransactionList transactions)
        {
            //serialize transactions

            //create httpclient or similar call to endpoint with payload
            throw new NotImplementedException();
        }

        public TransactionList GetTransactionsForRetry()
        {
            return ReadRetryFile();
        }

        private void AddToRetryList(Transaction transaction)
        {
            var retryItems = ReadRetryFile();
            
            //only add if we dont already have this transaction
            if (!retryItems.Transactions.Any(x => x.TransactionID == transaction.TransactionID))
            {
                retryItems.Transactions.Add(transaction);
            }

            var json = JsonConvert.SerializeObject(retryItems);

            //write all retry items to the file
            File.WriteAllText(this.RetryListPath, json);
        }

        private TransactionList ReadRetryFile()
        {
            if(File.Exists(this.RetryListPath))
            {
                using (StreamReader reader = new StreamReader(this.RetryListPath))
                {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<TransactionList>(json);
                }
            }

            return new TransactionList { };
        }
    }
}