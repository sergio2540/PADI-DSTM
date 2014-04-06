using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public class NotFrozenException : TxException
    {
         public NotFrozenException() : base("Can't freeze server because server is already frozen.")
        {

        }

         protected NotFrozenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
 
        }
    }
}
