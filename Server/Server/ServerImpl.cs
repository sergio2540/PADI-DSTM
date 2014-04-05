using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using Server;


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
            if (ServerApp.inFailMode)
                throw new FailStateException("BeginTransaction");

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
            if (ServerApp.inFailMode)
                throw new FailStateException("canCommit");
            ServerApp.debug = "canCommit called!";
            return transactionalManager.canCommit(tid);
        }

        //Participante deve fazer commit
        public bool doCommit(ulong tid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("doCommit");
            return transactionalManager.doCommit(tid);
        }

        public bool doAbort(ulong tid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("doAbort");
            return transactionalManager.doAbort(tid);
        }


        //PadInt
        public PadInt CreatePadInt(ulong tid, int uid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("CreatePadInt");
            return transactionalManager.CreatePadInt(uid);
        }

        public PadInt AccessPadInt(ulong tid, int uid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("AccessPadInt");
            return transactionalManager.AccessPadInt(uid);
        }

        public int ReadPadInt(ulong tid, int uid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("ReadPadInt");
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

        public bool Fail()
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("Fail");
            ServerApp.inFailMode = true;
            return true;
        }

        public bool Freeze()
        {
            throw new NotImplementedException();
        }

        public bool Recover()
        {
            if(!ServerApp.inFailMode)
                throw new NotFailedException();
            ServerApp.inFailMode = false;
            return true;


        }
    }
}