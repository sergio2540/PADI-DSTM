using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PADI_DSTM;
using CommonTypes;
using Server;
using Master;
using System.Threading;

namespace PADI_Tests
{
    /*
     UnitTest1 creates a padint with uid1. Then, that padint is written and then is read.
     *No effects are committed. The transaction is then aborted.
     */

    [TestClass]
    public class BasicReadWrite
    {

        [ClassInitialize]
        public void TestInitialize()
        {
            MasterApp.Main(null);
            Thread.Sleep(1000);
            ServerApp.Main(null);

        }
        
        
        [TestMethod]
        public void TestMethod1()
        {
            bool beginSuccess = false;
            int uid1 = 1; //object 1 uid
           // int uid2 = 2; //object 2 uid
            const int DEFAULT_PADINT_VALUE = 0; 
            const int WRITE_VALUE = 3;
            PadiDstm.Init();
        
               
                //T1
                beginSuccess = PadiDstm.TxBegin();

                Assert.IsTrue(beginSuccess,"Failed to begin transaction.");
  
                PadInt padInt1 = PadiDstm.CreatePadInt(uid1);
                Assert.IsNotNull(padInt1, "CreatePadint returned null for uid:" + uid1);

                
                int firstRead = padInt1.Read();
                Console.WriteLine("First read: " + firstRead);
                Assert.AreEqual(firstRead, DEFAULT_PADINT_VALUE, String.Format("Read:{0} Expected:{1}",firstRead,DEFAULT_PADINT_VALUE));

                padInt1.Write(3);
                int secondRead = padInt1.Read();
                Console.WriteLine("Second read: " + secondRead);
                Assert.AreEqual(secondRead,WRITE_VALUE, String.Format("Read:{0} Expected:{1}",secondRead,WRITE_VALUE));

                PadInt pad = PadiDstm.AccessPadInt(uid1);
                int thirdRead = pad.Read();
                Console.WriteLine("Third read: " + thirdRead);
                Assert.AreEqual(thirdRead, DEFAULT_PADINT_VALUE, "Read:{0} Expected:{1}", thirdRead, DEFAULT_PADINT_VALUE);

                bool didAbort = PadiDstm.TxAbort();
                Assert.IsTrue(didAbort, "Failed to abort transaction. One or more of the participants rejected abort.");

               
        }


        [ClassCleanup]
        public void TestCleanUp() { }

    }
}
