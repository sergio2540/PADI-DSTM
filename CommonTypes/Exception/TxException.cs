using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public abstract class TxException : SystemException
    {
        public TxException()
        {

        }
        public TxException(String mensagem) : base(mensagem)
        {
            
        }

    }
}
