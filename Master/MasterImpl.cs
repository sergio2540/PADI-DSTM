﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonTypes;
using System.Diagnostics;

namespace Master
{
    public class MasterImpl : MarshalByRefObject, IMaster
    {

        private LookupTable lookupTable = new LookupTable();

        private int getIndex()
        {
            int table_size = lookupTable.Size();
            //Arrendondar para a potencia de 2 mais acima do numero tabela_size
            int end = (int)Math.Pow(2, Math.Ceiling(Math.Log(table_size) / Math.Log(2)));

            //Arrendondar para a potencia de 2 mais abaixo do numero tabela_size
            int start = (int)Math.Pow(2, (Math.Log(end) / Math.Log(2)) - 1);

            return start == 0 ? 0 : 2 * (table_size % start);
        }
        
        //Entradas/Saidas dos servidores de dados

        public bool AddServer(string URL)
        {

            int index = getIndex();


            String primary_url = URL;
            String replica_url = URL;
            UIDRange newUIDRange = null;

            if (lookupTable.Size() != 0)
            {
                TableRow temp = lookupTable.GetRow(index);
                newUIDRange = temp.GetUIDRange().Split();
            }
            else
            {
                newUIDRange = lookupTable.DefaultUIDRange;
            }

            //TODO:Buscar replica
            ServerPair newServerPair = new ServerPair(primary_url, replica_url);
            TableRow newTableRow = new TableRow(newServerPair, newUIDRange);
            lookupTable.InsertRow(index, newTableRow);

            return true;

        }

        public bool RemoveServer(string URL)
        {
            throw new NotImplementedException();
        }


        //Get endpoints
        public string GetPrimaryEndpoint(int uid)
        {
            ServerPair pair = lookupTable.GetServerPair(uid);

            if (pair == null)
                return null;

            return pair.GetPrimary();
        }

        public string GetReplicaEndpoint(int uid)
        {

            ServerPair pair = lookupTable.GetServerPair(uid);

            return pair == null ? null : pair.GetReplica();
                

        

        }

        //Falhas
        public void PrimaryFailed(string URL)
        {
            throw new NotImplementedException();
        }

        public void ReplicaFailed(string URL)
        {
            throw new NotImplementedException();
        }



        public bool Status()
        {
            throw new NotImplementedException();
        }

        public bool Fail()
        {
            throw new NotImplementedException();
        }

        public bool Freeze()
        {
            throw new NotImplementedException();
        }

        public bool Recover()
        {
            throw new NotImplementedException();
        }
    }


}
