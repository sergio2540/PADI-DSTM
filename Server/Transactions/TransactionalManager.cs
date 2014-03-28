using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Transactions;
using System.Diagnostics;


//PadIntRemote

//PadIntTentative : PadIntRemote

//PadIntCommited : PadIntRemote

namespace Server
{
    public class TransactionalManager
    {
        private Dictionary<int,PadIntTransaction> objectsInServer = new Dictionary<int,PadIntTransaction>();

        internal TransactionalManager(){

        }

        internal PadIntCommitted CreatePadInt(int uid)////////////////para qur o 
        {

            if(objectsInServer.ContainsKey(uid))
                return null;


            PadIntTransaction obj = new PadIntTransaction(uid);

            ulong writeTimestampDefault = 0;

            PadIntCommitted committed = new PadIntCommitted(uid, writeTimestampDefault);

            obj.setCommited(committed);
            objectsInServer[uid] = obj;

            return committed;
           

        }

        internal PadIntCommitted AccessPadInt(int uid)
        {
            PadIntTransaction obj = objectsInServer[uid];
            
            if (obj == null)
                return null;
            
            return obj.getCommitted();
            
        }

        internal int Read(ulong tid, int uid)
        {

            ServerApp.debug = "Read called!";
            //PadIntTransaction obj = objectsInServer[uid];
           
            if (!objectsInServer.ContainsKey(uid))
            {
                //O objecto nao foi criado no servidor
                throw new PadIntNotExists(tid,uid);
            }

            PadIntTransaction obj = objectsInServer[uid];
            PadIntCommitted committed = obj.getCommitted();
           // return committed.Read();

            ulong tc = committed.WriteTimestamp;
            ServerApp.debug = "Object commited with value: " + committed.Value;


            if (tid < tc)
                throw new PadIntReadTooLate(tid, uid);

            SortedList<ulong,PadIntTentative> tentatives = obj.getTentatives();
            if(tentatives.Count == 0)
                return committed.Value;

            PadIntTentative mostUpdated = tentatives.Values.Where(x => ((x.WriteTimestamp < tid) ? true : false)).Max(x => x.WriteTimestamp < tid ? x : null);
            ulong tMax = (ulong) mostUpdated.WriteTimestamp;

            if (tc <= tMax)//pode ler
                return mostUpdated.Value;
            else return -1;//espera
        }

        internal void Write(ulong tid, int uid, int value)
        {

            ServerApp.debug = "Write called!";

            PadIntTransaction obj = objectsInServer[uid];//verificar se existe. isto é perigoso.
            obj.getTentatives()[tid].Value = 9;

            if (!objectsInServer.ContainsKey(uid))
            {
                //O objecto nao foi criado no servidor
                throw new PadIntNotExists(tid, uid);
            }

            PadIntCommitted committed = obj.getCommitted();
            ulong tc = committed.WriteTimestamp;

            //Verificacao 1: Ja existem escritas committed com timestamp superior ao desta transaccao 
            if (tid <= tc)
                throw new PadIntWriteTooLate(tid,uid);
            //Debug.WriteLine();

            SortedList<ulong,PadIntTentative> tentatives = obj.getTentatives();
            
            PadIntTentative t;
            if(tentatives.Count == 0){
                ServerApp.debug = "First tentative";
                t = new PadIntTentative(uid, 0, tid);
                t.Write(value);
                obj.addTentative(tid,t);
                return;
            }

            //Max de timestamp de leitura das versoes

            //Func<PadIntTentative, decimal> lam = tent => (decimal) tent.ReadTimestamp;

           // ulong tMax = tentatives.Max<PadIntTentative,ulong>(x => x.ReadTimestamp); //metodos extendidos. porque nao da?

            ulong tMax = (ulong) tentatives.Max(x => x.Value.ReadTimestamp);

            

            //Verificacao 2: Ja existem leituras de transaccoes a serem processadas
            if (tid < tMax)
            {
                throw new PadIntWriteTooLate(tid, uid);
            }

            PadIntTentative transactionTentative = tentatives[tid];
            if (transactionTentative != null)
            {
                Console.WriteLine("NOTNULLLLLLLLLLLLLLLLLLLLLLLLL!!!!!");

                // transactionTentative.Write(value);--> ja existe uma property
                transactionTentative.Value = value;

            }

            else
            {
                t = new PadIntTentative(uid, 0, tid);
                t.Write(value);
            }
        }

        internal bool canCommit(ulong tid)
        {
            throw new NotImplementedException();
        }

        internal bool doCommit(ulong tid)
        {
            throw new NotImplementedException();
        }

        internal bool doAbort(ulong tid)
        {
            throw new NotImplementedException();
        }
    }
}
