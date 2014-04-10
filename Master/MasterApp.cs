using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace Master
{

    /*
    private static string GetIP()
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();
            Console.WriteLine(strHostName);

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

            string ipaddress = Convert.ToString(ipEntry.AddressList[2]);

            return ipaddress.ToString();
        }
    */

    public class MasterApp
    {

        public static void Main(string[] args)
        {
            //como escolher o ip onde se liga? o tcpchannel?
            int port = 8080;
            TcpChannel channel = new TcpChannel(port);
            
            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MasterImpl), "Master", WellKnownObjectMode.Singleton);
           
            Console.WriteLine(String.Format("Master App - Listening for requests in port {0}.",port));
            Console.WriteLine("Press enter to exit...");
            Console.WriteLine(channel.GetUrlsForUri("Master")[0]);
            Console.ReadLine();

        }




    }
}
