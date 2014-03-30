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


        public bool BeginTransaction(ulong tid, string coordinatorAddress)
        {
            return transactionalManager.BeginTransaction(tid, coordinatorAddress);
            //Transaction newTransaction = new Transaction(tid,coordinatorAddress);
            //throw new NotImplementedException();
        }

        public override object InitializeLifetimeService()
        {

            return null;

        }

        //Transacções

        //Participante retorna voto true - commit false - abort
        public bool canCommit(ulong tid)
        {
            ServerApp.debug = "canCommit called!";
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
            return transactionalManager.CreatePadInt(uid);
        }

        public PadInt AccessPadInt(ulong tid, int uid)
        {
            return transactionalManager.AccessPadInt(uid);
        }

        public int ReadPadInt(ulong tid, int uid)
        {
            return transactionalManager.Read(tid, uid);
        }

        public void WritePadInt(ulong tid, int uid, int value)
        {
            transactionalManager.Write(tid,uid,value);
        }


        public bool Status()
        {
            throw new NotImplementedException();
        }

        public bool Fail(string URL)
        {
            throw new NotImplementedException();
        }

        public bool Freeze(string URL)
        {
            throw new NotImplementedException();
        }

        public bool Recover(string URL)
        {
            throw new NotImplementedException();
        }
    }
}