using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    class DSTMManager
    {
        private ulong currentTransactionId = 0;
        private Coordinator coordinator;

        bool Init()
        {
            coordinator = new Coordinator();
        }

        bool TxBegin()
        {

            ulong timestamp = Oracle.getTimestamp();

            currentTransactionId = timestamp;
            
            //quando retornar falso?quando se tenta criar uma transaccao com outra a decorrer?
            return coordinator.BeginTransaction(timestamp);

           
        }

        bool TxCommit()
        {
            return coordinator.CommitTransaction();
        }

        bool TxAbort()
        {
            return coordinator.AbortTransaction();
        }

        bool Status();

        bool Fail(String URL);

        bool Freeze(String URL);

        bool Recover(String URL);

        PadInt CreatePadInt(int uid)
        {
            //Informa servidor que existe novo Padint
            //INCOMPLETO
            return new PadInt(uid);
        }

        PadInt AccessPadInt(int uid); 
    }
}
