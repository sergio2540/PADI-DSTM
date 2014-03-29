using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using CommonTypes;
using Services;

namespace DSTMServices
{
     

    class Participant {
         
           private ulong tid;

           private String endpoint;
           private IServer server;
          
           private List<int> uids;

           Participant(ulong tid, String endpoint)
           {
                this.tid = tid;
                this.endpoint = endpoint;
                //url = ....procura no master
                //adicionar ao dicionario de strings e de referencias.
                TcpChannel channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, true);
                IServer serverRef = (IServer)Activator.GetObject(typeof(IServer), endpoint);
                this.server = serverRef;
           }
           
           
          
     }



    public class CoordinatorService
    {
        public IServer InteractWithServer(int uid)
        {//nao sei se e supost chamar isto a cada interacçao. lease

            String endpoint = lookupService.(uid);
            IServer serverRef = null;

            if (!uidServerRefAssociation.ContainsKey(uid)) //verifica se temos referencia para o servidor que guarda o uid.nao temos
            {
                dataService.getEndpoint(uid)
                Participant partipant = new Participant()
                uidServerAssociation[uid] = endpoint;
                uidServerRefAssociation[uid] = serverRef;

        
                //temos de criar transaccao se o coordenador ainda não existir


                if (serverRef == null)
                {
                    System.Console.WriteLine("Could not locate server");
                    return null;
                }


                 foreach(String server in uidServerAssociation.Values)
                {
                    if(server.Equals(endpoint))
                        return serverRef; 

                }

                bool canBegin = serverRef.BeginTransaction(currentTransactionId,"");
                if (canBegin)
                    return serverRef;
                else return null;
            }
            else return uidServerRefAssociation[uid];
           
        }

        private ulong currentTransactionId = 0;
        
        private Dictionary<int,Participant> participantes;

        private LookupService lookupService;
        
        //saber quem tem int
        //private Dictionary<int, String> ServersEndpoint;//vao haver colisões
        private Dictionary<int, IServer> uidServerRefAssociation;

        /*
         
            class Participant{
         *  
         *  String endpoint;
         *  IServer endpoint;
         *  Integer[] uids;
         * 
         * 
         * }
         
         
         
         */

        //Transaccoes
        public bool Begin(ulong transactionId) {

            //uidServerAssociation = new Dictionary<int, String>();
            uidServerRefAssociation = new Dictionary<int, IServer>();
            
           

            currentTransactionId = transactionId;

            lookupService = new LookupService();
            return true;
        }

        public bool Commit()//metodo deve ser repensado. pessimo codigo!!!!!!!!!!!!!!!!!!!!!!!!!
        {


            //Dictionary<String,int> getParticipantesEndpoints(uid);

            Dictionary<String, int> endpointUidAssociation = new Dictionary<String, int>();

            bool decision = true;

            foreach (KeyValuePair<int, String> entry in uidServerAssociation)
                if(!endpointUidAssociation.ContainsKey(entry.Value))
                    endpointUidAssociation[entry.Value] = entry.Key;

            foreach (KeyValuePair<String, int> entry in endpointUidAssociation)
                decision &= uidServerRefAssociation[entry.Value].canCommit(currentTransactionId);  //verificar se respondeu.usar metodo ping.isto funciona assumindo que respondem

            if(decision == true)
                foreach (KeyValuePair<String, int> entry in endpointUidAssociation)
                decision &= uidServerRefAssociation[entry.Value].doCommit(currentTransactionId);

            else
                foreach (KeyValuePair<String, int> entry in endpointUidAssociation)
                    decision &= uidServerRefAssociation[entry.Value].doAbort(currentTransactionId);
            return decision;
        }

        public bool Abort()
        {
            Dictionary<String, int> endpointUidAssociation = new Dictionary<String, int>();
            bool result = true;

            foreach (KeyValuePair<int, String> entry in uidServerAssociation)
                if (!endpointUidAssociation.ContainsKey(entry.Value))
                    endpointUidAssociation[entry.Value] = entry.Key;

            foreach (KeyValuePair<String, int> entry in endpointUidAssociation)
                result &= uidServerRefAssociation[entry.Value].doAbort(currentTransactionId);

            return result;

        }

        public IServer InteractWithServer(int uid)
        {
            
            //nao sei se e supost chamar isto a cada interacçao. lease

            //get endpoint
            String endpoint = lookupService.getServerEndpoint(uid);
            IServer server = lookupService.getServer(uid);

            IServer serverRef = null;

            if (!uidServerRefAssociation.ContainsKey(uid)) //verifica se temos referencia para o servidor que guarda o uid.nao temos
            {
                //url = ....procura no master
                //adicionar ao dicionario de strings e de referencias.
                TcpChannel channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, true);
                serverRef = (IServer)Activator.GetObject(typeof(IServer), endpoint);
                uidServerAssociation[uid] = endpoint;
                uidServerRefAssociation[uid] = serverRef;

        
                //temos de criar transaccao se o coordenador ainda não existir


                if (serverRef == null)
                {
                    System.Console.WriteLine("Could not locate server");
                    return null;
                }


                foreach(String server in uidServerAssociation.Values)
                {
                    if(server.Equals(endpoint))
                        return serverRef; 

                }

                bool canBegin = serverRef.BeginTransaction(currentTransactionId,"");
                if (canBegin)
                    return serverRef;
                else return null;
            }
            else return uidServerRefAssociation[uid];
           
        }


        //PadInt
        public int ReadPadInt(int uid) { 
            
            //remoting para server
           // String serverUrl = null;
            //if (!intServerMapping.ContainsKey(uid))
                //throw new NotImplementedException();
            //procura pelo servidor, adiciona ao mappeamento e busca. Adicionar evento, para detectar mudanças.
            //else serverUrl = intServerMapping[uid];
            
            getServer
            IServer serverRef = InteractWithServer(uid);
            if (serverRef == null){
                System.Console.WriteLine("Coordinator failed to connect to server. Null reference returned.");//pode acontecer que nao seja possivel criar transaccao
                return -1;//era inteligente lançar uma excepção caso nao de para ligar e outra caso nao exista.
            }
            
            
            int answerValue = serverRef.ReadPadInt(currentTransactionId, uid);//quantas instancias de coordenador deveriam haver?uma transaccao de cada vez.
           
            
            return answerValue;
          

            //return null;
        
        }



        private void WritePadInt(int uid, int value)
        {
            
           // String serverUrl = null;
            //if(!intServerMapping.ContainsKey(uid))
              //   throw new NotImplementedException();
                //procura pelo servidor, adiciona ao mappeamento e busca. Adicionar evento, para detectar mudanças.se falhar procura p
            //else //faz chamada.                 //remoting para server. verificar se existe valor
                //serverUrl = intServerMapping[uid];    
                
                IServer serverRef = InteractWithServer(uid);
                if (serverRef == null)
                {
                    System.Console.WriteLine("Coordinator failed to connect to server. Null reference returned.");
                    return;//era inteligente lançar uma excepção caso nao de para ligar e outra caso nao exista.
                    //o master tem de ser consultado
                }
                serverRef.WritePadInt(currentTransactionId,uid,value);
             
        }

        public PadInt CreatePadInt(int uid)
        {
            Console.WriteLine("CreatePadint called with: " + uid);
            IServer serverRef = InteractWithServer(uid);


            PadInt padInt = serverRef.CreatePadInt(currentTransactionId,uid);

            if (padInt == null)
                return null;
            padInt.Write(0);

            PadIntLocal localP = new PadIntLocal(uid);
            localP.Write(0);

            localP.changeHandler += this.OnPadintChange;
            localP.readHandler += this.OnPadintRead;
            return localP;

        }

        public PadInt AccessPadInt(int uid)
        {
            int padintValue = ReadPadInt(uid);
            PadIntLocal padInt = new PadIntLocal(uid);
            padInt.Write(padintValue);
            padInt.changeHandler += this.OnPadintChange;
            padInt.readHandler += this.OnPadintRead;
            return padInt;
        }

        public void OnPadintChange(Object sender ,EventArgs e) {
            Console.WriteLine("change event  called!");

            PadIntLocal changedPadInt = (PadIntLocal) sender;
            changedPadInt.readHandler -= OnPadintRead;
            WritePadInt(changedPadInt.Uid,changedPadInt.Read());
            changedPadInt.readHandler += OnPadintRead;

        }

        public void OnPadintRead(Object sender, EventArgs e)
        {
            PadIntLocal oldPadint = (PadIntLocal)sender;
            int updatedValue = ReadPadInt(oldPadint.Uid);
            Console.WriteLine(updatedValue);
            oldPadint.changeHandler -= OnPadintChange;
            oldPadint.Write(updatedValue);
            oldPadint.changeHandler += OnPadintChange;

        }


        
    }
}
