using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    public class Coordinator
    {
        private long currentTransactionId = 0;
        //saber quem tem int
        private Dictionary<int, String> intServerMapping;


        public IServer InteractWithServer(String endpoint){//nao sei se e supost chamar isto a cada interacçao. lease?
            TcpChannel channel = new TcpChannel(); 
            ChannelServices.RegisterChannel(channel,true);
            IServer obj = (IServer)Activator.GetObject(typeof(IServer), endpoint); 
            if (obj == null){ 
                System.Console.WriteLine("Could not locate server");
                return null;
            }
            return obj;
        }

        public void BeginTransaction(long transactionId) {
            intServerMapping = new Dictionary<int, String>();
            currentTransactionId = transactionId;
        }

        public PadInt ReadPadInt(int uid) { 
            //remoting para server
            String serverUrl = null;
            if (!intServerMapping.ContainsKey(uid))
                throw new NotImplementedException();
            //procura pelo servidor, adiciona ao mappeamento e busca. Adicionar evento, para detectar mudanças.
            else serverUrl = intServerMapping[uid];

            IServer serverRef = InteractWithServer(serverUrl);
            if (serverRef == null){
                System.Console.WriteLine("Coordinator failed to connect to server. Null reference returned.");
                return null;//era inteligente lançar uma excepção caso nao de para ligar e outra caso nao exista.
            }

            int answerValue = serverRef.ReadPadint(currentTransactionId, uid);//quantas instancias de coordenador deveriam haver?uma transaccao de cada vez.
            PadInt padInt = new PadInt(answerValue);
            padInt.changeHandler += this.OnPadintChange;
            return padInt;
          
        
        }

        private void WritePadInt(int uid, int value)
        {
            String serverUrl = null;
            if(!intServerMapping.ContainsKey(uid))
                 throw new NotImplementedException();
                //procura pelo servidor, adiciona ao mappeamento e busca. Adicionar evento, para detectar mudanças.se falhar procura p
            else //faz chamada.                 //remoting para server. verificar se existe valor
                serverUrl = intServerMapping[uid];    
                
                IServer serverRef = InteractWithServer(serverUrl);
                if (serverRef == null)
                {
                    System.Console.WriteLine("Coordinator failed to connect to server. Null reference returned.");
                    return;//era inteligente lançar uma excepção caso nao de para ligar e outra caso nao exista.
                    //o master tem de ser consultado
                }
                serverRef.WritePadInt(currentTransactionId,uid,value);
        }

        public void OnPadintChange(Object sender ,EventArgs e) {
            PadInt changedPadInt = (PadInt) sender;
            WritePadInt(changedPadInt.Uid,changedPadInt.Read());
        }

        
    }
}
