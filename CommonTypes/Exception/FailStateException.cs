using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public class FailStateException : TxException
    {
        public FailStateException()
        {

        }
        public FailStateException(String exceptionMessage) : base("Server down. Waiting for Recover call. Fail on: " + exceptionMessage)
        {
            
        }
    }
}
