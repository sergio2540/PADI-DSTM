using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
    class ServerReplicasTable
    {
        private List<string> availableReplicas = new List<string>();

        private Dictionary<string, List<string>> serverReplicas = new Dictionary<string, List<string>>();

        public ServerReplicasTable() {}


        
        public string getReplica()
        {

            List<string> candidates = new List<string>();
            int replicatedIntervalsNo = 0;
            int min = int.MaxValue;
            string replica = String.Empty;

            foreach(KeyValuePair<String,List<String>> kv in serverReplicas) {

                replicatedIntervalsNo = kv.Value.Count;

                if (kv.Value.Count < min) {
                    min = replicatedIntervalsNo;
                    replica = kv.Key;
                }
            }
        

            return replica;
        }
        

        /*
        public string getReplica()
        {
            if (availableReplicas.Count == 0)
            {
                //Se ja foram todos os servidores usados como replicas 
                //Escolhe novos
                availableReplicas = serverReplicas.Keys.ToList();
            }

            return replica;
        }
        */
        
        public void addReplicaToServer(string server, string replica) {

            if (serverReplicas.ContainsKey(server))
            {
                serverReplicas[server].Add(replica);
            }
            else 
            {
                serverReplicas.Add(server, new List<string>());
                serverReplicas[server].Add(replica);
            }

        }

        public void addPrimary(string server) {
            if (!serverReplicas.ContainsKey(server)) {
                serverReplicas.Add(server, new List<string>());
            }
        }

        public void changePrimary(string server1, string server2) {
            
        }

        public void removeReplicaFromServer(string server, string replica) {

            if (serverReplicas.ContainsKey(server))
            {
                serverReplicas[server].Remove(replica);
            }

        }

        public List<string> getReplicasFromServer(string server) {

            if (serverReplicas.ContainsKey(server)) { 
                return serverReplicas[server];
            } 

            return null;

        }

        public ICollection<string> getServers() {
            return serverReplicas.Keys;
        } 

        public void removeServerAndReplicas(string server) {

            if (serverReplicas.ContainsKey(server)) {
                serverReplicas.Remove(server);
            } 

        }

        public void removeAllReplicasFromServer(string server) {

            if (serverReplicas.ContainsKey(server))
            {
                serverReplicas[server].Clear();
            } 

        }

        public string ToString() { 

            string table = String.Empty;
            table += "Server\t\t\t\t\tReplica\n";
            foreach(KeyValuePair<string, List<string>> keyValue in serverReplicas) {
                foreach(string replica in keyValue.Value) {
                    table += keyValue.Key;
                    table += "\t";
                    table += replica;                
                }
            }
            
            return table;
        
        }


    }
}
