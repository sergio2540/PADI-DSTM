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

        private int value = 0;
        
        //private int uid;
        public int Uid { get; set; }
        
        public event EventHandler changeHandler;
        public event EventHandler readHandler;

        public PadIntLocal(int uid, int value)
        {
            this.Uid = uid;
            this.value = value;
        }

        public PadIntLocal(int uid) : this(uid,0) {
           
        }

        public int Read() {
            if (readHandler != null)
                readHandler(this, null);
            return value;
            
        }

        public void Write(int value) {
            //Console.WriteLine("write called!");
            this.value = value;//temos de verificar que a escrita teve sucesso. 
            if (changeHandler != null)
                changeHandler(this, null);
        }

       

    }
}
