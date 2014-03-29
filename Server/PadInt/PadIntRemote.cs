using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;

namespace Server
{
    public enum PadIntState { Tentative, Committed, Aborted }


    [Serializable]
    public abstract class PadIntRemote : PadInt
    {
        public int Value { get; set; }

        public readonly int uid;

        public PadIntState State {get;set;}

        public void SetTentative()
        {
            State = PadIntState.Tentative;
        }

        public void SetCommited()
        {
            State = PadIntState.Committed;
        }

        public void SetAborted()
        {
            State = PadIntState.Aborted;
        }

        public ulong WriteTimestamp { get; set; }

        public PadIntRemote(int uid)
        {
            this.uid = uid;
        }

        public abstract int Read();

        public abstract void Write(int value);
        

       
    }
}