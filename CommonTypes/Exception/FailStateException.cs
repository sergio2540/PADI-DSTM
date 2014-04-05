using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public class FailStateException : TxException
    {
        public FailStateException()
        {

        }
        public FailStateException(String exceptionMessage) : base("Server down. Waiting for Recover call. Fail on: " + exceptionMessage)
        {
            
        }

        protected FailStateException(SerializationInfo info, StreamingContext context) : base (info, context)
        {
 
        }
    }
}
