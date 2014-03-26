using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    class DSTMManager
    {
        private long currentTransactionId = 0;
        private Coordinator transaccionalCoordinator;
        private string getLocalIp() { 
        
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

        bool Init()
        {
            transaccionalCoordinator = new Coordinator();
        }

        bool TxBegin()
        {
            //get transaction id
            int localTime = DateTime.Now.Millisecond;
            string localIp = getLocalIp();
            int intAddress = BitConverter.ToInt32(IPAddress.Parse(localIp).GetAddressBytes(), 0);
            currentTransactionId = localTime + intAddress;
            transaccionalCoordinator.BeginTransaction(currentTransactionId);
            return true;//quando retornar falso?quando se tenta criar uma transaccao com outra a decorrer?
        }

        bool TxCommit();

        bool TxAbort();

        bool Status();

        bool Fail(String URL);

        bool Freeze(String URL);

        bool Recover(String URL);

        PadInt CreatePadInt(int uid);

        PadInt AccessPadInt(int uid); 
    }
}
