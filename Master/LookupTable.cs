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

        public List<TableRow> GetTableRows()
        {
            return lookupTable;
        }

        public void AddRow(TableRow row, int index) 
        {

            lookupTable.Add(row);

        }

        public void RemoveRow(TableRow row)
        {
            lookupTable.Remove(row);
        }

    }
}
