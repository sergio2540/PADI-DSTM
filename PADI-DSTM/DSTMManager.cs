using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    public class DSTMManager
    {
        private ulong currentTransactionId = 0;
        private Coordinator coordinator;

        public bool Init()
        {
            coordinator = new Coordinator();
            return true;
        }

        public bool TxBegin()
        {

            ulong timestamp = Oracle.getTimestamp();

            currentTransactionId = timestamp;
            
            //quando retornar falso?quando se tenta criar uma transaccao com outra a decorrer?
            return coordinator.BeginTransaction(timestamp);

           
        }

        public bool TxCommit()
        {
            return coordinator.CommitTransaction();
        }

        public bool TxAbort()
        {
            return coordinator.AbortTransaction();
        }

        public bool Status()
        {
            throw new NotImplementedException();
        }

        public bool Fail(String URL)
        {
            throw new NotImplementedException();
        }

        public bool Freeze(String URL)
        {
            throw new NotImplementedException();
        }

        public bool Recover(String URL)
        {
            throw new NotImplementedException();
        }

        public PadInt CreatePadInt(int uid)
        {
            //Informa servidor que existe novo Padint
            //INCOMPLETO
            return new PadInt(uid);
        }
        

        public PadInt AccessPadInt(int uid)
        {
            throw new NotImplementedException();
        }

       
    }
}
