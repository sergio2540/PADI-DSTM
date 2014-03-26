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
    }
}
