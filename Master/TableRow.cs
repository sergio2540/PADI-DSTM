using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
    class TableRow
    {

        ServerPair servers;
        UIDRange uidRange;

        public TableRow(ServerPair servers, UIDRange uidRange) { 
            this.servers = servers;
            this.uidRange = uidRange;
        }

        public ServerPair GetServerPair() {
            return servers;
        }

        public UIDRange GetUIDRange() {
            return uidRange;
        }

    }
}
