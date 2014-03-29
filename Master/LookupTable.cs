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

        public LookupTable() 
        {
            lookupTable = new List<TableRow>();
            
        }

        /*
        public List<TableRow> GetTableRows()
        {
            return lookupTable;
        }*/


       //ADD
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

    }
}