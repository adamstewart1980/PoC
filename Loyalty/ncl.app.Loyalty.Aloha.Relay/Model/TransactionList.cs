using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ncl.app.Loyalty.Aloha.Relay.Model
{
    [DataContract]
    public class TransactionList
    {
        [DataMember(Name="transactions")]
        public List<Transaction> Transactions { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{nameof(Transactions)}=[");
            foreach(var transaction in this.Transactions)
            {
                builder.Append($"{nameof(Transaction)}={transaction.ToString()}, ");
            }
            builder.Append($"]");

            return builder.ToString();
        }
    }
}
