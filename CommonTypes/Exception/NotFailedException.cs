using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{

    [Serializable]
    public class NotFailedException : TxException
    {
        
       // public NotFailedException() : base("Server is not down. Can't recover.")
        //{
            
        //}
        public NotFailedException() : base("Can't recover server that hasn't been put down by a previous fail.")
        {

        }

        protected NotFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
 
        }

       
    }
}
