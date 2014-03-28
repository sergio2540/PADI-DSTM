using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace DSTMServices
{
    public class DataService
    {
        //Interacção com o servidor e feita nesta classe


        public IServer InteractWithServer(String endpoint)
        {//nao sei se e supost chamar isto a cada interacçao. lease?
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            IServer obj = (IServer)Activator.GetObject(typeof(IServer), endpoint);
            
            if (obj == null)
            {
                System.Console.WriteLine("Could not locate server");
                return null;
            }
            return obj;
        }

        public DataService()
        {

        }    

        public PadInt CreatePadInt(int uid)
        {
            //Master
            //Server
            //Server.Create
            //padInt
            return null;

        }

        public PadInt AccessPadInt(int uid)
        {
            return null;
        }

    }
}