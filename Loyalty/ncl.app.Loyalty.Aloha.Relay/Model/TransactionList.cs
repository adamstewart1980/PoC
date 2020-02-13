using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ncl.app.Loyalty.Aloha.Relay.Model
{
    [DataContract]
    public class TransactionList
    {
        [DataMember(Name="transactions")]
        public List<Transaction> Transactions { get; set; }
    }
}
