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
        public static String debug = null;
        public static void Main(String[] args) {

            int port = int.Parse(args[0]);

            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerImpl), "Server", WellKnownObjectMode.Singleton);

            Console.WriteLine("Server App - Listening for requests.");
            Console.WriteLine("Press enter to exit...");

            while (true)
            {
                if (debug != null)
                {
                    Console.WriteLine(debug);
                    debug = null;
                }
            }

            /*
            ServerImpl receiver = new ServerImpl();
            RemotingServices.Marshal((MarshalByRefObject) receiver, "Server", typeof(ServerImpl)); 
             */
        
        }

    }
}
