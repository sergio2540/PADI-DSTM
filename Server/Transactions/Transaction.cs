﻿using System;
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

        private HashSet<int> modifiedObjects;

        private TransactionInServerState TransactionState{ get; set; }
      

        public Transaction(ulong transactionId, String coordinatorAddress) {
            this.transactionId = transactionId;
            this.coordinatorAddress = coordinatorAddress;
            this.TransactionState = TransactionInServerState.Running;
            this.modifiedObjects = new HashSet<int>();
        }

        public int[] getModifiedObjectIds(){
            
            int[] changeableModifiedObjects = new int[modifiedObjects.Count];
            modifiedObjects.CopyTo(changeableModifiedObjects);
           
            return changeableModifiedObjects;
            
        }

        public void addModifiedObjectId(int objectId)
        {
            modifiedObjects.Add(objectId);
        }

        public void setVoteAbort()
        {

            TransactionState = TransactionInServerState.VoteAbort;
        }
        public void setVoteCommit()
        {

            TransactionState = TransactionInServerState.VoteCommit;
        }

        public void setCommited() {

            TransactionState = TransactionInServerState.Committed;
        }

        public void setAborted()
        {

            TransactionState = TransactionInServerState.Aborted;
        }

    }
}
                                                      