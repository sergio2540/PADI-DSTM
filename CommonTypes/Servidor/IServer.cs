using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IServer : IDebug
    {

      bool BeginTransaction(ulong transactionId, String coordinatorAddress);


      //Transacções

      //Participante retorna voto true - commit false - abort
      bool canCommit(ulong transactioId);

      //Participante deve fazer commit
      bool doCommit(ulong transactionId);
      
      bool doAbort(ulong transactionId); 


      //PadInt
      PadInt CreatePadInt(ulong transactionId, int padintUid);//estes objectos deveriam devolver apenas booleanos
      PadInt AccessPadInt(ulong transactionId, int padintUid);
      int ReadPadInt(ulong transactionId, int padintUid);
      void WritePadInt(ulong transactionId, int padintUid, int newValue);

      ulong GetMaxTID();
      void SetMaxTID(ulong tid);

      ulong GetTid();

      void SendPadInt(List<PadIntRemote> padInts);

      void AddTIDToPendingTable(string url, ulong tid, int startRange, int endRange);


    }

}
