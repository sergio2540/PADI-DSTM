using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;

namespace Services
{

    //PadInt Local

    public class PadIntLocal : PadInt
    {

        private int padInt = 0;
        
        //private int uid;
        public int Uid { get; set; }
        
        public event EventHandler changeHandler;
        public event EventHandler readHandler;

        public PadIntLocal(int uid) {
            this.Uid = uid;
        }

        public int Read() {
            if (readHandler != null)
                readHandler(this, null);
            return padInt;
            
        }

        public void Write(int value) {
            this.padInt = value;//temos de verificar que a escrita teve sucesso. 
            if (changeHandler != null)
                changeHandler(this, null);
        }

       

    }
}
