using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerApp
    {



        public static void Main(String[] args) { 

            TcpChannel channel = new TcpChannel(8086); 
            ChannelServices.RegisterChannel(channel,true);
            ServerImpl receiver = new ServerImpl();
            RemotingServices.Marshal((MarshalByRefObject) receiver, "Server", typeof(ServerImpl)); 

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerImpl),"Server", WellKnownObjectMode.SingleCall);
        
        }

    }
}
