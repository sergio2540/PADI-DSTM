using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public abstract class TxException : Exception //na documentação aconselham a derivar desta antiga:SystemException
    {
        public TxException()
        {

        }
        public TxException(String exceptionMessage) : base(exceptionMessage)
        {
            
        }

        protected TxException(SerializationInfo info, StreamingContext context) : base (info, context)
        {
 
        }


    }
}
