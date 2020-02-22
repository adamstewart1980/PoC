using ncl.app.Loyalty.Aloha.Relay.Interfaces;
using ncl.app.Loyalty.Aloha.Relay.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        private Configuration InterceptConfiguration { get; set; }
        private ILogWriter LogWriter { get; set; }

        public LoyaltyRelay(Configuration interceptConfiguration, ILogWriter logWriter)
        {
            this.InterceptConfiguration = interceptConfiguration;
            this.LogWriter = logWriter;

            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(this.InterceptConfiguration.AppSettings.EndpointBaseAddress);
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public Dictionary<string, string> ReadTransactionFile(List<string> fileKeys, string transactionId)
        {
            if (fileKeys == null || !fileKeys.Any())
            {
                throw new ArgumentException($"Must pass at least one fileKey to read from the logFile. {nameof(fileKeys)} is null or empty");
            }

            if(string.IsNullOrWhiteSpace(transactionId))
            {
                throw new ArgumentNullException(nameof(transactionId));
            }

            this.LogWriter.WriteLog($"Starting {nameof(ReadTransactionFile)}:: with {nameof(fileKeys)}: {String.Join(",", fileKeys)} {nameof(transactionId)}: {transactionId}");
            
            if(File.Exists(this.InterceptConfiguration.AppSettings.CardLogPath))
            {
                var result = new Dictionary<string, string> { };

                using(var reader = new StreamReader(this.InterceptConfiguration.AppSettings.CardLogPath))
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

                    //return null if we dont find the record in the log file - this should help us shortcut the process
                    if (!result.ContainsKey(fileKeys[0]) || string.IsNullOrWhiteSpace(result[fileKeys[0]]))
                    {
                        this.LogWriter.WriteLog($"{nameof(ReadTransactionFile)}:: Returning null because we didnt find transaction {transactionId} in the file");

                        result = null;
                    }

                    this.LogWriter.WriteLog($"Finishing {nameof(ReadTransactionFile)}");

                    return result;
                }
            }

            this.LogWriter.WriteLog($"Finishing {nameof(ReadTransactionFile)}:: Couldn't find file {this.InterceptConfiguration.AppSettings.CardLogPath}");
            throw new FileNotFoundException($"{nameof(this.InterceptConfiguration.AppSettings.CardLogPath)}");
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
            this.LogWriter.WriteLog($"Starting {nameof(SendTransactionDetails)}:: with {nameof(transactions)}:{transactions.ToString()}");
            
            // create a request message
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(this.InterceptConfiguration.AppSettings.EndpointMethod, UriKind.Relative));

            // add the content type to the headers collection
            request.Headers.Clear();
            request.Headers.Add("ContentType", "application/json");
            request.Headers.Add("X-API-KEY", this.InterceptConfiguration.AppSettings.ApiKey);
            
            // set the body content for the request
            var jsonString = JsonConvert.SerializeObject(transactions, new JsonSerializerSettings { Formatting = Formatting.None });
            request.Content = new StringContent(jsonString);

            // set the content type for the request
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // send the request asynchronously
            var response = this.httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                this.LogWriter.WriteLog($"Finishing {nameof(SendTransactionDetails)}:: returning {true}");

                return true;
            }
            else
            {
                this.LogWriter.WriteLog($"Error {nameof(SendTransactionDetails)}:: Failed sending payload. Response was {response.StatusCode}-{response.ReasonPhrase}");
            }

            this.LogWriter.WriteLog($"Finishing {nameof(SendTransactionDetails)}:: returning {true}");

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
            File.WriteAllText(this.InterceptConfiguration.AppSettings.RetryListPath, json);
        }

        private TransactionList ReadRetryFile()
        {
            if(File.Exists(this.InterceptConfiguration.AppSettings.RetryListPath))
            {
                using (StreamReader reader = new StreamReader(this.InterceptConfiguration.AppSettings.RetryListPath))
                {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<TransactionList>(json);
                }
            }

            return new TransactionList { };
        }
    }
}