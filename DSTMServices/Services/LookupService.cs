﻿using System;
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

        private HashSet<String> participants = new HashSet<String>();

    
        
        public LookupService(MasterService masterEndpointService)
        {
            masterService = masterEndpointService;
            master = masterService.Master;
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

            /*
            Dictionary<String, int> serversEndpoints = new Dictionary<String, int>();
            
            List<IServer> participants = new List<IServer>();

            foreach (KeyValuePair<int, String> entry in endpoints)
                if (!serversEndpoints.ContainsKey(entry.Value))
                    serversEndpoints[entry.Value] = entry.Key;

            foreach (KeyValuePair<String, int> entry in serversEndpoints)
                if(servers.ContainsKey(entry.Value)) //Deve ser removido!!!!
                    participants.Add(servers[entry.Value]);
            */

            
            HashSet<int> uids = new HashSet<int>();
            
            //List<IServer> participants = new List<IServer>();
            foreach (KeyValuePair<int, String> endpoint in endpoints)
                if (participants.Contains(endpoint.Value))
                    uids.Add(endpoint.Key);

            List<IServer> particip = new List<IServer>();

            foreach (int uid in uids)
                particip.Add(servers[uid]);
           


            return particip; //acontece que quando inverto as chaves passam a ser urls. os dubplicados são eliminados. 
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
            
            return endpoints[uid];
        }

        public IServer GetServer(int uid)
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
                  if(e.Value.Equals(endpoint) && (servers.ContainsKey(e.Key))) //ja existe url e ja existe referencia.
                    return server; 

                }                

                Console.WriteLine("About to begin transaction!");

                //e se tivermos dois objectos diferentes mas que estao no mesmo server begin de novo?
                //bool canBegin = server.BeginTransaction(tid, "");

                //adiciona a cache
                servers[uid] = server;
                return server;

            }
            //Ja existe na cache
            else {
                return servers[uid];
            }

        }

        //begin se for a primeira vez que vai ao server x.

        public void AddParticipant(ulong currentTid, int uid) {
            IServer server = servers[uid];
            String url = endpoints[uid];
            if (!participants.Contains(url)) { 
                server.BeginTransaction(currentTid, "");
                participants.Add(url);
            }
        
        }
    }
}
