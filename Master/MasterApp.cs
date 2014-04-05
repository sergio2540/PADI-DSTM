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
        public static String debug = null;

        static void Main(string[] args)
        {
            //como escolher o ip onde se liga? o tcpchannel?
            int port = 8080;
            TcpChannel channel = new TcpChannel(port);
            
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MasterImpl), "Master", WellKnownObjectMode.SingleCall);

           
            Console.WriteLine(String.Format("Master App - Listening for requests in port {0}.",port));
            Console.WriteLine("Press enter to exit...");


            while (true)
            {
                if (debug != null)
                {
                    Console.WriteLine(debug);
                    debug = null;
                }
            }

            Console.ReadLine();

        }




    }
}
