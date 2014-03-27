using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PADI_DSTM;

namespace Client
{

    class ClientApp
    {
        
        public DSTMManager Manager { get; set; }

        bool transaction_only_reads()
        {
            bool succeed = false;
            int uid1 = 1;
            int uid2 = 2;

            try
            {
                succeed = Manager.TxBegin();
                
                if (!succeed)
                    return false;

                PadInt padInt1 = Manager.CreatePadInt(uid1);
                if (padInt1 == null)
                {
                    Manager.TxAbort();
                }

                int a = padInt1.Read();
                Console.WriteLine(a);

                PadInt padInt2 = Manager.CreatePadInt(uid2);
                if (padInt2 == null)
                {
                    Manager.TxAbort();
                }
                int b = padInt2.Read();
                Console.WriteLine(b);

                succeed = Manager.TxCommit();

                if (!succeed)
                {
                    Manager.TxAbort();
                    return false;
                }

            }
            catch (TxException e)
            {
                Console.WriteLine(e);
            }

            return true;
        }
        
        static void Main(string[] args)
        {
            var clientApp = new ClientApp();

            clientApp.Manager = new DSTMManager();
            
            clientApp.Manager.Init();

            clientApp.transaction_only_reads();

            

        }
    
    }


}
