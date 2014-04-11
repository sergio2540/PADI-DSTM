using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Master
{

   

    public class MasterApp
    {
        private static string GetIp()
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress ipv4Addresses = Array.FindLast(ipEntry.AddressList, x => x.AddressFamily == AddressFamily.InterNetwork);
            return ipv4Addresses.ToString();

        }

        public static void Main(string[] args)
        {
            //como escolher o ip onde se liga? o tcpchannel?
            int port = 8080;
            TcpChannel channel = new TcpChannel(port);
            
            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MasterImpl), "Master", WellKnownObjectMode.Singleton);

            Console.WriteLine("Master App - url: " + GetIp());
            Console.WriteLine("Press enter to exit...");
            Console.ReadKey();

        }




    }
}
