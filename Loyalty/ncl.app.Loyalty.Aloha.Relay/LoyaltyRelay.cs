using ncl.app.Loyalty.Aloha.Relay.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ncl.app.Loyalty.Aloha.Relay
{
    public class LoyaltyRelay
    {
        private HttpClient httpClient;

        public const string TransactionFile_Keys_CardNumber = "loyalty_card";
        public const string TransactionFile_Keys_TimeStamp = "timestamp";
        public const string TransactionFile_Keys_TransactionType = "type";
        public const string TransactionFile_Keys_TransactionId = "transaction_id";

        private string CardLogPath { get; set; }
        private string RetryListPath { get; set; }

        public LoyaltyRelay(string cardLogPath, string retryListPath)
        {
            this.CardLogPath = cardLogPath;
            this.RetryListPath = retryListPath;
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["EndpointBaseAddress"]);
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public Dictionary<string, string> ReadTransactionFile(List<string> fileKeys, string transactionId)
        {
            if(fileKeys == null || !fileKeys.Any())
            {
                throw new ArgumentException($"Must pass at least one fileKey to read from the logFile. {nameof(fileKeys)} is null or empty");
            }

            if(string.IsNullOrWhiteSpace(transactionId))
            {
                throw new ArgumentNullException(nameof(transactionId));
            }

            if(File.Exists(this.CardLogPath))
            {
                var result = new Dictionary<string, string> { };

                using(var reader = new StreamReader(this.CardLogPath))
                {
                    var magicString = $",\"type\":\"loyalty.nfc_capture\",\"transaction_id\":\"{transactionId}\",\"loyalty_card\":\"";
                    var line = reader.ReadLine();
                    
                    while (line != null)
                    {
                        //find the one line we are looking for
                        if (line.Contains(magicString))
                        {
                            var jsonLine = JsonConvert.DeserializeObject<LogFileLine>(line);

                            foreach(var fileKey in fileKeys)
                            {
                                switch (fileKey)
                                {
                                    case TransactionFile_Keys_CardNumber:
                                        result[fileKey] = jsonLine.LoyaltyCardNumber;
                                        break;
                                    case TransactionFile_Keys_TimeStamp:
                                        result[fileKey] = jsonLine.TimeStamp;
                                        break;
                                    case TransactionFile_Keys_TransactionType:
                                        result[fileKey] = jsonLine.TransactionType;
                                        break;
                                    case TransactionFile_Keys_TransactionId:
                                        result[fileKey] = jsonLine.TransactionId;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(fileKey), fileKey, $"Value is not expected as a key in the log file. Please check format");
                                }
                            }

                            line = null;
                        }
                        else
                        {
                            //Read the next line
                            line = reader.ReadLine();
                        }
                    }

                    //throw if we didnt find the card number in the file for the given transaction id
                    if(!result.ContainsKey(fileKeys[0]) || string.IsNullOrWhiteSpace(result[fileKeys[0]]))
                    {
                        throw new ArgumentOutOfRangeException(nameof(transactionId), $"Couldn't find line in logFile for supplied {nameof(transactionId)}: {transactionId}");
                    }

                    return result;
                }
            }

            throw new FileNotFoundException($"{nameof(CardLogPath)}");
        }

        public bool SendTransactionDetails(Transaction transaction)
        {
            var transactions = new TransactionList { Transactions = new List<Transaction> { transaction } };

            //we failed to send the transaction, save it for a later try
            if(!SendTransactionDetails(transactions))
            {
                AddToRetryList(transaction);
                return false;
            }

            return true;
        }

        public bool SendTransactionDetails(TransactionList transactions)
        {
            // create a request message
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(ConfigurationManager.AppSettings["EndpointMethod"], UriKind.Relative));

            // add the content type to the headers collection
            request.Headers.Clear();
            request.Headers.Add("ContentType", "application/json");
            request.Headers.Add("X-API-KEY", ConfigurationManager.AppSettings["ApiKey"]);
            
            // set the body content for the request
            var jsonString = JsonConvert.SerializeObject(transactions, new JsonSerializerSettings { Formatting = Formatting.None });
            request.Content = new StringContent(jsonString);

            // set the content type for the request
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // send the request asynchronously
            var response = this.httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
                //var json = response.Content.ReadAsStringAsync().Result;
                //return JsonConvert.DeserializeObject<int>(json);
            }
            else
            {
                //logger.LogError($"Failed to call {CreatePEDCheckUrl} returning status code: {response.StatusCode} and reason: {response.ReasonPhrase}");
                //TODO:<Adam> this needs logging throw new ApplicationException(response.ReasonPhrase);
            }

            //create httpclient or similar call to endpoint with payload
            return false;
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