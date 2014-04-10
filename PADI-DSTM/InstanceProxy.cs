using CommonTypes;
using DSTMServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services_DSTM
{
    public class InstanceProxy
    {
        private CoordinatorService coordinatorService;
        //private TimestampService timestampService;
        private DebugService debugService;
        private MasterService masterService;


        public bool Init()
        {
            masterService = new MasterService("tcp://localhost:8080/Master");
            coordinatorService = new CoordinatorService(masterService);
            //timestampService = new TimestampService();
            debugService = new DebugService(masterService);

            return true;
        }

        public bool TxBegin()
        {
            //var timestamp = timestampService.getTimestamp();
            
            //quando retornar falso?quando se tenta criar uma transaccao com outra a decorrer?
            //return coordinatorService.Begin(timestamp);
            return coordinatorService.Begin();


           
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
