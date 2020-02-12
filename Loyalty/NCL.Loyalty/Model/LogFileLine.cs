using System;
using System.Runtime.Serialization;

namespace NCL.Loyalty.Model
{
    [DataContract]
    public class LogFileLine
    {
        /* Expected line format
        {"timestamp":"2019-11-19T13:56:14.963Z","type":"loyalty.nfc_capture","transaction_id":"1048578","loyalty_card":"18329202336722"}
        */

        [DataMember(Name ="timestamp")]
        public string TimeStamp { get; set; }

        [DataMember(Name = "type")]
        public string TransactionType { get; set; }

        [DataMember(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [DataMember(Name = "loyalty_card")]
        public string LoyaltyCardNumber { get; set; }
    }
}
