using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum TransactionInServerState { Running, VoteCommit, Committed, VoteAbort, Aborted}
    class Transaction
    {
        private ulong transactionId;
        private String coordinatorAddress;
        private TransactionInServerState TransactionState{ get; set; }

        public Transaction(ulong transactionId, String coordinatorAddress) {
            this.transactionId = transactionId;
            this.coordinatorAddress = coordinatorAddress;
            TransactionState = TransactionInServerState.Running;
        }

    }
}
                                                      