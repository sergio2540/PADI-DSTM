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
using System.Net;
using System.Net.Sockets;


namespace PADI_Tests.CheckPoint
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Cicle
    {

        static Process master;
        static Process server;
        private const int PAD_ID1 = 20;
        private const int PAD_ID2 = 30;

        private static string GetIp()
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress ipv4Addresses = Array.FindLast(ipEntry.AddressList, x => x.AddressFamily == AddressFamily.InterNetwork);
            return ipv4Addresses.ToString();

        }


        [ClassInitialize]
        public static void TestInitialize(TestContext c)
        {
            String master_ip = GetIp();
            String master_port = "8080";
            String server_port = "8081";

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            master = Process.Start(@"..\..\..\Master\bin\Debug\Master.exe");
            Thread.Sleep(2000);
            //for (int i = 0; i < 20; i++)

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\Server\bin\Debug\Server.exe";
            startInfo.Arguments = String.Format("{0} {1} {2}", master_ip, master_port, server_port);
            server = Process.Start(startInfo);
            Thread.Sleep(2000);



        }

        public Cicle()
        {
            //
            // TODO: Add constructor logic here
            //
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CicleTest()
        {
            bool res = false; int aborted = 0, committed = 0;

            PadiDstm.Init();
            try
            {
                //if ((args.Length > 0) && (args[0].Equals("C"))) {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(2);
                Assert.IsNotNull(pi_a, "Padint a failed to create.");
                PadInt pi_b = PadiDstm.CreatePadInt(2000000001);
                Assert.IsNotNull(pi_b, "Padint b failed to create.");
                PadInt pi_c = PadiDstm.CreatePadInt(1000000000);
                Assert.IsNotNull(pi_c, "Padint c failed to create.");
                pi_a.Write(0);
                pi_b.Write(0);
                res = PadiDstm.TxCommit();
                Assert.IsTrue(res, "Failed to commit first transaction.");
                //}
                Console.WriteLine("####################################################################");
                Console.WriteLine("Finished creating PadInts. Press enter for 300 R/W transaction cycle.");
                Console.WriteLine("####################################################################");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
                Console.WriteLine("####################################################################");
                PadiDstm.TxAbort();
                Assert.Fail("Failed to commit first transaction.");
            }
            int previousD = 0;
            int previousE = 0;
            int previousF = 0;
            for (int i = 0; i < 300; i++)
            {
                try
                {
                    res = PadiDstm.TxBegin();
                    PadInt pi_d = PadiDstm.AccessPadInt(2);
                    Assert.IsNotNull(pi_d, "Padint a failed to access.");
                    PadInt pi_e = PadiDstm.AccessPadInt(2000000001);
                    Assert.IsNotNull(pi_e, "Padint e failed to access.");
                    PadInt pi_f = PadiDstm.AccessPadInt(1000000000);
                    Assert.IsNotNull(pi_f, "Padint f failed to access.");

                    int d = pi_d.Read();
                    Assert.AreEqual(previousD, d, "Object d does not have a consistent value.");
                    d++;
                    pi_d.Write(d);
                    previousD = d;
                    int e = pi_e.Read();
                    Assert.AreEqual(previousE, e, "Object e does not have a consistent value.");
                    e++;
                    pi_e.Write(e);
                    previousE = e;
                    int f = pi_f.Read();
                    Assert.AreEqual(previousF, f, "Object f does not have a consistent value.");
                    f++;
                    pi_f.Write(f);
                    previousF = f;
                    Console.Write(".");
                    res = PadiDstm.TxCommit();
                    Assert.IsTrue(res, "Failed to commit second transaction.");
                    if (res) { committed++; Console.Write("."); }
                    else
                    {
                        aborted++;
                        Console.WriteLine("$$$$$$$$$$$$$$ ABORT $$$$$$$$$$$$$$$$$");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    Console.WriteLine("####################################################################");
                    Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
                    Console.WriteLine("####################################################################");
                    PadiDstm.TxAbort();
                    aborted++;
                }

            }
            Console.WriteLine("####################################################################");
            Console.WriteLine("committed = " + committed + " ; aborted = " + aborted);
            Console.WriteLine("Status after cycle. Press enter for verification transaction.");
            Console.WriteLine("####################################################################");
            PadiDstm.Status();

            try
            {
                res = PadiDstm.TxBegin();
                PadInt pi_g = PadiDstm.AccessPadInt(2);
                PadInt pi_h = PadiDstm.AccessPadInt(2000000001);
                PadInt pi_j = PadiDstm.AccessPadInt(1000000000);
                int g = pi_g.Read();
                Assert.AreEqual(previousD, g, "Object g does not have a consistent value.");
                int h = pi_h.Read();
                Assert.AreEqual(previousE, h, "Object d does not have a consistent value.");
                int j = pi_j.Read();
                Assert.AreEqual(previousF, j, "Object d does not have a consistent value.");
                res = PadiDstm.TxCommit();
                Assert.IsTrue(res, "Failed to commit last transaction.");
                Console.WriteLine("####################################################################");
                Console.WriteLine("2 = " + g);
                Console.WriteLine("2000000001 = " + h);
                Console.WriteLine("1000000000 = " + j);
                Console.WriteLine("Status post verification transaction. Press enter for exit.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
                Console.WriteLine("####################################################################");
                PadiDstm.TxAbort();
                Assert.IsTrue(res, "Failed to commit last transaction.");
            }
        }
    }
}
