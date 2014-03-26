using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Server
{
    class ServerApp
    {
        public static void Main(String[] args) {


            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerImpl), "Server", WellKnownObjectMode.SingleCall);

            Console.WriteLine("Server App - Listening for requests.");
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();

            /*
            ServerImpl receiver = new ServerImpl();
            RemotingServices.Marshal((MarshalByRefObject) receiver, "Server", typeof(ServerImpl)); 
             */
        
        }

    }
}
