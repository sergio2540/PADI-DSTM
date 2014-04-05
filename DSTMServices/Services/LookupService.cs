using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;

using Master;
using DSTMServices;

namespace DSTMServices
{
    public class LookupService
    {
        
        //private IMaster master; recentemente comentado MasterService
        private IMaster master;
        private MasterService masterService;

        private Dictionary<int, String> endpoints = new Dictionary<int, string>();
         
        private Dictionary<int, IServer> servers = new Dictionary<int, IServer>();
        
        public LookupService(MasterService masterEndpointService)
        {
            masterService = masterEndpointService;
            master = masterService.Master;
            //master = (IMaster)Activator.GetObject(typeof(IMaster), endpoint); //recentemente comentado MasterService


            //Chave uid
           // endpoints[1] = "tcp://localhost:8086/Server";
           // endpoints[2] = "tcp://localhost:8086/Server";
            
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
                if(servers.ContainsKey(entry.Value)) //Deve ser removido!!!!
                    participants.Add(servers[entry.Value]);

            return participants; //acontece que quando inverto as chaves passam a ser urls. os dubplicados são eliminados. 
                                //õs urls nao podem ser estaticos. os urls so podem estar se tiverem sido usados. caso contrario temos o caso
                                //em que temos o url e um id, mas esse id não identifica nenhuma referencia IRef porque nunca foi adicionada
                                //no contexto desse uid.
        }




        private String GetServerEndpoint(int uid)
        {

            String endpoint = master.GetPrimaryEndpoint(uid);
            
            //master.GetReplicaEndpoint(uid);

            //Cache
            endpoints[uid] = endpoint;
            
            /*
            if(uid == 1)
                endpoints[1] = "tcp://localhost:8086/Server";
            else if(uid == 2)
                endpoints[2] = "tcp://localhost:8086/Server";
            else endpoints[3] = "tcp://localhost:8086/Server";
            */
            
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

                //este foreach so deve estar activo quando ainda não existirem endpoints por defeito.
                //Console.WriteLine("Vamos verificar se já ligámos a este server");

                foreach(KeyValuePair<int ,String> e in endpoints)
                {
                  if(e.Value.Equals(endpoint) && (servers.ContainsKey(e.Key)))
                    return server; 

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
