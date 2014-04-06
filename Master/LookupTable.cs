using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
    class LookupTable
    {

        private List<TableRow> lookupTable;
        public UIDRange DefaultUIDRange {get; private set;}

        public LookupTable() 
        {
            lookupTable = new List<TableRow>();
            //mudar este range para Int32.Max
            DefaultUIDRange = new UIDRange(0, 2048);
        }

       public int Size(){
           return lookupTable.Count;
       }

       public TableRow GetRow(int index)
       {
           return lookupTable[index];
       }

       public ServerPair GetServerPair(int uid)
       {

            foreach (TableRow row in lookupTable)
            {
                if (row.GetUIDRange().UIDInRange(uid))
                {
                    return row.GetServerPair();
                }

            }

            Console.WriteLine("Servidor responsável por uid não encontrado");
            return null;
        }


       public void InsertRow(int index, TableRow row) 
       {
            lookupTable.Insert(index,row);
       }

        public void RemoveRow(TableRow row)
        {
            lookupTable.Remove(row);
        }

        public string ToString()
        {
            string output = String.Empty;
            foreach (TableRow row in lookupTable)
            {
                output += String.Format("{0} {1} {2} \n", row.GetUIDRange().ToString(), row.GetServerPair().GetPrimary(), row.GetServerPair().GetReplica());

            }

            return output;

        }

    }
}