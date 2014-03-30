using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;

namespace Server
{
    [Serializable]
    class PadIntCommitted : PadIntRemote
    {

        public PadIntCommitted(int uid) : this(uid,0,0)
        {
           
        }

        public PadIntCommitted(int uid, ulong writeTimestamp, int value) : base(uid,value)
        {
            this.State = PadIntState.Committed;
            this.WriteTimestamp = writeTimestamp;
        }
       
        public override int Read()
        {
            return this.Value;
        }

        public override void Write(int value)
        {
            this.Value = value;
        }
    }
}
