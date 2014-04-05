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


    class Participant
    {

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

        private ulong currentTid = 0;

        //private Dictionary<int,Participant> participantes;

        private LookupService lookupService;

        //saber quem tem int
        //private Dictionary<int, String> ServersEndpoint;//vao haver colisões
        //private Dictionary<int, IServer> uidServerRefAssociation;
        //private Dictionary<int, String>  uidServerAssociation = new Dictionary<int, String>();

        public CoordinatorService()
        {

            //uidServerAssociation = new Dictionary<int, String>();
            //uidServerRefAssociation = new Dictionary<int, IServer>();

        }

        //Transaccoes
        public bool Begin(ulong transactionId)
        {
            //uidServerAssociation = new Dictionary<int, String>();//-->idealmente isto deveria ser aqui, mas por agora fica no constructor para termos estado.
            //uidServerRefAssociation = new Dictionary<int, IServer>();
            //uidServerAssociation[1] = "tcp://localhost:8086/Server";
            //uidServerAssociation[2] = "tcp://localhost:8086/Server";

            String master_endpoint = "tcp://localhost:8080/Master";
            lookupService = new LookupService(master_endpoint);
            currentTid = transactionId;


            return true;
        }

        public bool Commit()//metodo deve ser repensado. pessimo codigo!!!!!!!!!!!!!!!!!!!!!!!!!
        {


            //Dictionary<String,int> getParticipantesEndpoints(uid);


            bool decision = true;

            List<IServer> participants = lookupService.GetParticipants();

            foreach (IServer participant in participants)
            {
                Console.WriteLine("There is one particiapant!!!!!!!!!!!");
                decision &= participant.canCommit(currentTid);  //verificar se respondeu.usar metodo ping.isto funciona assumindo que respondem
            }
            bool result = true;

            if (decision == true)
            {
                Console.WriteLine("CAN COMMIT!!!!!");
                foreach (IServer participant in participants)
                    result &= participant.doCommit(currentTid);
            }

            else
            {

                Console.WriteLine("Is going to abort!!!!!");
                foreach (IServer participant in participants)
                    result &= participant.doAbort(currentTid);
            }
            return result;
        }

        public bool Abort()
        {
            bool result = true;
            List<IServer> participants = lookupService.GetParticipants();
            foreach (IServer participant in participants)
                result &= participant.doAbort(currentTid);

            return result;

        }




        //PadInt
        public int ReadPadInt(int uid)
        {

            //remoting para server
            // String serverUrl = null;
            //if (!intServerMapping.ContainsKey(uid))
            //throw new NotImplementedException();
            //procura pelo servidor, adiciona ao mappeamento e busca. Adicionar evento, para detectar mudanças.
            //else serverUrl = intServerMapping[uid];


            IServer serverRef = lookupService.GetServer(currentTid, uid);

            if (serverRef == null)
            {
                System.Console.WriteLine("Coordinator failed to connect to server. Null reference returned.");//pode acontecer que nao seja possivel criar transaccao
                return -1;//era inteligente lançar uma excepção caso nao de para ligar e outra caso nao exista.
            }


            int answerValue = serverRef.ReadPadInt(currentTid, uid);//quantas instancias de coordenador deveriam haver?uma transaccao de cada vez.


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

            IServer serverRef = lookupService.GetServer(currentTid, uid);

            if (serverRef == null)
            {
                System.Console.WriteLine("Coordinator failed to connect to server. Null reference returned.");
                return;//era inteligente lançar uma excepção caso nao de para ligar e outra caso nao exista.
                //o master tem de ser consultado
            }
            serverRef.WritePadInt(currentTid, uid, value);

        }

        public PadInt CreatePadInt(int uid)
        {
            Console.WriteLine("CreatePadint called with: " + uid);

            IServer server = lookupService.GetServer(currentTid, uid);
            PadInt padInt = server.CreatePadInt(currentTid, uid);

            if (padInt == null)
                return null;
           

            PadIntLocal local = new PadIntLocal(uid);

            if (local == null)
                return null;
            

            local.changeHandler += this.OnPadintChange;
            local.readHandler += this.OnPadintRead;

            return local;

        }

        public PadInt AccessPadInt(int uid)
        {

            IServer server = lookupService.GetServer(currentTid, uid);
            PadInt remote = server.AccessPadInt(currentTid, uid);
            
            if (remote == null)
            {
                return null;
            }
            
            int value = remote.Read();

            PadIntLocal local = new PadIntLocal(uid,value);
          

            local.changeHandler += this.OnPadintChange;
            local.readHandler += this.OnPadintRead;
            
            return local;
        
        }

        public void OnPadintChange(Object sender, EventArgs e)
        {
            Console.WriteLine("change event  called!");

            PadIntLocal changedPadInt = (PadIntLocal)sender;
            changedPadInt.readHandler -= OnPadintRead;
            WritePadInt(changedPadInt.Uid, changedPadInt.Read());
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