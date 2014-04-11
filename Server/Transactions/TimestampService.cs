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

            private static string GetIp()
            {
                string strHostName = System.Net.Dns.GetHostName();
                IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
                IPAddress ipv4Addresses = Array.FindLast(ipEntry.AddressList, x => x.AddressFamily == AddressFamily.InterNetwork);
                return ipv4Addresses.ToString();

            }

            public ulong getTimestamp()
            {
                
                long beginTicks = new DateTime(2014, 1, 1).Ticks;
                long endTicks = DateTime.UtcNow.Ticks;

                long elapsedTicks = endTicks - beginTicks;

                TimeSpan elapsed = new TimeSpan(elapsedTicks);

                ulong t = (ulong) elapsed.TotalMilliseconds;

                string localIp = GetIp();

                byte[] bytes = IPAddress.Parse(localIp).GetAddressBytes();

                Array.Reverse(bytes);

                uint ip = BitConverter.ToUInt32(bytes, 0);

                ulong timestamp = (ulong) t << 32 | ip;

                Console.WriteLine("time {0}, server-id {1}, timestamp-service {2}", t, ip,timestamp);

                return timestamp;

            }
        }
    
}
