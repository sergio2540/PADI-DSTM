using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

using CommonTypes;

namespace Server
{
    public class TransactionalManager
    {
        private Dictionary<int, PadIntTransaction> objectsInServer;
        private Dictionary<ulong, Transaction> transactions;

        //Objecto esta a espera que uma transacção faça commit
        // int -> uid do objecto
        // EventWaitHandle -> thread
        private Dictionary<int, EventWaitHandle> objectWaitHandle;

        //Depois de votar commit, esta transacção deve esperar pelo doCommit por parte do coordenador
        // int -> uid do objecto
        // EventWaitHandle -> thread
        private Dictionary<int, EventWaitHandle> pendingTransactions;

        private EventWaitHandle waitForPadIntTransfer;

        //novo
        private ulong maxTID = ulong.MaxValue;

        private IServer replica = null;

        //velho
        private List<Tuple<string, ulong, int, int>> pendingTransactionSplitted;

        internal TransactionalManager()
        {

            objectsInServer = new Dictionary<int, PadIntTransaction>();
            transactions = new Dictionary<ulong, Transaction>();
            objectWaitHandle = new Dictionary<int, EventWaitHandle>();
            pendingTransactions = new Dictionary<int, EventWaitHandle>();
            waitForPadIntTransfer = new EventWaitHandle(false, EventResetMode.ManualReset);
            pendingTransactionSplitted = new List<Tuple<string, ulong, int, int>>();

        }

        internal ICollection<PadIntTransaction> GetPadIntsTransaction()
        {
            return objectsInServer.Values;
        }

        internal ulong GetMaxTID()
        {
            if (transactions.Count != 0)
            {
                return transactions.Keys.Max<ulong>();
            }
            return ulong.MinValue;
        }

        internal void SetMaxTID(ulong tid)
        {
            this.maxTID = tid;
        }

        internal void SetReplica(string replica_url)
        {
            IServer replica = (IServer)Activator.GetObject(typeof(IServer), replica_url);
            this.replica = replica;
        }

        internal PadIntCommitted CreatePadInt(ulong tid, int uid, bool isReplicating)
        {
            
            Console.WriteLine("Create padInt " + uid);
            
            if (objectsInServer.ContainsKey(uid))
                return null;


            PadIntTransaction obj = new PadIntTransaction(uid);

            PadIntCommitted committed = new PadIntCommitted(uid);
            obj.setCommited(committed);

            objectsInServer[uid] = obj;

            //wait handle não notificado e com manual reset pois deve ser o write a fechar a passagem às threads.
            objectWaitHandle[uid] = new EventWaitHandle(false, EventResetMode.ManualReset);//esta errado? se for criado, todas as threads devem puder passar por handle certo?
            pendingTransactions[uid] = new EventWaitHandle(false, EventResetMode.ManualReset);



            if (!isReplicating)
                Write(tid, uid, 0);

            
            return committed;


        }

        internal PadIntCommitted AccessPadInt(int uid)
        {


            //Nao foi encontrado o PadInt procurado
            //Se calhar devia lançar excepção
            if (!objectsInServer.ContainsKey(uid))
            {
                Console.WriteLine("Nao foi encontrado " + uid);
                return null;
            }

            PadIntTransaction obj = objectsInServer[uid];

            //O PadInt foi encontrado mas é null
            if (obj == null)
                return null;

            return obj.getCommitted();

        }

        internal int Read(ulong tid, int uid)
        {

            //ServerApp.debug = "Read called!";

            //fica a espera que sejam transferidos dados de um servidor que sofreu divisao do seeu range pela entrada deste novo server
            if (tid > maxTID)
            {
                //Console.WriteLine("read: tid>mazxTID");

                waitForPadIntTransfer.Reset();
            }


            if (!objectsInServer.ContainsKey(uid))
            {
                //O objecto nao foi criado no servidor
                throw new PadIntNotExists(tid, uid);
            }

            PadIntTransaction obj = objectsInServer[uid];
            PadIntCommitted committed = obj.getCommitted();
            // return committed.Read();

            ulong tc = committed.WriteTimestamp;
            //ServerApp.debug = "Object commited with value: " + committed.Value;

            //Console.WriteLine(String.Format("on read:Tid:{0} Tc:{1}", tid, tc));
            if (tid <= tc)
                throw new PadIntReadTooLate(tid, uid);

            SortedList<ulong, PadIntTentative> tentatives = obj.getTentatives();

            //se não existirem versões tentativa, ele não/ninguém escreveu. pode ler do commited.
            if (tentatives.Count == 0)
            {
                objectsInServer[uid].addTentative(tid, new PadIntTentative(uid, tid, tid, committed.Value));//////////////////////////////////////////////////////////////////////

                transactions[tid].addModifiedObjectId(uid); ////depois de modificar o object, adiciona-lo à transaccao para sabermos o que mudamos no fim.

                pendingTransactions[uid].Reset(); // dado que temos uma tentativa, as que forem fazer commit teem de esperar

                return committed.Value;
            }


            //se ja foi escrita uma versao, entao le-se dessa versão.
            if (tentatives.ContainsKey(tid))
            {
                PadIntTentative ownTentative = tentatives[tid];
                ownTentative.ReadTimestamp = tid;
                return ownTentative.Value;
            }

            //versao de tentativa que tem um timestamp de escrita superior a todos os inferiores ao da transaccao que quer ler.
            PadIntTentative mostUpdated = tentatives.Values.Where(x => ((x.WriteTimestamp < tid) ? true : false)).Max(x => x.WriteTimestamp < tid ? x : null);

            //se nao existe nenhum que tenha escrito e que tenha timestamp inferior, significa que a transaccao actual pode ler o valor do commited.
            if (mostUpdated == null)
            {
                objectsInServer[uid].addTentative(tid, new PadIntTentative(uid, tid, tid, committed.Value));//////////////////////////////////////////////////////////////////////

                transactions[tid].addModifiedObjectId(uid); ////depois de modificar o object, adiciona-lo à transaccao para sabermos o que mudamos no fim.

                pendingTransactions[uid].Reset(); // dado que temos uma tentativa, as que forem fazer commit teem de esperar
                return committed.Value;
            }


            ulong tMax = mostUpdated.WriteTimestamp;// este e o valor do maior timestamp de escrita menor que o da transaccao

            //verificar se a versao commited tem um timestamp igual ao de tmax. se tiver, significa que o tmax está commited. pode então ler
            if (tc == tMax)//pode ler 
            {
                mostUpdated.ReadTimestamp = tid;
                return mostUpdated.Value;
            }
            else //espera que a transaccao faca commit e volta a repetir todos os passos até aqui.É criada uma thred para cada chamada a esta função.
            {
                objectWaitHandle[uid].WaitOne(); //bloqueia e quando fôr desbloqueada, volta a tentar.

                return Read(tid, uid);
            }
        }

        internal void Write(ulong tid, int uid, int value)
        {
            //addicionar objecto modificado

            //ServerApp.debug = "Write called!";

            //fica a espera que sejam transferidos dados de um servidor que sofreu divisao do seeu range pela entrada deste novo server
            if (tid > maxTID)
            {
                waitForPadIntTransfer.Reset();
            }

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
                throw new PadIntWriteTooLate(tid, uid);
            //Debug.WriteLine();

            SortedList<ulong, PadIntTentative> tentatives = obj.getTentatives();

            PadIntTentative t;

            //Não existem transações a mexer no objecto com identificador uid
            if (tentatives.Count == 0)
            {
                t = new PadIntTentative(uid, tid, value);

                obj.addTentative(tid, t);

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

            ulong tMax = (ulong)tentatives.Max(x => x.Value.ReadTimestamp);

            //Console.WriteLine("Tid: " + tid + "Tmax: " + tMax);

            //Verificacao 2: Ja existem leituras de transaccoes a serem processadas
            if (tid < tMax)
            {
                throw new PadIntWriteTooLate(tid, uid);
            }


            if (tentatives.ContainsKey(tid))
            {
                PadIntTentative transactionTentative = tentatives[tid];
                // transactionTentative.Write(value);--> ja existe uma property
                //se tem tentativa é porque já escreveu antes. Não é preciso resetar o handle? Ou é????????
                transactionTentative.Value = value;
            }

            else//primeira tentativa da transacçao tid
            {

                t = new PadIntTentative(uid, tid, value);
                obj.addTentative(tid, t);

                transactions[tid].addModifiedObjectId(uid); ////depois de modificar o object, adiciona-lo à transaccao para sabermos o que mudamos no fim.
                objectWaitHandle[uid].Reset();///primeira tentativa de escrita. Reseta handle para bloquear threads.
                pendingTransactions[uid].Reset(); // dado que escrevemos, as que forem fazer commit teem de esperar

            }
        }


        internal bool BeginTransaction(ulong tid, string coordinatorAddress)
        {
            //TODO: Lançar excepção.
            if (transactions.ContainsKey(tid))
                return false;

            transactions[tid] = new Transaction(tid, coordinatorAddress);
            return true;
        }


        internal bool canCommit(ulong tid)//vai dar sempre canCommit???
        {


            Transaction transaction = transactions[tid];//assume-se que existe!!!!!!!!!!!!!!!!
            bool decision = true;
            SortedList<ulong, PadIntTentative> tentatives = null;

            foreach (int modifiedObjectId in transaction.getModifiedObjectIds())
            {
                //Console.WriteLine("Tid: " + tid + "modificou" + modifiedObjectId);
    
                tentatives = objectsInServer[modifiedObjectId].getTentatives();
                while (tentatives.Min(x => x.Value.WriteTimestamp) < tid) //se houver apenas 1, é ele próprio e pode fazer commit.
                // while ((tentatives.Count > 1) && (tentatives.Min(x => x.Value.WriteTimestamp) < tid)) //se houver apenas 1, é ele próprio e pode fazer commit.
                {                                                                                     //o que acontece se não houverem objectos com um timestamp inferior?
                    pendingTransactions[modifiedObjectId].WaitOne();//se houver um objecto com um write time stamp inferior, temos de esperar por ele.
                    //o que acontece se depois de dar decisao de commit, aparece outra transaccao com um tid menor que os que la estavam
                    //
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

            PadIntTransaction objectTransaction = null;
            PadIntTentative tentative = null;
            PadIntCommitted commited = null;

            List<PadIntRemote> toReplicate = new List<PadIntRemote>();

            //desbloquear threads em espera no read com
            Console.WriteLine("Em do commit Tid: " + tid);

            foreach (int modifiedObject in transactions[tid].getModifiedObjectIds())
            {
                //Console.WriteLine("Em do commit no for Tid: " + tid + "modificou" + modifiedObject);

                //por commited
                //remover objecto transaccao
                //remover objecto tentativa
                //por tentativa como commited se vamos remover vale a pena? ter em atenção que no read ele ve se o anterior esta commited. nao sei se deviamos eliminar.
                //actualizar o valor do commited


                objectTransaction = objectsInServer[modifiedObject];

                SortedList<ulong, PadIntTentative> tentatives = objectTransaction.getTentatives();
                tentative = tentatives[tid];

                commited = objectTransaction.getCommitted();
                commited.WriteTimestamp = tid;
                commited.Value = tentative.Value;

                toReplicate.Add(commited);

                tentative.SetCommited();
                tentatives.Remove(tid);

                objectWaitHandle[modifiedObject].Set();//fez commit.temos de notificar threads para avancarem.
                pendingTransactions[modifiedObject].Set();
            }

            transactions[tid].setCommited();
            //Transacção efectuda e agora removida
            transactions.Remove(tid);

            if(replica != null)
                replica.SendPadInt(toReplicate);
            
            foreach(var p in toReplicate)
                Console.WriteLine("replicado = " + p.uid +" " + p.Value);
            
            checkTableOfPendingTransactions(tid);

            return true; //sempre verdade?

        }

        private void checkTableOfPendingTransactions(ulong tid)
        {

            Console.WriteLine(pendingTransactionSplitted.Count);
            Console.WriteLine("checkTableOfPendingTransactions: " + tid);

            bool transferDone = false;

            foreach (Tuple<string, ulong, int, int> tuple in pendingTransactionSplitted)
            {
                
                //(tuple.Item2 != 0) pq e a primeira vez

                if (tid >= tuple.Item2)
                {
                    IServer server = (IServer)Activator.GetObject(typeof(IServer), tuple.Item1);



                    List<PadIntRemote> padIntsToSend = new List<PadIntRemote>();
                    foreach (var padInt in objectsInServer)
                    {


                        if (padInt.Key >= tuple.Item3 && padInt.Key <= tuple.Item4)
                        {
                            padIntsToSend.Add(padInt.Value.getCommitted());
                        }

                    }

                    //TODO ter em conta que pode falhar
                    server.SendPadInt(padIntsToSend);
                    //foreach (var padInt in padIntsToSend)
                    //{
                    //    objectsInServer.Remove(padInt.uid);
                    //}

                }
            }
        }

        internal bool doAbort(ulong tid)
        {

            Console.WriteLine("doAbort");

            PadIntTransaction objectTransaction = null;
            PadIntTentative tentative = null;
            //desbloquear threads em espera no read com
            foreach (int modifiedObject in transactions[tid].getModifiedObjectIds())
            {

                Console.WriteLine("Abort: " + modifiedObject);
                //por abort
                //remover objecto transaccao check
                //remover objecto tentativa check
                //por tentativa como commited se vamos remover vale a pena? ter em atenção que no read ele ve se o anterior esta commited. nao sei se deviamos eliminar.
                //actualizar o valor do commited


                objectTransaction = objectsInServer[modifiedObject];

               


                SortedList<ulong, PadIntTentative> tentatives = objectTransaction.getTentatives();

                tentative = tentatives[tid];
                tentative.SetAborted();

                tentatives.Remove(tid);

                PadIntCommitted com = objectTransaction.getCommitted();

                //Remove creates
                if (com.WriteTimestamp == 0)
                {
                    objectsInServer.Remove(modifiedObject);

                    objectWaitHandle[modifiedObject].Set();//fez commit.temos de notificar threads para avancarem.
                    pendingTransactions[modifiedObject].Set();

                    objectWaitHandle.Remove(modifiedObject);//fez commit.temos de notificar threads para avancarem.
                    pendingTransactions.Remove(modifiedObject);
                }
                else
                {

                    objectWaitHandle[modifiedObject].Set();//fez commit.temos de notificar threads para avancarem.
                    pendingTransactions[modifiedObject].Set();
                }
            
            
            }

            transactions[tid].setAborted();
            //Transacção efectuda e agora removida
            transactions.Remove(tid);

            return true; //sempre verdade?

        }

        public void AddPadInts(List<PadIntRemote> padInts)
        {
            Console.WriteLine("recebeu dados");
            foreach (PadIntCommitted pad in padInts)
            {

               

                Console.WriteLine("recebeu padint " + pad.uid);
                Console.WriteLine("recebeu padint " + pad.Value);

                PadIntCommitted p = CreatePadInt(pad.WriteTimestamp, pad.uid, true);


                //Ainda na existe no server
                if (p != null)
                {
                    p.WriteTimestamp = pad.WriteTimestamp;
                    p.Value = pad.Value;
                }
                else
                {
                    //ja existe no server
                    objectsInServer[pad.uid].getCommitted().WriteTimestamp = pad.WriteTimestamp;
                    objectsInServer[pad.uid].getCommitted().Value = pad.Value;
                }

                //PadIntTransaction padTransaction = new PadIntTransaction(pad.uid);
                //padTransaction.setCommited(pad);

                //if (!objectsInServer.ContainsKey(pad.uid)){
                //    objectsInServer.Add(pad.uid, padTransaction);
                //}
                //else
                //{
                //    objectsInServer[pad.uid] = padTransaction;
                //}

                //objectWaitHandle[uid] = new EventWaitHandle(false, EventResetMode.ManualReset);//esta errado? se for criado, todas as threads devem puder passar por handle certo?
                //pendingTransactions[uid] = new EventWaitHandle(false, EventResetMode.ManualReset);

            
            }

            //transferencia concluida
            Console.WriteLine("transferencia concluida");
            waitForPadIntTransfer.Set();

        }

        public void AddTIDToPendingTable(string url, ulong tid, int startRange, int endRange)
        {
            Console.WriteLine("url: " + url + " tid: " + tid);


            pendingTransactionSplitted.Add(Tuple.Create(url, tid, startRange, endRange));

            //nao estao a decorrer transaccoes
            if (transactions.Count == 0)
            {
               checkTableOfPendingTransactions(tid + 1);
            }

        }

    }
}
