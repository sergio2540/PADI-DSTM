using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using CommonTypes;
using Services;
using DSTMServices;

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
        private MasterService masterService;

        //saber quem tem int
        //private Dictionary<int, String> ServersEndpoint;//vao haver colisões
        //private Dictionary<int, IServer> uidServerRefAssociation;
        //private Dictionary<int, String>  uidServerAssociation = new Dictionary<int, String>();

        public CoordinatorService(MasterService masterS)
        {
            masterService = masterS;
            //???!!!
            lookupService = new LookupService(masterService);



            //uidServerAssociation = new Dictionary<int, String>();
            //uidServerRefAssociation = new Dictionary<int, IServer>();

        }

        //Transaccoes
        //public bool Begin(ulong transactionId)
        public bool Begin()
        {
            //uidServerAssociation = new Dictionary<int, String>();//-->idealmente isto deveria ser aqui, mas por agora fica no constructor para termos estado.
            //uidServerRefAssociation = new Dictionary<int, IServer>();
            //uidServerAssociation[1] = "tcp://localhost:8086/Server";
            //uidServerAssociation[2] = "tcp://localhost:8086/Server";

            //String master_endpoint = "tcp://localhost:8080/Master";



            //?????????!!!!!!!!!!!
            //lookupService = new LookupService(masterService);

            currentTid = 0;
            lookupService.ResetService();

            //currentTid = transactionId;


            return true;
        }

        public bool Commit()//metodo deve ser repensado. pessimo codigo!!!!!!!!!!!!!!!!!!!!!!!!!
        {


            //Dictionary<String,int> getParticipantesEndpoints(uid);


            bool decision = true;

            List<IServer> participants = lookupService.GetParticipants();

            //Se ligaçao falha, aborta-se a transacção.
            try
            {
                foreach (IServer participant in participants)
                {
                    decision &= participant.canCommit(currentTid);  //verificar se respondeu.usar metodo ping.isto funciona assumindo que respondem
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to do canCommit!. Aborting now...with message: " + e.Message);
                decision = false;

            }

            bool result = true;

            if (decision == true)
            {
                foreach (IServer participant in participants)
                    try
                    {
                        result &= participant.doCommit(currentTid);
                    }
                    catch (SocketException e)
                    {
                        result = false;
                        continue;
                    }
            }
            else
            {
                foreach (IServer participant in participants)
                    try
                    {
                        result &= participant.doAbort(currentTid);
                    }
                    catch (SocketException e)
                    {
                        result = false;
                        continue;
                    }
            }

            currentTid = 0;
            lookupService.ResetService();
            return result;
        }

        public bool Abort()
        {
            bool result = true;
            List<IServer> participants = lookupService.GetParticipants();
            foreach (IServer participant in participants)
                try
                {
                    result &= participant.doAbort(currentTid);
                }
                catch (SocketException e)
                {
                    result = false;
                    continue;
                }

            currentTid = 0;
            lookupService.ResetService();
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
            //canBegin = serverRef.BeginTransaction(currentTid, ""); // verificar se lança excepcao.

            IServer serverRef = lookupService.GetServer(uid);

            if (currentTid == 0)
            {
                currentTid = serverRef.GetTid();
            }

            //bool canBegin = false; //= server.BeginTransaction(tid, "");

            //canBegin = serverRef.BeginTransaction(currentTid, ""); // verificar se lança excepcao.

            //se a transaccao tem tid 0, acabou de começar. 


            if (serverRef == null)
            {
                System.Console.WriteLine("Coordinator failed to connect to server. Null reference returned.");//pode acontecer que nao seja possivel criar transaccao
                return -1;//era inteligente lançar uma excepção caso nao de para ligar e outra caso nao exista.
            }


            lookupService.AddParticipant(currentTid, uid);
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

            IServer serverRef = lookupService.GetServer(uid);

            if (currentTid == 0)
            {
                currentTid = serverRef.GetTid();
            }

            if (serverRef == null)/////////////////////////////////////////////////////////////////////////////////////////<TER EM ATENCAO QUE PODE FALHAR!!!!!!!!!!!!!!!!!!!!!!!!!!>
            {
                System.Console.WriteLine("Coordinator failed to connect to server. Null reference returned.");
                return;//era inteligente lançar uma excepção caso nao de para ligar e outra caso nao exista.
                //o master tem de ser consultado
            }


            lookupService.AddParticipant(currentTid, uid);
            serverRef.WritePadInt(currentTid, uid, value);


        }

        public PadInt CreatePadInt(int uid)
        {

            IServer server = lookupService.GetServer(uid);
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

            IServer server = lookupService.GetServer(uid);
            PadInt remote = server.AccessPadInt(currentTid, uid);

            if (remote == null)
                return null;

            int value = remote.Read();

            PadIntLocal local = new PadIntLocal(uid, value);

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