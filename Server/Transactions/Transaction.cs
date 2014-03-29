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
        private HashSet<int> modifiedObjects;
        private TransactionInServerState TransactionState{ get; set; }
      

        public Transaction(ulong transactionId, String coordinatorAddress) {
            this.transactionId = transactionId;
            this.coordinatorAddress = coordinatorAddress;
            TransactionState = TransactionInServerState.Running;
            modifiedObjects = new HashSet<int>();
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
        

    }
}
                                                      