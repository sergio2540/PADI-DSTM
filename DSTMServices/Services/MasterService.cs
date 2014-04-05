using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTMServices.Services
{
    public class MasterService
    {
        public readonly IMaster Master { get; private set; }
        private String masterEndPoint;
        public MasterService(String masterUrl) {

            masterEndPoint = masterUrl;
            Master = (IMaster)Activator.GetObject(typeof(IMaster), masterEndPoint);
        }

        public String GetPrimaryEndpoint(int uid) { 

            return Master.GetPrimaryEndpoint(uid);
        
        }

       

    }
}
