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

      
        
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);

            ClientApp clientApp = new ClientApp();

            clientApp.Manager = new DSTMManager();
            
            clientApp.Manager.Init();
            clientApp.Manager.Fail("tcp://localhost:8089/Server");
            clientApp.Manager.Fail("tcp://localhost:8089/Server");
            clientApp.Manager.Recover("tcp://localhost:8089/Server");
            clientApp.Manager.Fail("tcp://localhost:8089/Server");
            Console.WriteLine("About to recover");
            clientApp.Manager.Recover("tcp://localhost:8089/Server");
            clientApp.Manager.Recover("tcp://localhost:8089/Server");


            Console.ReadLine();
            //clientApp.transaction1();
            //clientApp.transaction2();

        }

        //Criado 1 e 2
        //1-> 3
        //2 -> 7
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
                Console.WriteLine("Reading again padInt 1:" + pad2.Read());


                
                PadInt padInt2 = Manager.CreatePadInt(2);
                if (padInt2 == null)
                {
                    Manager.TxAbort();
                }
                
                padInt2.Write(7);
                Console.WriteLine("Reading again padInt 2:" + padInt2.Read());

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

        //Criado 3 
        //1-> 3
        //2 -> 10
        //3-> 30
        private bool transaction2()
        {
            bool succeed;

            try {

            Console.WriteLine("Primeira transaccao começa");
            succeed = Manager.TxBegin();
            if (!succeed)
                return false;

            //PadInt pad = Manager.CreatePadInt(2);
            Console.WriteLine("Antes do write");
            PadInt pad = Manager.AccessPadInt(2);
            pad.Write(10);
            pad.Write(20);
            
            Console.WriteLine("Deve ler 20 ->" + pad.Read());

            
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
            pad.Write(10);
            Console.WriteLine("Deve dar 10 -> " + pad.Read());

            succeed = Manager.TxCommit();

            if (!succeed)
            {
                Manager.TxAbort();
                return false;
            }

            //T3
            succeed = Manager.TxBegin();
            if (!succeed)
                return false;

            pad = Manager.CreatePadInt(3);
            pad.Write(30);
            Console.WriteLine("Deve dar 30 -> " + pad.Read());

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


        //Criado 3 
        //1-> 3
        //2 -> 10
        //3-> 30
        private bool transaction3()
        {
            bool succeed;

            try
            {

                Console.WriteLine("Primeira transaccao começa");
                succeed = Manager.TxBegin();
                if (!succeed)
                    return false;

                //PadInt pad = Manager.CreatePadInt(2);
                Console.WriteLine("Antes do write");
                PadInt pad = Manager.AccessPadInt(2);
                pad.Write(10);
                pad.Write(20);

                Console.WriteLine("Deve ler 20 ->" + pad.Read());


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
                pad.Write(10);
                Console.WriteLine("Deve dar 10 -> " + pad.Read());

                succeed = Manager.TxCommit();

                if (!succeed)
                {
                    Manager.TxAbort();
                    return false;
                }

                //T3
                succeed = Manager.TxBegin();
                if (!succeed)
                    return false;

                pad = Manager.CreatePadInt(3);
                pad.Write(30);
                Console.WriteLine("Deve dar 30 -> " + pad.Read());

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
