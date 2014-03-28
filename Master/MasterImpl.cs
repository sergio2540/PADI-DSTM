using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;

namespace Master
{
    class MasterImpl : MarshalByRefObject, IMaster
    {

        private LookupTable lookupTable = new LookupTable();
        
        public bool AddServer(string URL)
        {
            throw new NotImplementedException();
        }

        public bool RemoveServer(string URL)
        {
            throw new NotImplementedException();
        }

        public void PrimaryFailed(string URL)
        {
            throw new NotImplementedException();
        }

        public void ReplicaFailed(string URL)
        {
            throw new NotImplementedException();
        }

        public string GetServer(long uid)
        {

            foreach(TableRow row in lookupTable.GetTableRows()) 
            {
                if (row.GetUIDRange().UIDInRange(uid))
                {
                    return row.GetServerPair().GetPrimary();
                }
                else
                {
                    Console.WriteLine("Servidor reponsável por uid não encontrado na LookupTable");
                    return " ";
                }

            }

        }

        public bool Status()
        {
            throw new NotImplementedException();
        }

        public bool Fail(string URL)
        {
            throw new NotImplementedException();
        }

        public bool Freeze(string URL)
        {
            throw new NotImplementedException();
        }

        public bool Recover(string URL)
        {
            throw new NotImplementedException();
        }
    }


}
