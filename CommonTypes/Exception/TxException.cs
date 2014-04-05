using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public abstract class TxException : Exception //na documentação aconselham a derivar desta antiga:SystemException
    {
        public TxException()
        {

        }
        public TxException(String exceptionMessage) : base(exceptionMessage)
        {
            
        }

    }
}
