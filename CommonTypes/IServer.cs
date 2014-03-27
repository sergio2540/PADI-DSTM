using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IServer 
    {

      bool StartTransaction(ulong transactionId, String coordinatorAddress);


      //Transacções

      //Participante retorna voto true - commit false - abort
      bool canCommit(ulong transactioId);

      //Participante deve fazer commit
      bool doCommit(ulong transactionId);
      
      bool doAbort(ulong transactionId); 


      //PadInt
      bool CreatePadInt(ulong transactionId, int padintUid);
      int ReadPadInt(ulong transactionId, int padintUid);
      void WritePadInt(ulong transactionId, int padintUid, int newValue);

    }
}
