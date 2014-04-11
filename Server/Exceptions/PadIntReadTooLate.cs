using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonTypes;
using System.Runtime.Serialization;

namespace Server
{
    [Serializable]
    class PadIntReadTooLate : TxException
    {
         public PadIntReadTooLate(ulong tid, int uid) 
             : base(String.Format("In transaction {0}: The padInt with uid: {1} tried to read a later commited version",tid,uid)){
                 
        }

         protected PadIntReadTooLate(SerializationInfo info, StreamingContext context): base(info, context)
        {
 
        }
    }
}
