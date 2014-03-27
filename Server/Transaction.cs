using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum State { Running,Commited,Aborted}
    class Transaction
    {
        private long transactionId;
        private String coordinatorAddress;
        private State transactionState;

        public Transaction(long transactionId, String coordinatorAddress) {
            this.transactionId = transactionId;
            this.coordinatorAddress = coordinatorAddress;
            this.transactionState = State.Running;
        }


        public State TransactionState{get; set;}


    }
}
