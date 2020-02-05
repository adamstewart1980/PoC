using NCL.Loyalty.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NCL.Loyalty
{
    public class LoyaltyRelay
    {
        private HttpClient httpClient;

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
            // create a request message
            var request = new HttpRequestMessage();

            // set the request send method
            request.Method = HttpMethod.Post;

            // set the request Uri
            request.RequestUri = new Uri(ConfigurationManager.AppSettings["EndpointMethod"], UriKind.Relative);

            // add the content type to the headers collection
            request.Headers.Add("ContentType", "application/json");
            request.Headers.Add("X-API-KEY ", ConfigurationManager.AppSettings["ApiKey"]);

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