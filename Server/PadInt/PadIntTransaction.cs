using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PadIntTransaction
    {
        private int Uid { get; set; }

        //Ordenado por readTimestamp
        //Sera possivel haver duplicados
       // private SortedSet<PadIntTentative> tentatives = new SortedSet<PadIntTentative>(Comparer<PadIntTentative>.Create(
         //                                                                                   (p1, p2) => p1.ReadTimestamp.CompareTo(p2.ReadTimestamp)));

       
        private SortedList<ulong,PadIntTentative> tentatives = new SortedList<ulong,PadIntTentative>(); // deveria ser readonly para funcoes exteriores
        
        private PadIntCommitted committed;

        public PadIntTransaction(int uid)
        {
            this.Uid = uid;
        }
        
        public SortedList<ulong,PadIntTentative> getTentatives()
        {
            return tentatives;
        }

        public void addTentative(ulong tid,PadIntTentative tentative)
        {
            tentatives[tid] = tentative; ;
        }

        public PadIntCommitted getCommitted()
        {
            return committed;
        }

        public void setCommited(PadIntCommitted committed)
        {
            this.committed = committed;
        }

    }
}
