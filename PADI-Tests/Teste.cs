using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PADI_DSTM;
using CommonTypes;
using Master;
using System.Threading;
using Server;
using System.Diagnostics;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.IO;

namespace PADI_Tests
    {

    [TestClass]
    public class Teste
    {
        static Process master;
        static Process server1;
        static Process server2;

        public static void run()
        {
            Thread.Sleep(3000);
            server1 = Process.Start(@"..\..\..\Server\bin\Debug\Server.exe");
            
        }


        [ClassInitialize]
        public static void TestInitialize(TestContext c)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            master =  Process.Start(@"..\..\..\Master\bin\Debug\Master.exe");
            Thread.Sleep(3000);
            server1 = Process.Start(@"..\..\..\Server\bin\Debug\Server.exe");

            //new Thread(new ThreadStart(run)).Start();
            
        }

        public static void StartServer() {
            server2 = Process.Start(@"..\..\..\Server\bin\Debug\Server.exe"); 
        }
         
        [TestMethod]
        public void TestComThread()
        {
            Thread.Sleep(2000);
            bool beginSuccess = false;
            int uid2 = 4; //object 1 uid
            int uid3 = 10;
            // int uid2 = 2; //object 2 uid
            const int DEFAULT_PADINT_VALUE = 0;
            const int WRITE_VALUE = 5;

            bool result = PadiDstm.Init();

            Assert.IsTrue(result, "Failed to load library.");

            //T1
            beginSuccess = PadiDstm.TxBegin();
            //bool status = PadiDstm.Status();
            Assert.IsTrue(beginSuccess, "Failed to begin transaction.");

            PadInt padInt1 = PadiDstm.CreatePadInt(uid2);
            Assert.IsNotNull(padInt1, "CreatePadint returned null for uid:" + uid2);


            //int firstRead = padInt1.Read();
            //Assert.AreEqual(firstRead, DEFAULT_PADINT_VALUE, String.Format("Read:{0} Expected:{1}", firstRead, DEFAULT_PADINT_VALUE));
            int value = padInt1.Read();

            Console.WriteLine(value);

            padInt1.Write(10);
            //int secondRead = padInt1.Read();
            //Assert.AreEqual(secondRead, WRITE_VALUE, String.Format("Read:{0} Expected:{1}", secondRead, WRITE_VALUE));
         
            bool didCommit = PadiDstm.TxCommit();

            bool status = PadiDstm.Status();

            StartServer();

            Console.WriteLine("depois de arrancar o 2 server");


            Debug.WriteLine("antes do thread sleep de 15s");
            Thread.Sleep(15000);
            status = PadiDstm.Status();
            Debug.WriteLine("depois do thread sleep de 15segundos");

            beginSuccess = PadiDstm.TxBegin();

            Assert.IsTrue(beginSuccess, "Nao deu true");
            if (beginSuccess)
                Console.WriteLine("success");
            PadInt pad = PadiDstm.CreatePadInt(uid3);



            int thirdRead = pad.Read();

            Assert.IsTrue(didCommit, "Failed to commit transaction.");
            didCommit = PadiDstm.TxCommit();

            Assert.IsTrue(didCommit, "Failed to commit transaction.");

            status = PadiDstm.Status();

        }

        [ClassCleanup]
        public static void ClassCleanUp() {
            //server.Kill();
           // master.Kill();
        
        }

        }
    }
