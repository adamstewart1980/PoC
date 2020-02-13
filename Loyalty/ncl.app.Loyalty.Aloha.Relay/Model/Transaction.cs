using System;
using System.Runtime.Serialization;

namespace ncl.app.Loyalty.Aloha.Relay.Model
{
    [DataContract]
    public class Transaction
    {
        [DataMember(Name = "transactionID")]
        public string TransactionID { get; set; }

        [DataMember(Name = "cardNumber")]
        public string CardNumber { get; set; }

        [DataMember(Name = "timeStamp")]
        public DateTime TimeStamp { get; set; }

        [DataMember(Name = "hostName")]
        public string HostName { get; set; }
    }
}