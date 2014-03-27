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
        TransactionalManager transactionalManager;

        public ServerImpl()
        {
            transactionalManager = new TransactionalManager();
        }


        public bool StartTransaction(ulong tid, string coordinatorAddress)
        {
            throw new NotImplementedException();
        }

        //Transacções

        //Participante retorna voto true - commit false - abort
        public bool canCommit(ulong tid)
        {
            return transactionalManager.canCommit(tid);
        }

        //Participante deve fazer commit
        public bool doCommit(ulong tid)
        {
            return transactionalManager.doCommit(tid);
        }

        public bool doAbort(ulong tid)
        {
            return transactionalManager.doAbort(tid);
        }


        //PadInt
        public PadInt CreatePadInt(ulong tid, int uid)
        {
            return transactionalManager.CreatePadInt(tid, uid);
        }

        public PadInt AccessPadInt(ulong tid, int uid)
        {
            return transactionalManager.AccessPadInt(tid, uid);
        }

        public int ReadPadInt(ulong tid, int uid)
        {
            return transactionalManager.Read(tid, uid);
        }

        public void WritePadInt(ulong tid, int uid, int value)
        {
            transactionalManager.Write(tid,uid,value);
        }

    }
}