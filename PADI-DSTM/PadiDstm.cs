using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using DSTMServices;
using CommonTypes;
using DSTMServices;

namespace PADI_DSTM
{
    public class PadiDstm
    {
        //private ulong currentTransactionId = 0;

        private static CoordinatorService coordinatorService;
        //private TimestampService timestampService;
        private static DebugService debugService;
        private static MasterService masterService;


        private PadiDstm() { }
        public static bool Init()
        {
            masterService = new MasterService("tcp://localhost:8080/Master");
            coordinatorService = new CoordinatorService(masterService);
            //timestampService = new TimestampService();
            debugService = new DebugService(masterService);

            return true;
        }

        public static bool TxBegin()
        {
            //var timestamp = timestampService.getTimestamp();
            
            //quando retornar falso?quando se tenta criar uma transaccao com outra a decorrer?
            //return coordinatorService.Begin(timestamp);
            return coordinatorService.Begin();


           
        }

        public static bool TxCommit()
        {
            return coordinatorService.Commit();
        }

        public static bool TxAbort()
        {
            return coordinatorService.Abort();
        }

        public static bool Status()
        {
            return debugService.Status();
        }

        public static bool Fail(String URL)
        {
            return debugService.Fail(URL);
        }

        public static bool Freeze(String URL)
        {
            return debugService.Freeze(URL);
        }

        public static bool Recover(String URL)
        {
            return debugService.Recover(URL);
        }

        public static PadInt CreatePadInt(int uid)
        {
            return coordinatorService.CreatePadInt(uid);
           
        }
        

        public static PadInt AccessPadInt(int uid)
        {
            return coordinatorService.AccessPadInt(uid);
        }

       
    }
}
