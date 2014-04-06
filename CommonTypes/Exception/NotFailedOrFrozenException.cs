using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public class NotFailedOrFrozenException :TxException
    {

          public  NotFailedOrFrozenException() : base("Can't recover because server is not on fail/frozen state.")
        {

        }

          protected NotFailedOrFrozenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
 
        }
    }
}
