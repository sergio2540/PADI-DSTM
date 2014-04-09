using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PADI_DSTM;

using CommonTypes;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace Client
{

    class ClientApp
    {
        
      

      
        
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);

            ClientApp clientApp = new ClientApp();

            
            PadiDstm.Init();
            //clientApp.PadiDstm.Fail("tcp://localhost:8089/Server");
            //clientApp.PadiDstm.Fail("tcp://localhost:8089/Server");
            //clientApp.PadiDstm.Recover("tcp://localhost:8089/Server");
            //clientApp.PadiDstm.Fail("tcp://localhost:8089/Server");
            //Console.WriteLine("About to recover");
            //clientApp.PadiDstm.Recover("tcp://localhost:8089/Server");
            //clientApp.PadiDstm.Recover("tcp://localhost:8089/Server");


            //clientApp.PadiDstm.Status();


            //clientApp.PadiDstm.Recover("tcp://localhost:8089/Server");
            //clientApp.PadiDstm.Freeze("tcp://localhost:8089/Server");
            //clientApp.PadiDstm.Freeze("tcp://localhost:8089/Server");
            //clientApp.PadiDstm.Fail("tcp://localhost:8089/Server");
            //clientApp.PadiDstm.Fail("tcp://localhost:8089/Server");
            //Console.WriteLine("After fail");


            //Console.ReadLine();
            //clientApp.transaction1();
            clientApp.transaction2();

            Console.ReadLine();
            PadiDstm.Status();
            Console.ReadLine();
   

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
            succeed = PadiDstm.TxBegin();
            if (!succeed)
                return false;

            PadInt pad = PadiDstm.CreatePadInt(2);
            Console.WriteLine("Antes do write");
            pad = PadiDstm.AccessPadInt(2);
            pad.Write(10);
            pad.Write(20);
            
            Console.WriteLine("Deve ler 20 ->" + pad.Read());

            
            succeed = PadiDstm.TxCommit();
            

            if (!succeed)
            {
                PadiDstm.TxAbort();
                return false;
            }
            Console.WriteLine("Segunda transaccao começa");

            succeed = PadiDstm.TxBegin();
            if (!succeed)
                return false;

            pad = PadiDstm.AccessPadInt(2);
            pad.Write(10);
            Console.WriteLine("Deve dar 10 -> " + pad.Read());

            succeed = PadiDstm.TxCommit();

            if (!succeed)
            {
                PadiDstm.TxAbort();
                return false;
            }

            //T3
            succeed = PadiDstm.TxBegin();
            if (!succeed)
                return false;

            pad = PadiDstm.CreatePadInt(3);
            pad.Write(30);
            Console.WriteLine("Deve dar 30 -> " + pad.Read());

            succeed = PadiDstm.TxCommit();

            if (!succeed)
            {
                PadiDstm.TxAbort();
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
                succeed = PadiDstm.TxBegin();
                if (!succeed)
                    return false;

                //PadInt pad = PadiDstm.CreatePadInt(2);
                Console.WriteLine("Antes do write");
                PadInt pad = PadiDstm.AccessPadInt(2);
                pad.Write(10);
                pad.Write(20);

                Console.WriteLine("Deve ler 20 ->" + pad.Read());


                succeed = PadiDstm.TxCommit();


                if (!succeed)
                {
                    PadiDstm.TxAbort();
                    return false;
                }
                Console.WriteLine("Segunda transaccao começa");

                succeed = PadiDstm.TxBegin();
                if (!succeed)
                    return false;

                pad = PadiDstm.AccessPadInt(2);
                pad.Write(10);
                Console.WriteLine("Deve dar 10 -> " + pad.Read());

                succeed = PadiDstm.TxCommit();

                if (!succeed)
                {
                    PadiDstm.TxAbort();
                    return false;
                }

                //T3
                succeed = PadiDstm.TxBegin();
                if (!succeed)
                    return false;

                pad = PadiDstm.CreatePadInt(3);
                pad.Write(30);
                Console.WriteLine("Deve dar 30 -> " + pad.Read());

                succeed = PadiDstm.TxCommit();

                if (!succeed)
                {
                    PadiDstm.TxAbort();
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
