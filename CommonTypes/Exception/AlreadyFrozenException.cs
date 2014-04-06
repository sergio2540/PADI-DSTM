using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public class AlreadyFrozenException : TxException
    {
        public AlreadyFrozenException() :base("Server is already frozen.")
        {

        }
      

        protected AlreadyFrozenException(SerializationInfo info, StreamingContext context) : base (info, context)
        {
 
        }
    }
}
