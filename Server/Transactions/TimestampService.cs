using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class TimestampService
    {


        public TimestampService()
        {

        }
            private string getLocalIp()
            {

                IPHostEntry host;
                string localIP = "";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
                return localIP;
            }

            public ulong getTimestamp()
            {

                //get clock
                
                long beginTicks = new DateTime(2014, 1, 1).Ticks;
                long endTicks = DateTime.UtcNow.Ticks;

                long elapsedTicks = endTicks - beginTicks;

                TimeSpan elapsed = new TimeSpan(elapsedTicks);

                long t = (long)elapsed.TotalMilliseconds;



                //get IP

                //Provavelmente deve ser o master a emitir um id
                
                string localIp = getLocalIp();

                uint ip = BitConverter.ToUInt32(IPAddress.Parse(localIp).GetAddressBytes(), 0);

                


                ulong timestamp = (ulong)t << 52 | ip;

                Console.WriteLine("time {0}, server-id {1}, timestamp-service {2}", t, ip,timestamp);

                return timestamp;
            }
        }
    
}
