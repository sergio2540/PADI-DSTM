using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PADI_DSTM;
using CommonTypes;
using Master;
using System.Threading;
using Server;

namespace PADI_Tests
{
    /*Tests if the committed are available one a next access to the padint.
     
     */

    [TestClass]
    public class ReadAfterCommit
    {

        [TestInitialize]
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
            int uid2 = 2; //object 1 uid
            // int uid2 = 2; //object 2 uid
            const int DEFAULT_PADINT_VALUE = 0;
            const int WRITE_VALUE = 5;
            DSTMManager manager = new DSTMManager();
            manager.Init();


            //T1
            beginSuccess = manager.TxBegin();

            Assert.IsTrue(beginSuccess, "Failed to begin transaction.");

            PadInt padInt1 = manager.CreatePadInt(uid2);
            Assert.IsNotNull(padInt1, "CreatePadint returned null for uid:" + uid2);


            int firstRead = padInt1.Read();
            Console.WriteLine("First read: " + firstRead);
            Assert.AreEqual(firstRead, DEFAULT_PADINT_VALUE, String.Format("Read:{0} Expected:{1}", firstRead, DEFAULT_PADINT_VALUE));

            padInt1.Write(3);
            int secondRead = padInt1.Read();
            Console.WriteLine("Second read: " + secondRead);
            Assert.AreEqual(secondRead, WRITE_VALUE, String.Format("Read:{0} Expected:{1}", secondRead, WRITE_VALUE));

            bool didCommit = manager.TxCommit();

            PadInt pad = manager.AccessPadInt(uid2);
            int thirdRead = pad.Read();
            Console.WriteLine("Third read: " + thirdRead);
            Assert.IsTrue(didCommit, "Failed to commit transaction.");


        }
    }
}
