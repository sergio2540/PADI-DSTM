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
            //DefaultUIDRange = new UIDRange(0, 2048);
            DefaultUIDRange = new UIDRange(int.MinValue, int.MaxValue);
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

            //Console.WriteLine("Servidor responsável por uid não encontrado");
            return null;
        }


       public void InsertRow(int index, TableRow row) 
       {
            lookupTable.Insert(index,row);
       }

       public void ReplaceRow(int index, TableRow row)
       {
           lookupTable[index] = row;
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
                output += String.Format("{0} {1}\n", row.GetUIDRange().ToString(), row.GetServerPair().ToString());
            }

            return output;

        }


        //Check if given string correspond to replica(false) or main server(true)
        public bool IsPrimary(String url) {

            foreach (TableRow row in lookupTable)
            {
                ServerPair pair = row.GetServerPair();
                if (url.Equals(pair.GetPrimary()))
                    return true;
                else if (url.Equals(pair.GetReplica()))
                    return false;
            }

            //DEVERIA SER LANCADA UMA EXCEPCAO POR NAO EXISTIR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            return false;
        
        }

        public TableRow GetRowGivenReplica(String replicaUrl)
        {
            foreach (TableRow row in lookupTable)
            {

                ServerPair pair = row.GetServerPair();

                if (replicaUrl.Equals(pair.GetReplica()))
                {
                    return row; //DEVIAMOS LANCAR EXCEPCAO CASO NAO EXISTA O MAIN SERVER.
                }
            }
            return null;
        }

        public TableRow GetRowGivenPrimary(String primaryUrl)
        {
            foreach (TableRow row in lookupTable)
            {

                ServerPair pair = row.GetServerPair();

                if (primaryUrl.Equals(pair.GetPrimary()))
                {
                    return row; //DEVIAMOS LANCAR EXCEPCAO CASO NAO EXISTA O MAIN SERVER.
                }
            }
            return null;
        }
         
        public void SetNewReplica(String oldReplicaUrl, String newReplicaUrl) {

            GetRowGivenReplica(oldReplicaUrl).GetServerPair().SetReplica(newReplicaUrl);
        
        }

        //
        public void SwapPrimaryReplica(String oldPrimaryUrl, String newReplicaUrl) {

            ServerPair pair = GetRowGivenPrimary(oldPrimaryUrl).GetServerPair();
            pair.SetPrimary(pair.GetReplica());
            pair.SetReplica(newReplicaUrl);
            
        
        }
    

    }
}