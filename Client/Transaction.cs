using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    interface ITransaction
    {

        bool TxBegin();

        bool TxCommit();

        bool TxAbort();

        bool Status();

        bool Fail(String URL);

        bool Freeze(String URL);

        bool Recover(String URL);

        PadInt CreatePadInt(int uid);

        PadInt AccessPadInt(int uid); 



    }

}
