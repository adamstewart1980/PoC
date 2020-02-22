using System;
using System.Runtime.Serialization;
using System.Text;

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

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{nameof(TransactionID)}={TransactionID}, ");
            builder.Append($"{nameof(CardNumber)}={CardNumber}, ");
            builder.Append($"{nameof(TimeStamp)}={TimeStamp}, ");
            builder.Append($"{nameof(HostName)}={HostName}, ");

            return builder.ToString();
        }
    }
}