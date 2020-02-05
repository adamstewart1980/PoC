using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NCL.Loyalty.Model
{
    [DataContract]
    public class TransactionList
    {
        [DataMember(Name="transactions")]
        public List<Transaction> Transactions { get; set; }
    }
}
