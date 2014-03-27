using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;

namespace Server
{
    public enum PadIntState { Tentative, Commited }

    public abstract class PadIntRemote : PadInt
    {
        public int Value { get; set; }

        public readonly int uid;

        public PadIntState State {get;set;}

        public ulong WriteTimestamp { get; set; }

        public PadIntRemote(int uid)
        {
            this.uid = uid;
        }

        public abstract int Read();

        public abstract void Write(int value);
        

       
    }
}