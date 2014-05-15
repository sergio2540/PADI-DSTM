using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;
using System.Diagnostics;
using System.Runtime.Remoting;

namespace Master
{
    public class MasterImpl : MarshalByRefObject, IMaster
    {

        private LookupTable lookupTable = new LookupTable();
        private ServerReplicasTable serverReplicasTable = new ServerReplicasTable();
        private delegate void AddTIDToPendingTableAsyncDelegate(string url, ulong tid, int startRange, int endRange);

        public override object InitializeLifetimeService()
        {
            return null;
        }

        //TODO passar funcao para LookupTable
        private int getIndex()
        {
            int table_size = lookupTable.Size();
            //Arrendondar para a potencia de 2 mais acima do numero tabela_size
            int end = (int)Math.Pow(2, Math.Ceiling(Math.Log(table_size) / Math.Log(2)));

            //Arrendondar para a potencia de 2 mais abaixo do numero tabela_size
            int start = (int)Math.Pow(2, (Math.Log(end) / Math.Log(2)) - 1);

            return start == 0 ? 0 : 2 * (table_size % start);
        }

        //Entradas/Saidas dos servidores de dados

        public bool UIDRangeTransferCompleted(string urlServerToConfirm)
        {
            return false;
        }

        public bool AddServer(string URL)
        {
            //Console.WriteLine("AddServer called");
            int index = getIndex();


            String primary_url = URL;
            String replica_url = String.Empty;
            
            UIDRange newUIDRange = null;

            if (lookupTable.Size() == 0)
            {
                //Primeiro servidor
                newUIDRange = lookupTable.DefaultUIDRange;

            }
            else
            {

                TableRow temp = lookupTable.GetRow(index);
                string oldPrimaryUrl = temp.GetServerPair().GetPrimary();
                newUIDRange = temp.GetUIDRange().Split();


                replica_url = serverReplicasTable.getReplica();

                IServer oldPrimaryServer = (IServer)Activator.GetObject(typeof(IServer), oldPrimaryUrl);
                ulong oldPrimaryTid = oldPrimaryServer.GetMaxTID();
                
                oldPrimaryServer.AddTIDToPendingTable(primary_url, oldPrimaryTid, newUIDRange.GetRangeStart(), newUIDRange.GetRangeEnd());

                IServer newPrimaryServer = (IServer)Activator.GetObject(typeof(IServer), primary_url);
                newPrimaryServer.Init(oldPrimaryTid, replica_url);


                //A tabela so temos o servidor 1 e entrou um novo srvidor 
                //como agora temos 2 servidores, o novo servidor pode 
                //ser replica do servidor 1

                if (lookupTable.Size() == 1)
                {
                    //Como era o unico servidor no sistema nao tinha replica
                    //adiciona-mos a replica agora que entrou um segundo
                    //servidor, e este o que entrou (primary_url) e a replica
                    //do anterior

                    //outlink
                    ServerPair sp = new ServerPair(temp.GetServerPair().GetPrimary(), primary_url);
                    lookupTable.ReplaceRow(index, new TableRow(sp, temp.GetUIDRange()));

                    //inlink
                    serverReplicasTable.addReplicaToServer(primary_url, temp.GetServerPair().GetPrimary());

                    oldPrimaryServer.SetReplica(primary_url);

                }

            }
            
     

            //outlinks
            ServerPair newServerPair = new ServerPair(primary_url, replica_url);
            TableRow newTableRow = new TableRow(newServerPair, newUIDRange);
            lookupTable.InsertRow(index, newTableRow);

            //inlinks
            //primary_url e adicionado como candidato para 
            //ser escolhido como replica de qualquer outro
            //servidor
            serverReplicasTable.addPrimary(primary_url);

            //Se existir replica disponivel
            //replica_url e responsavel por primary_url
            if (!replica_url.Equals(String.Empty))
                serverReplicasTable.addReplicaToServer(replica_url, primary_url);

            return true;

        }

        public bool RemoveServer(string url)
        {
            Console.WriteLine("RemoveServer");

            bool isPrimary = lookupTable.IsPrimary(url);

            Console.WriteLine("isPrimary: " + isPrimary);


            ////Falhou deve remover server e replica
            //List<String> replicatedServers = serverReplicasTable.getReplicasFromServer(url);

            //if (replicatedServers.Count != 0)
            //{
            //    //NOTIFICAR DA ALTERACAO

            //    foreach (String replicatedServer in replicatedServers)
            //    {
            //        IServer server = (IServer)Activator.GetObject(typeof(IServer), replicatedServer);
            //        //AddTIDToPendingTable(string url, ulong tid, int startRange, int endRange)
            //        AddTIDToPendingTableAsyncDelegate remoteDel = new AddTIDToPendingTableAsyncDelegate(server.AddTIDToPendingTable);
            //        UIDRange primaryRange = lookupTable.GetRowGivenPrimary(replicatedServer).GetUIDRange();
            //        remoteDel.BeginInvoke(newReplica, server.GetMaxTID(), primaryRange.GetRangeStart(), primaryRange.GetRangeEnd(), null, null);
            //    }
            //}

            //IServer newPrimaryServer = (IServer)Activator.GetObject(typeof(IServer), newPrimary);
            //AddTIDToPendingTableAsyncDelegate primaryServerDel = new AddTIDToPendingTableAsyncDelegate(newPrimaryServer.AddTIDToPendingTable);
            //UIDRange newPrimaryRange = lookupTable.GetRowGivenPrimary(newPrimary).GetUIDRange();
            //primaryServerDel.BeginInvoke(newReplica, newPrimaryServer.GetMaxTID(), newPrimaryRange.GetRangeStart(), newPrimaryRange.GetRangeEnd(), null, null);
        
            //Console.WriteLine("Removeu! ");

            serverReplicasTable.removeServerAndReplicas(url);
            String newReplica = serverReplicasTable.getReplica();
           
            Console.WriteLine("newReplica: " + newReplica);

            String newPrimary = null;


            //if the failed server is a primary server we request another replica and set replica as primary
            if (isPrimary) {
                
                lookupTable.SwapPrimaryReplica(url, newReplica);
                Console.WriteLine("swap.");

                newPrimary = lookupTable.GetRowGivenReplica(newReplica).GetServerPair().GetReplica();
                
                Console.WriteLine("newPrimary");

             //if the failed server is a replica we get a new replica 
            } else {
                
                lookupTable.SetNewReplica(url, newReplica);
                //Console.WriteLine("setNewReplica.");

                TableRow r = lookupTable.GetRowGivenReplica(newReplica);

                Console.WriteLine(r.ToString());


                ServerPair sp = r.GetServerPair();
                newPrimary = sp.GetPrimary();

                Console.WriteLine("newPrimary");
                //LANCAR EXCEPCAO CASO HAJA FALHA

            }

          
            return true;
            //throw new NotImplementedException();
        }

        //Get endpoints
        public string GetPrimaryEndpoint(int uid)
        {
            ServerPair pair = lookupTable.GetServerPair(uid);

            if (pair == null)
                return null;

            return pair.GetPrimary();
        }

        public string GetReplicaEndpoint(int uid)
        {

            ServerPair pair = lookupTable.GetServerPair(uid);

            return pair == null ? null : pair.GetReplica();

        }

        //Falhas
        public void PrimaryFailed(string URL)
        {
            throw new NotImplementedException();
        }

        public void ReplicaFailed(string URL)
        {
            throw new NotImplementedException();
        }

        public bool Status()
        {

            bool result = true;

            foreach (string primaryServer in serverReplicasTable.getServers())
            {
                try
                {
                    IServer server = (IServer)Activator.GetObject(typeof(IServer), primaryServer);
                    Console.WriteLine(primaryServer);
                    result = server.Status();
                }
                catch (Exception e)
                {
                    result = false;
                }
            }

            //numero de replicas 
            //numero de servers activos
            Console.WriteLine(lookupTable.ToString());
            Console.WriteLine(serverReplicasTable.ToString());

            return result;

        }

        public bool Fail()
        {
            throw new NotImplementedException();
        }

        public bool Freeze()
        {
            throw new NotImplementedException();
        }

        public bool Recover()
        {
            throw new NotImplementedException();
        }
    }


}
