using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace DSTMServices
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
                uint clock = (uint)DateTime.Now.Millisecond;

                //get IP
                string localIp = getLocalIp();
                uint ip = BitConverter.ToUInt32(IPAddress.Parse(localIp).GetAddressBytes(), 0);


                ulong timestamp = (ulong)clock << 32 | ip;

                return timestamp;
            }
        }
    
}
