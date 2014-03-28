using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IMaster : IDebug
    {

        bool AddServer(String URL);

        bool RemoveServer(String URL);

        void PrimaryFailed(String URL);

        void ReplicaFailed(String URL);

        string GetServer(long uid);

    }
}
