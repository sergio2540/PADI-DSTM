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

namespace Server
{   
    
    
      

    public class ServerApp
    {
        public static bool inFailMode = false;
        public static bool inFreezeMode = false;
        public static EventWaitHandle frozenCalls =  new EventWaitHandle(true, EventResetMode.ManualReset);


        private static string GetIp()
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();
            Console.WriteLine(strHostName);

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

            string ipaddress = Convert.ToString(ipEntry.AddressList[2]);

            return ipaddress.ToString();
        }
     
     

        public static void Main(String[] args) {

            //int port = int.Parse(args[0]);
            //int port = 8089;
            Random r = new Random();
            int port = r.Next(8081, 9000);
            //int port = 8089;
            TcpChannel channel = null;

            while (true) {
                port = r.Next(8081,9000);

                try
                {
                    channel = new TcpChannel(port);
                    break;

                }catch(Exception e){
                   
                
                }


            
            }
            ChannelServices.RegisterChannel(channel, false);

            

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerImpl), "Server", WellKnownObjectMode.Singleton);

            Console.WriteLine(String.Format("Server App - Listening for requests in port {0}.",port));

            Console.WriteLine("Press enter to exit...");
            //Console.ReadLine();

            String[] ipFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), @"IpConf.txt"));
            String ip = ipFile[0].Replace(System.Environment.NewLine, String.Empty);
           
            String master_endpoint = "tcp://" + ip + ":" + "8080/Master";
            String server_endpoint = String.Format("tcp://{0}:{1}/Server", GetIp(), port);
            Console.WriteLine("Server endpoint:" + server_endpoint);
            
            while(true) {
                try
                {

                    IMaster master = (IMaster)Activator.GetObject(typeof(IMaster), master_endpoint);
                    
                    master.AddServer(server_endpoint);
                    Console.WriteLine("Connection to Master at: " + master_endpoint);
                   while(true) {
                   }
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception message:" + e.Message);
                    //Tenta ligar novamente
                  // Console.WriteLine("A tentar liga);
                }
            }
            /*
            ServerImpl receiver = new ServerImpl();
            RemotingServices.Marshal((MarshalByRefObject) receiver, "Server", typeof(ServerImpl)); 
             */
        
        }

    }
}
