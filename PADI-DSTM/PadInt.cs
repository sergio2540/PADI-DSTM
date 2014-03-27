using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    public class PadInt
    {

        private int padInt = 0;
        
        //private int uid;
        public int Uid { get; set; }
        
        public event EventHandler changeHandler;

        public PadInt(int uid) {
            this.Uid = uid;
        }

        public int Read() {
            return this.padInt;
        }

        public void Write(int value) {
            this.padInt = value;
            if (changeHandler != null)
                changeHandler(this, null);
        }

       

    }
}
