using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using DSTMServices;
using CommonTypes;

namespace PADI_DSTM
{
    public class DSTMManager
    {
        //private ulong currentTransactionId = 0;

        private CoordinatorService coordinatorService;
        private TimestampService timestampService;
        private DataService dataService;
        private DebugService debugService;

        public bool Init()
        {
            coordinatorService = new CoordinatorService();
            timestampService = new TimestampService();
            dataService = new DataService();
            debugService = new DebugService();

            return true;
        }

        public bool TxBegin()
        {
            var timestamp = timestampService.getTimestamp();
            
            //quando retornar falso?quando se tenta criar uma transaccao com outra a decorrer?
            return coordinatorService.Begin(timestamp);

           
        }

        public bool TxCommit()
        {
            return coordinatorService.Commit();
        }

        public bool TxAbort()
        {
            return coordinatorService.Abort();
        }

        public bool Status()
        {
            return debugService.Status();
        }

        public bool Fail(String URL)
        {
            return debugService.Fail(URL);
        }

        public bool Freeze(String URL)
        {
            return debugService.Freeze(URL);
        }

        public bool Recover(String URL)
        {
            return debugService.Recover(URL);
        }

        public PadInt CreatePadInt(int uid)
        {
            return coordinatorService.CreatePadInt(uid);
           
        }
        

        public PadInt AccessPadInt(int uid)
        {
            return coordinatorService.AccessPadInt(uid);
        }

       
    }
}
