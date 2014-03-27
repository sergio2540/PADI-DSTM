using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;


namespace Server
{
    class ServerImpl : MarshalByRefObject, IServer
    {
        public bool StartTransaction(ulong transactionId, string coordinatorAddress)
        {
            throw new NotImplementedException();
        }

        //Transacções

        //Participante retorna voto true - commit false - abort
        public bool canCommit(ulong transactioId)
        {
            throw new NotImplementedException();
        }

        //Participante deve fazer commit
        public bool doCommit(ulong transactionId)
        {
            throw new NotImplementedException();
        }

        public bool doAbort(ulong transactionId)
        {
            throw new NotImplementedException();
        }


        //PadInt
        public bool CreatePadInt(ulong transactionId, int padintUid)
        {
            throw new NotImplementedException();
        }


        public int ReadPadInt(ulong transactionId, int padintUid)
        {
            throw new NotImplementedException();
        }

        public void WritePadInt(ulong transactionId, int padintUid, int newValue)
        {
            throw new NotImplementedException();
        }

    }
}
