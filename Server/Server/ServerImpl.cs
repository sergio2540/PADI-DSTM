using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using Server;
using System.Diagnostics;



namespace Server
{
    class ServerImpl : MarshalByRefObject, IServer
    {

        TransactionalManager transactionalManager;
        TimestampService timestampService;

        public ServerImpl()
        {
            transactionalManager = new TransactionalManager();
            timestampService = new TimestampService();
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

            return transactionalManager.CreatePadInt(tid, uid, false);
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
            int temp = 0;

         
            temp = transactionalManager.Read(tid, uid);
            

            return temp;
        }

        public void WritePadInt(ulong tid, int uid, int value)
        {
            if (ServerApp.inFailMode)
                throw new FailStateException("WritePadInt");

            if (ServerApp.inFreezeMode)
                ServerApp.frozenCalls.WaitOne();

            transactionalManager.Write(tid, uid, value);
            
        }


        public bool Status()
        {
            //TODO

            ICollection<PadIntTransaction> padIntTransaction = transactionalManager.GetPadIntsTransaction();


            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("COMMITTED:");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("UID\t\t\tVALUE\t\t\tTIMESTAMP");

            foreach(PadIntTransaction padInt in padIntTransaction) {
                Console.Write(padInt.getCommitted().uid);
                Console.Write("\t\t\t");
                Console.Write(padInt.getCommitted().Value);
                Console.Write("\t\t\t");
                Console.WriteLine(padInt.getCommitted().WriteTimestamp);
            }
            
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("TENTATIVE:");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("TID\t\t\tUID\t\t\tVALUE\t\t\tWRITETIMESTAMP\t\t\tREADTIMESTAMP");

            foreach (PadIntTransaction padInt in padIntTransaction) {

                foreach(KeyValuePair<ulong, PadIntTentative> padIntTentatives in padInt.getTentatives()) {
                    Console.Write(padIntTentatives.Key);
                    Console.Write("\t\t\t");
                    Console.Write(padIntTentatives.Value.uid);
                    Console.Write("\t\t\t");
                    Console.Write(padIntTentatives.Value.Value);
                    Console.Write("\t\t\t");
                    Console.Write(padIntTentatives.Value.WriteTimestamp);
                    Console.Write("\t\t\t");
                    Console.Write(padIntTentatives.Value.ReadTimestamp);
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            
            return true;
        }

        public bool Fail()
        {
            if(ServerApp.inFailMode)
                throw new FailStateException("Fail");

            if (ServerApp.inFreezeMode)
            {
                ServerApp.frozenCalls.WaitOne();
            }
            ServerApp.inFailMode = true;
            return true;
        }

        public bool Freeze()
        {
            if (ServerApp.inFreezeMode)
                throw new AlreadyFrozenException();
            ServerApp.inFreezeMode = true;
            ServerApp.frozenCalls.Reset();
            //throw new NotImplementedException();
            return true;
        }

        public bool Recover()
        {

            if ((!ServerApp.inFailMode) && (!ServerApp.inFreezeMode))
                throw new NotFailedOrFrozenException();

            
            ServerApp.inFailMode = false;
            ServerApp.inFreezeMode = false;
            ServerApp.frozenCalls.Set();
            return true;


        }

        public ulong GetTid(){
            return timestampService.getTimestamp(); 
        }

        public ulong GetMaxTID() {

            return transactionalManager.GetMaxTID();

        }

        public void Init(ulong tid, string url) {
            transactionalManager.SetMaxTID(tid);
            transactionalManager.SetReplica(url);
        }

        public void SetReplica(string url)
        {
            transactionalManager.SetReplica(url);
        }

        public void AddTIDToPendingTable(string url, ulong tid, int startRange, int endRange) {
            transactionalManager.AddTIDToPendingTable(url, tid, startRange, endRange);
        }

        public void SendPadInt(List<PadIntRemote> padInts) {
            transactionalManager.AddPadInts(padInts);
        }

        

    }
}