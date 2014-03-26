using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IServer : MarshalByRefObject
    {

      public bool StartTransaction(ulong transactionId, String coordinatorAddress);


      //Transacções

      //Participante retorna voto true - commit false - abort
      public bool canCommit(ulong transactioId);

      //Participante deve fazer commit
      public bool doCommit(ulong transactionId);
      
      public bool doAbort(ulong transactionId); 


      //PadInt
      public bool CreatePadInt(ulong transactionId, int padintUid);
      public int ReadPadInt(ulong transactionId, int padintUid);
      public void WritePadInt(ulong transactionId, int padintUid, int newValue);

    }
}
