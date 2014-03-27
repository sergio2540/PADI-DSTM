using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonTypes;

namespace Server
{
    //Ver remoting exceptions
    class PadIntNotExists : TxException
    {

        public PadIntNotExists(ulong tid, int uid) : base(String.Format("In transaction {0}: The padInt with uid: {1} does not exist",tid,uid)){
                 
        }

       
    }
}
