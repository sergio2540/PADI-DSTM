using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PADI_DSTM;

using CommonTypes;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;

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
                //T1
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


                padInt1.Write(3);
                Console.WriteLine(padInt1.Read());

                PadInt pad  = Manager.AccessPadInt(1);
                Console.WriteLine(pad.Read());
                Manager.TxAbort();

                //T2
                succeed = Manager.TxBegin();

                if (!succeed)
                    return false;
                PadInt pad2 = Manager.AccessPadInt(1);
                Console.WriteLine("Reading again:" + pad2.Read());


                /*
                PadInt padInt2 = Manager.CreatePadInt(uid2);
                if (padInt2 == null)
                {
                    Manager.TxAbort();
                }
                
                padInt2.Write(7);
               */

                succeed = Manager.TxCommit();

                if (!succeed)
                {
                    Manager.TxAbort();
                    return false;
                }

               
                
               
               
                Console.Read();
                

            }
            catch (TxException e)
            {
                Console.WriteLine(e);
            }

            return true;
        }
        
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);

            ClientApp clientApp = new ClientApp();

            clientApp.Manager = new DSTMManager();
            
            clientApp.Manager.Init();

            //clientApp.transaction1();
            clientApp.transaction2();

            


            

        }

        private bool transaction1()
        {
            bool succeed = false;
            int uid1 = 1;
            int uid2 = 2;

            try
            {
                //T1
                succeed = Manager.TxBegin();

                if (!succeed)
                    return false;

                PadInt padInt1 = Manager.CreatePadInt(uid1);
                if (padInt1 == null)
                {
                    Manager.TxAbort();
                }

                int a = padInt1.Read();
                Console.WriteLine("First read: " + a);


                padInt1.Write(3);
                Console.WriteLine("Second read: " + padInt1.Read());

                PadInt pad = Manager.AccessPadInt(1);
                Console.WriteLine("Third read: " + pad.Read());
                Manager.TxAbort();

                //T2
                succeed = Manager.TxBegin();

                if (!succeed)
                    return false;
                PadInt pad2 = Manager.AccessPadInt(1);
                Console.WriteLine("Reading again:" + pad2.Read());


                /*
                PadInt padInt2 = Manager.CreatePadInt(uid2);
                if (padInt2 == null)
                {
                    Manager.TxAbort();
                }
                
                padInt2.Write(7);
               */

                succeed = Manager.TxCommit();

                if (!succeed)
                {
                    Manager.TxAbort();
                    return false;
                }





                Console.Read();


            }
            catch (TxException e)
            {
                Console.WriteLine(e);
            }

            return true;
        }

        private bool transaction2()
        {
            bool succeed;

            try {

            Console.WriteLine("Primeira transaccao começa");
            succeed = Manager.TxBegin();
            if (!succeed)
                return false;

            PadInt pad = Manager.CreatePadInt(2);
            Console.WriteLine("Antes do write");

            pad.Write(10);
            Console.WriteLine("Depois dow write");

            
            succeed = Manager.TxCommit();
            

            if (!succeed)
            {
                Manager.TxAbort();
                return false;
            }
            Console.WriteLine("Segunda transaccao começa");
            succeed = Manager.TxBegin();
            if (!succeed)
                return false;

            pad = Manager.AccessPadInt(2);
            Console.WriteLine("Accc: ");
            Console.WriteLine("LOL: " + pad.Read());

            succeed = Manager.TxCommit();

            if (!succeed)
            {
                Manager.TxAbort();
                return false;
            }
            Console.ReadLine();

            }
            catch (TxException e)
            {
                Console.WriteLine(e);
            }

            return true;
        }
    
    }


}
