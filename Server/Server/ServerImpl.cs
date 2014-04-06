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
            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();

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

            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();

            ServerApp.debug = "canCommit called!";
            return transactionalManager.canCommit(tid);
        }

        //Participante deve fazer commit
        public bool doCommit(ulong tid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("doCommit");

            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();

            return transactionalManager.doCommit(tid);
        }

        public bool doAbort(ulong tid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("doAbort");

            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();

            return transactionalManager.doAbort(tid);
        }


        //PadInt
        public PadInt CreatePadInt(ulong tid, int uid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("CreatePadInt");

            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();

            return transactionalManager.CreatePadInt(uid);
        }

        public PadInt AccessPadInt(ulong tid, int uid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("AccessPadInt");

            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();

            return transactionalManager.AccessPadInt(uid);
        }

        public int ReadPadInt(ulong tid, int uid)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("ReadPadInt");

            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();

            return transactionalManager.Read(tid, uid);
        }

        public void WritePadInt(ulong tid, int uid, int value)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("WritePadInt");

            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();
           
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

            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();

            ServerApp.inFailMode = true;
            return true;
        }

        public bool Freeze()
        {
            ServerApp.inFreezeMode = true;
            ServerApp.frozenCalls.Reset();
            //throw new NotImplementedException();
            return true;
        }

        public bool Recover()
        {
            if(!ServerApp.inFailMode)
                throw new NotFailedException();
            if (!ServerApp.inFreezeMode)
                throw new NotFrozenException();

            ServerApp.inFailMode = false;
            ServerApp.inFreezeMode = false;
            ServerApp.frozenCalls.Set();
            return true;


        }
    }
}