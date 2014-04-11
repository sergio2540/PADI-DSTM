using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using CommonTypes;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{

    public class ServerApp
    {
        public static bool inFailMode = false;
        public static bool inFreezeMode = false;
        public static EventWaitHandle frozenCalls = new EventWaitHandle(true, EventResetMode.ManualReset);


        private static string GetIp()
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress ipv4Addresses = Array.FindLast(ipEntry.AddressList, x => x.AddressFamily == AddressFamily.InterNetwork);
            return ipv4Addresses.ToString();

        }

        public static void Main(String[] args)
        {

            String master_ip = String.Empty;
            int master_port = 0;


            if (args.Length == 2)
            {

                master_ip = args[0];
                master_port = int.Parse(args[1]);

            }
            else
            {
                Console.WriteLine("Argumentos Invalidos.");
            }


            Random r = new Random();
            int server_port = 0;
            TcpChannel channel = null;

            while (true)
            {
                server_port = r.Next(8081, 9000);

                try
                {
                    channel = new TcpChannel(server_port);
                    break;

                }
                catch (Exception e)
                {


                }

            }

            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerImpl), "Server", WellKnownObjectMode.Singleton);

            //String[] ipFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), @"IpConf.txt"));
            //String ip = ipFile[0].Replace(System.Environment.NewLine, String.Empty);

            String master_endpoint = String.Format("tcp://{0}:{1}/Master", master_ip, master_port);
            string server_ip = GetIp();
            String server_endpoint = String.Format("tcp://{0}:{1}/Server", server_ip, server_port);

            while (true)
            {

                try
                {

                    IMaster master = (IMaster)Activator.GetObject(typeof(IMaster), master_endpoint);

                    master.AddServer(server_endpoint);

                    Console.WriteLine("Master App - url: " + master_endpoint);
                    Console.WriteLine("Success connection to Master.");

                    Console.WriteLine("Server App - url: " + server_endpoint);
                    Console.WriteLine("Server is up.");
                    Console.WriteLine("Press enter to exit...");
                    Console.ReadKey();

                }
                catch (Exception e)
                {


                    Console.WriteLine("Exception message:" + e.Message);
                    //Tenta ligar novamente
                    Console.WriteLine("Press enter to retry connection");
                    Console.ReadKey();

                }


            }


        }

    }
}
