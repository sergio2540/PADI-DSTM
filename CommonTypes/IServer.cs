using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IServer : MarshalByRefObject
    {

      public bool StartTransaction(long transactionId, String coordinatorAddress);
      public bool CreatePadint(long transactionId, int padintUid);
      public int ReadPadint(long transactionId, int padintUid);
      public void WritePadInt(long transactionId, int padintUid, int newValue);

    }
}
