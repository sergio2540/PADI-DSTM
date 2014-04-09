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
    /*Tests if the committed are available one a next access to the padint.
     
     */

    [TestClass]
    public class ReadAfterCommit
    {
        static Process master;
        static Process server;

    

        [ClassInitialize]
        public static void TestInitialize(TestContext c)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            master =  Process.Start(@"..\..\..\Master\bin\Debug\Master.exe");
            Thread.Sleep(1000);
            for (int i = 0; i < 20; i++)
                server = Process.Start(@"..\..\..\Server\bin\Debug\Server.exe");
            Thread.Sleep(1000);


        }
         
        [TestMethod]
        public void TestMethod1()
        {

            bool beginSuccess = false;
            int uid2 = 2; //object 1 uid
            // int uid2 = 2; //object 2 uid
            const int DEFAULT_PADINT_VALUE = 0;
            const int WRITE_VALUE = 5;

            bool result = PadiDstm.Init();

            Assert.IsTrue(result, "Failed to load library.");

            //T1
            beginSuccess = PadiDstm.TxBegin();

            Assert.IsTrue(beginSuccess, "Failed to begin transaction.");

            PadInt padInt1 = PadiDstm.CreatePadInt(uid2);
            Assert.IsNotNull(padInt1, "CreatePadint returned null for uid:" + uid2);


            int firstRead = padInt1.Read();
            Assert.AreEqual(firstRead, DEFAULT_PADINT_VALUE, String.Format("Read:{0} Expected:{1}", firstRead, DEFAULT_PADINT_VALUE));

            padInt1.Write(WRITE_VALUE);
            int secondRead = padInt1.Read();
            Assert.AreEqual(secondRead, WRITE_VALUE, String.Format("Read:{0} Expected:{1}", secondRead, WRITE_VALUE));
            bool status = PadiDstm.Status();
            Thread.Sleep(10000);
            bool didCommit = PadiDstm.TxCommit();
            

            beginSuccess = PadiDstm.TxBegin();
            PadInt pad = PadiDstm.AccessPadInt(uid2);
            
            int thirdRead = pad.Read();

            Assert.IsTrue(didCommit, "Failed to commit transaction.");
            didCommit = PadiDstm.TxCommit();


            Assert.IsTrue(didCommit, "Failed to commit transaction.");
            Assert.IsTrue(status, "Status operation not done.");


        }

        [ClassCleanup]
        public static void ClassCleanUp() {
            //server.Kill();
           // master.Kill();
        
        }
    }
}
