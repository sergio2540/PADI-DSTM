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

namespace Server
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

    public class ServerApp
    {
        public static bool inFailMode = false;
        public static bool inFreezeMode = false;
        public static EventWaitHandle frozenCalls =  new EventWaitHandle(true, EventResetMode.ManualReset);

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

                } catch(Exception e){
                   
                
                }


            
            }
            ChannelServices.RegisterChannel(channel, true);

            

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerImpl), "Server", WellKnownObjectMode.Singleton);

            Console.WriteLine(String.Format("Server App - Listening for requests in port {0}.",port));

            Console.WriteLine("Press enter to exit...");
            //Console.ReadLine();
            
            String master_endpoint = "tcp://localhost:8080/Master";
            while(true){
               

                    IMaster master = (IMaster)Activator.GetObject(typeof(IMaster), master_endpoint);
                    
                    master.AddServer(String.Format("tcp://localhost:{0}/Server", port));

                   while(true) {
                   
                   }
                    
           
            }
            /*
            ServerImpl receiver = new ServerImpl();
            RemotingServices.Marshal((MarshalByRefObject) receiver, "Server", typeof(ServerImpl)); 
             */
        
        }

    }
}
