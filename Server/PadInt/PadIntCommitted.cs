using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;

namespace Server
{
    class PadIntCommitted : PadIntRemote
    {
        public PadIntCommitted(int uid, ulong writeTimestamp) : base(uid)
        {
            this.State = PadIntState.Commited;
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
