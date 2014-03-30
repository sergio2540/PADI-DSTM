using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;

namespace Server
{
    public class PadIntTentative : PadIntRemote
    {
        public ulong ReadTimestamp { get; set; }

        public PadIntTentative(int uid, ulong writeTimestamp, int value)
            : this(uid,0,writeTimestamp, value)
        {
           
        }

        public PadIntTentative(int uid, ulong readTimestamp, ulong writeTimestamp, int value) : base(uid,value)
        {
            this.State = PadIntState.Tentative;
            this.ReadTimestamp = readTimestamp;
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
