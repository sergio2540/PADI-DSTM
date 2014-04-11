using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Diagnostics;
using System.Threading;
using Services_DSTM;
using CommonTypes;
using Server;
using PADI_DSTM;

namespace PADI_Tests
{
    /* Tests concurrency on same object. We will  be running two transactions accessing the same object. One of them must fail.
     */
    [TestClass]
    public class FailOnRead
    {


        static Process master;
        static Process server;
        static InstanceProxy proxy1;
        static InstanceProxy proxy2;
        private const int PAD_ID1 = 20;
        private const int PAD_ID2 = 30;




        [ClassInitialize]
        public static void TestInitialize(TestContext c)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            master = Process.Start(@"..\..\..\Master\bin\Debug\Master.exe");
            Thread.Sleep(1000);
            //for (int i = 0; i < 20; i++)
            server = Process.Start(@"..\..\..\Server\bin\Debug\Server.exe");
            Thread.Sleep(1000);

            proxy1 = new InstanceProxy();
            proxy2 = new InstanceProxy();
            proxy1.Init();
            proxy2.Init();
            

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod]
        [ExpectedException(typeof(PadIntWriteTooLate))]
        public void TestFailOnRead()
        {

           
            
            bool canBegin1 = proxy1.TxBegin();
            bool canBegin2 = proxy2.TxBegin();
            Assert.IsTrue(canBegin1, "Could not begin transaction1");
            Assert.IsTrue(canBegin2, "Could nout begin transaction2");

            PadInt padInt1 = proxy1.CreatePadInt(PAD_ID1);//os objectos criados e acedidos teem handlers para read/write. ou seja, abrem transaccao à primeira escrita.
            PadInt padInt2 = proxy2.CreatePadInt(PAD_ID2);

            Assert.IsNotNull(padInt1, "Could not create padint1 on transaction1");
            Assert.IsNotNull(padInt2, "Could not create padint2 on transaction1");


            int padint1Read = padInt1.Read();
            Thread.Sleep(2000);
            int padint2Read = padInt2.Read();

            Assert.IsNotNull(padint1Read, "Could not read padint1 on transaction1");
            Assert.IsNotNull(padint2Read, "Could not read padint2 on transaction1");
            Console.WriteLine("========================proxy1 accessing padint 2: ====================" + PAD_ID2);

            padInt1 = proxy1.AccessPadInt(PAD_ID2);
            Assert.IsNotNull(padInt1, "Could not access padint2 on transaction1");
            Console.WriteLine("========================Write====================");
            proxy1.Status();
            padInt1.Write(20);
          


        }

   

        [ClassCleanup]
        public static void ClassCleanUp() {
            //server.Kill();
            //master.Kill();
        
        }
    }
}
