using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTMServices
{
    public class MasterService
    {
        private readonly IMaster master;
        private String masterEndPoint;
        public MasterService(String masterUrl) {

            masterEndPoint = masterUrl;
            master = (IMaster)Activator.GetObject(typeof(IMaster), masterEndPoint);
        }

        public String GetPrimaryEndpoint(int uid) { 

            return master.GetPrimaryEndpoint(uid);
        
        }

       public IMaster Master{get{return master;}}

    }
}
