using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;

namespace DSTMServices
{
    public class LookupService
    {
        Dictionary<int, String> endpoints = new Dictionary<int, string>();
         
        Dictionary<int, IServer> servers = new Dictionary<int, IServer>();
        
        public LookupService()
        {
            //Chave uid
            endpoints[1] = "tcp://localhost:8086/Server";
            //endpoints[2] = "tcp://localhost:8086/Server";
            
        }

        public Dictionary<int, String> GetEndpoints()
        {
            return endpoints;
        }

        public Dictionary<int, IServer> GetServers()
        {
            return servers;
        }

        public List<IServer> GetParticipants()
        {
            Dictionary<String, int> serversEndpoints = new Dictionary<String, int>();
            
            List<IServer> participants = new List<IServer>();

            foreach (KeyValuePair<int, String> entry in endpoints)
                if (!serversEndpoints.ContainsKey(entry.Value))
                    serversEndpoints[entry.Value] = entry.Key;

            foreach (KeyValuePair<String, int> entry in serversEndpoints)
                participants.Add(servers[entry.Value]);

            return participants;
        }




        private String GetServerEndpoint(int uid)
        {
            return endpoints[uid];
        }

        public IServer GetServer(ulong tid, int uid)
        {
            
            if (!servers.ContainsKey(uid))
            {
                //Nao existe servidor

                String endpoint = GetServerEndpoint(uid);
                
                //url = ....procura no master
                //adicionar ao dicionario de strings e de referencias.

              
                IServer server = (IServer)Activator.GetObject(typeof(IServer), endpoint);
                
                if (server == null)
                {
                    return null;
                }

                Console.WriteLine("About to begin transaction!");

                bool canBegin = server.BeginTransaction(tid, "");

                //adiciona a cache
                servers[uid] = server;

                if (canBegin)
                    return server;
                else return null;

            }
            //Ja existe na cache
            else {
                return servers[uid];
            }

        }

        

    }
}
