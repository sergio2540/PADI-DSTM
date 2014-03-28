using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
    class ServerPair
    {

        private string URLPrimary;
        private string URLReplica;

        public ServerPair(string primary, string replica)
        {
            this.URLPrimary = primary;
            this.URLReplica = replica;
        }

        public string GetPrimary() {
            return URLPrimary;
        }

        public string GetSecondary() {
            return URLReplica;
        }

        public void PrintServerPair() {
            Console.WriteLine("[ Primary: " + URLPrimary + " - " + "Secondary: " + URLReplica + "]");
        }

    }
}
