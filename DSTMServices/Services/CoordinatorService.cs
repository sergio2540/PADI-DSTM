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
    public class CoordinatorService
    {
        private ulong currentTransactionId = 0;
        
        private DataService dataService;
        //saber quem tem int
        private Dictionary<int, String> uidServerAssociation;//vao haver colisões
        private Dictionary<int, IServer> uidServerRefAssociation;

        //Transaccoes
        public bool Begin(ulong transactionId) {
            uidServerAssociation = new Dictionary<int, String>();
            uidServerRefAssociation = new Dictionary<int, IServer>();
            uidServerAssociation[1] = "tcp://localhost:8086/Server";
            uidServerAssociation[2] = "tcp://localhost:8086/Server";
            currentTransactionId = transactionId;
            dataService = new DataService();
            return true;
        }

        public bool Commit()
        {
            throw new NotImplementedException();
           
        }

        public bool Abort()
        {
            throw new NotImplementedException();

        }

        public IServer InteractWithServer(int uid)
        {//nao sei se e supost chamar isto a cada interacçao. lease
            String endpoint = uidServerAssociation[uid];
            IServer serverRef = null;

            if (!uidServerRefAssociation.ContainsKey(uid))
            {
                //url = ....procura no master
                //adicionar ao dicionario de strings e de referencias.
                TcpChannel channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, true);
                serverRef = (IServer)Activator.GetObject(typeof(IServer), endpoint);
                uidServerAssociation[uid] = endpoint;
                uidServerRefAssociation[uid] = serverRef;

                if (serverRef == null)
                {
                    System.Console.WriteLine("Could not locate server");
                    return null;
                }
                return serverRef;
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

            IServer serverRef = InteractWithServer(uid);
            if (serverRef == null){
                System.Console.WriteLine("Coordinator failed to connect to server. Null reference returned.");
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
