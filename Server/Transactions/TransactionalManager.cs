using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Transactions;


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

        internal PadIntCommitted CreatePadInt(ulong tid, int uid)
        {

            if(objectsInServer.ContainsKey(uid))
                return null;


            PadIntTransaction obj = new PadIntTransaction(uid);

            ulong writeTimestampDefault = 0;

            PadIntCommitted committed = new PadIntCommitted(uid, writeTimestampDefault);

            obj.setCommited(committed);

            return committed;
           

        }

        internal PadIntCommitted AccessPadInt(ulong tid, int uid)
        {
            PadIntTransaction obj = objectsInServer[uid];
            
            if (obj == null)
                return null;
            
            return obj.getCommitted();
            
        }

        internal int Read(ulong tid, int uid)
        {
            PadIntTransaction obj = objectsInServer[uid];
           
            if (obj == null)
            {
                //O objecto nao foi criado no servidor
                throw new PadIntNotExists(tid,uid);
            }

            PadIntCommitted committed = obj.getCommitted();

            ulong tc = committed.WriteTimestamp;

            if (tid < tc)
                throw new PadIntReadTooLate(tid, uid);



            throw new NotImplementedException();
        }

        internal void Write(ulong tid, int uid, int value)
        {
            PadIntTransaction obj = objectsInServer[uid];

            if (obj == null)
            {
                //O objecto nao foi criado no servidor
                throw new PadIntNotExists(tid, uid);
            }

            PadIntCommitted committed = obj.getCommitted();
            ulong tc = committed.WriteTimestamp;

            //Verificacao 1: Ja existem escritas committed com timestamp superior ao desta transaccao 
            if (tid <= tc)
                throw new PadIntWriteTooLate(tid,uid);


            SortedSet<PadIntTentative> tentatives = obj.getTentatives();
            
            PadIntTentative t;
            if(tentatives.Count == 0){
                t = new PadIntTentative(uid, 0, tid);
                t.Write(value);
                obj.addTentative(t);
                return;
            }

            //Max de timestamp de leitura das versoes
            ulong tMax = tentatives.Max.ReadTimestamp;

            //Verificacao 2: Ja existem leituras de transaccoes a serem processadas
            if (tid < tMax)
            {
                throw new PadIntWriteTooLate(tid, uid);
            }

            if(tentatives.Contains()){

            }
            t = new PadIntTentative(uid, 0, tid);
            t.Write(value);
            
            if(tentatives.Add())
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
