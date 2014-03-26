using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Transaction
    {
        private long transactionId;
        private String coordinatorAddress;
        
        public Transaction(long transactionId, String coordinatorAddress) {
            this.transactionId = transactionId;
            this.coordinatorAddress = coordinatorAddress;
        }


    }
}
