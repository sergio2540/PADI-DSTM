using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonTypes;
using System.Runtime.Serialization;

namespace Server
{
    [Serializable]
    public class PadIntWriteTooLate : TxException
    {
         public PadIntWriteTooLate(ulong tid, int uid) 
             : base(String.Format("In transaction {0}: The padInt with uid: {1} tried to write a later commited version",tid,uid)){
                 
        }

         protected PadIntWriteTooLate(SerializationInfo info, StreamingContext context) : base(info, context)
        {
 
        }
    }
}
