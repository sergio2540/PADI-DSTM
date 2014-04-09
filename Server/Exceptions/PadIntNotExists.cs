using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonTypes;
using System.Runtime.Serialization;

namespace Server
{
    [Serializable]
    class PadIntNotExists : TxException
    {

        public PadIntNotExists(ulong tid, int uid) : base(String.Format("In transaction {0}: The padInt with uid: {1} does not exist",tid,uid)){
                 
        }

        protected PadIntNotExists(SerializationInfo info, StreamingContext context): base(info, context)
        {
 
        }
    }
}
