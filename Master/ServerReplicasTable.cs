using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
    class ServerReplicasTable
    {

        private Dictionary<string, List<string>> serverReplicas = new Dictionary<string, List<string>>();

        public ServerReplicasTable() {}

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
