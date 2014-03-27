using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;


namespace Server
{
    class ServerImpl : IServer
    {
        public bool StartTransaction(long transactionId, string coordinatorAddress)
        {
            throw new NotImplementedException();
        }

        public bool CreatePadint(long transactionId, int padintUid)
        {
            throw new NotImplementedException();
        }

        public int ReadPadint(long transactionId, int padintUid)
        {
            throw new NotImplementedException();
        }

        public void WritePadInt(long transactionId, int padintUid, int newValue)
        {
            throw new NotImplementedException();
        }

        public bool StartTransaction(ulong transactionId, string coordinatorAddress)
        {
            throw new NotImplementedException();
        }

        public bool canCommit(ulong transactioId)
        {
            throw new NotImplementedException();
        }

        public bool doCommit(ulong transactionId)
        {
            throw new NotImplementedException();
        }

        public bool doAbort(ulong transactionId)
        {
            throw new NotImplementedException();
        }

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
