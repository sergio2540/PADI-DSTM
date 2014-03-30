using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Transactions;
using System.Diagnostics;
using System.Threading;


//PadIntRemote

//PadIntTentative : PadIntRemote

//PadIntCommited : PadIntRemote

namespace Server
{
    public class TransactionalManager
    {
        private Dictionary<int,PadIntTransaction> objectsInServer; 
        private Dictionary<ulong, Transaction> transactions;  
        private Dictionary <int,EventWaitHandle> objectWaitHandle;
        private Dictionary<int, EventWaitHandle> pendingTransactions;

        internal TransactionalManager(){

            objectsInServer = new Dictionary<int,PadIntTransaction>();
            transactions = new Dictionary<ulong, Transaction>();
            objectWaitHandle = new Dictionary<int,EventWaitHandle>();
            pendingTransactions = new Dictionary<int, EventWaitHandle>();
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

            //wait handle não notificado e com manual reset pois deve ser o write a fechar a passagem às threads.
            objectWaitHandle[uid] = new EventWaitHandle(false, EventResetMode.ManualReset);
            pendingTransactions[uid] = new EventWaitHandle(false, EventResetMode.ManualReset);

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

            //ServerApp.debug = "Read called!";
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
            //ServerApp.debug = "Object commited with value: " + committed.Value;


            if (tid < tc)
                throw new PadIntReadTooLate(tid, uid);

            SortedList<ulong,PadIntTentative> tentatives = obj.getTentatives();
            //se não existirem versões tentativa, ele não/ninguém escreveu. pode ler do commited.
            if(tentatives.Count == 0)
                return committed.Value;

            //se ja foi escrita uma versao, entao le-se dessa versão.
            if (tentatives.ContainsKey(tid))
            {
                ServerApp.debug = "estamos a ler e ha uma versao de tentativa.";
                PadIntTentative ownTentative = tentatives[tid];
                ownTentative.ReadTimestamp = tid;
                return ownTentative.Value;
            }
            
            //versao de tentativa que tem um timestamp de escrita superior a todos os inferiores ao da transaccao que quer ler.
            PadIntTentative mostUpdated = tentatives.Values.Where(x => ((x.WriteTimestamp < tid) ? true : false)).Max(x => x.WriteTimestamp < tid ? x : null);

            //se nao existe nenhum que tenha escrito e que tenha timestamp inferior, significa que a transaccao actual pode ler o valor do commited.
            if (mostUpdated == null)
                return committed.Value;

            ulong tMax = (ulong) mostUpdated.WriteTimestamp;// este e o valor do maior timestamp de escrita menor que o da transaccao

            //verificar se a versao commited tem um timestamp igual ao de tmax. se tiver, significa que o tmax está commited. pode então ler
            if (tc == tMax)//pode ler 
            {
                mostUpdated.ReadTimestamp = tid;
                return mostUpdated.Value;
            }
            else //espera que a transaccao faca commit e volta a repetir todos os passos até aqui.É criada uma thred para cada chamada a esta função.
            {
                objectWaitHandle[uid].WaitOne(); //bloqueia e quando fôr desbloqueada, volta a tentar.
                return Read(tid,uid);
            }           
        }

        internal void Write(ulong tid, int uid, int value)
        {//addicionar objecto modificado

            //ServerApp.debug = "Write called!";

            //PadIntTransaction obj = objectsInServer[uid];//verificar se existe. isto é perigoso.
           // obj.getTentatives()[tid].Value = 9;

            if (!objectsInServer.ContainsKey(uid))
            {
                //O objecto nao foi criado no servidor
                throw new PadIntNotExists(tid, uid);
            }

            PadIntTransaction obj = objectsInServer[uid];
            PadIntCommitted committed = obj.getCommitted();
            ulong tc = committed.WriteTimestamp;

            //Verificacao 1: Ja existem escritas committed com timestamp superior ao desta transaccao 
            if (tid <= tc)
                throw new PadIntWriteTooLate(tid,uid);
            //Debug.WriteLine();

            SortedList<ulong,PadIntTentative> tentatives = obj.getTentatives();
            
            PadIntTentative t;
            if(tentatives.Count == 0){
                ServerApp.debug = "tentative added for 0 tentatives";
                t = new PadIntTentative(uid, 0, tid);
                t.Write(value);
                ServerApp.debug = "Written value: " + t.Read();
                obj.addTentative(tid,t);
                transactions[tid].addModifiedObjectId(uid); ////depois de modificar o object, adiciona-lo à transaccao para sabermos o que mudamos no fim.
                objectWaitHandle[uid].Reset();//escreveu, bloqueia as threads. O que acontece se não fôr a tempo de bloquear as threads???????
                pendingTransactions[uid].Reset(); // dado que escrevemos, as que forem fazer commit teem de esperar
                //evitar que venha a esperar por si mesma espera por si mesma?
                return;
            }

            //Max de timestamp de leitura das versoes

            //Func<PadIntTentative, decimal> lam = tent => (decimal) tent.ReadTimestamp;

           // ulong tMax = tentatives.Max<PadIntTentative,ulong>(x => x.ReadTimestamp); //metodos extendidos. porque nao da? Não apagar. Ainda tenho de 
            //perceber isto Ass:Braga :)

            ulong tMax = (ulong) tentatives.Max(x => x.Value.ReadTimestamp);

            

            //Verificacao 2: Ja existem leituras de transaccoes a serem processadas
            if (tid < tMax)
            {
                throw new PadIntWriteTooLate(tid, uid);
            }

            
            if(tentatives.ContainsKey(tid))
            //if (transactionTentative != null)
            {
                PadIntTentative transactionTentative = tentatives[tid];
                Console.WriteLine("NOTNULLLLLLLLLLLLLLLLLLLLLLLLL!!!!!");
                // transactionTentative.Write(value);--> ja existe uma property
                //se tem tentativa é porque já escreveu antes. Não é preciso resetar o handle? Ou é????????
                transactionTentative.Value = value;
                

            }

            else//primeira tentativa
            {
                ServerApp.debug = "new tentative. not empty";
                t = new PadIntTentative(uid, 0, tid);
                t.Write(value);
                obj.addTentative(tid,t);
                transactions[tid].addModifiedObjectId(uid); ////depois de modificar o object, adiciona-lo à transaccao para sabermos o que mudamos no fim.
                objectWaitHandle[uid].Reset();///primeira tentativa de escrita. Reseta handle para bloquear threads.
                pendingTransactions[uid].Reset(); // dado que escrevemos, as que forem fazer commit teem de esperar

            }
        }


        internal bool BeginTransaction(ulong tid, string coordinatorAddress)
        {
            ServerApp.debug = "Transaction begun!";
            //TODO: Lançar excepção.
            if (transactions.ContainsKey(tid))
                return false;

            transactions[tid] = new Transaction(tid,coordinatorAddress);
            return true;
        }


        internal bool canCommit(ulong tid)//vai dar sempre canCommit???
        {
            ServerApp.debug = "Can commit!";

            Transaction transaction = transactions[tid];//assume-se que existe!!!!!!!!!!!!!!!!
            bool decision = true;
            foreach (int modifiedObjectId in transaction.getModifiedObjectIds())
            {
                while (objectsInServer[modifiedObjectId].getTentatives().Min(x => x.Value.WriteTimestamp) < tid)
                {
                    pendingTransactions[modifiedObjectId].WaitOne();//se houver um objecto com um write time stamp inferior, temos de esperar por ele.

                }
                decision &= true;
            }

            if (decision)
                transaction.setVoteCommit();
            else
                transaction.setVoteAbort();


            return decision;
        }

        //o doCommit deveria ser atomico. pode dar problema?
        internal bool doCommit(ulong tid)
        {
            ServerApp.debug = "Do commit!";

            PadIntTransaction objectTransaction = null;
            PadIntTentative tentative = null;
            //desbloquear threads em espera no read com
            foreach (int modifiedObject in transactions[tid].getModifiedObjectIds())
            {
                //por commited
                //remover objecto transaccao
                //remover objecto tentativa
                //por tentativa como commited se vamos remover vale a pena? ter em atenção que no read ele ve se o anterior esta commited. nao sei se deviamos eliminar.
                //actualizar o valor do commited

                transactions[tid].setCommited();
                objectTransaction = objectsInServer[modifiedObject];
                tentative = objectTransaction.getTentatives()[tid];
                objectTransaction.getCommitted().Value = tentative.Value;
                tentative.SetCommited();
                objectWaitHandle[modifiedObject].Set();//fez commit.temos de notificar threads para avancarem.
                pendingTransactions[modifiedObject].Set();
            }

            return true; //sempre verdade?

        }

        internal bool doAbort(ulong tid)
        {
            ServerApp.debug = "Do abort!";

            PadIntTransaction objectTransaction = null;
            PadIntTentative tentative = null;
            //desbloquear threads em espera no read com
            foreach (int modifiedObject in transactions[tid].getModifiedObjectIds())
            {
                //por commited
                //remover objecto transaccao
                //remover objecto tentativa
                //por tentativa como commited se vamos remover vale a pena? ter em atenção que no read ele ve se o anterior esta commited. nao sei se deviamos eliminar.
                //actualizar o valor do commited

                transactions[tid].setAborted();
                objectTransaction = objectsInServer[modifiedObject];
                tentative = objectTransaction.getTentatives()[tid];
                tentative.SetAborted();
                objectWaitHandle[modifiedObject].Set();//fez commit.temos de notificar threads para avancarem.
            }

            return true; //sempre verdade?
        }
    }
}
