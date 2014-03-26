using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Master
{
    class MasterApp
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MasterImpl), "Master", WellKnownObjectMode.SingleCall);

            Console.WriteLine("Master App - Listening for requests.");
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}
