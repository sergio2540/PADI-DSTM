using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IDebug
    {
        bool Status();
        bool Fail();
        bool Freeze(String URL);
        bool Recover();
    }
}
