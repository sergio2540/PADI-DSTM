using DSTMServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTMServices
{
    public class DebugService
    {
        private MasterService masterService;
        public DebugService(MasterService masterS) {

            masterService = masterS;
        }

        public bool Status()
        {
            //get master
            //master.Status()
            return false;  
        }
        public bool Fail(String URL)
        {
            return false;
        }

        public bool Freeze(String URL)
        {
            return false;
        }

        public bool Recover(String URL)
        {
            return false;
        }

    }
}
