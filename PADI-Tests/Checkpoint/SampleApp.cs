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

namespace PADI_Tests
{
    /// <summary>
    /// Summary description for SampleApp
    /// </summary>
    [TestClass]
    public class SampleApp
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
            Thread.Sleep(1000);
            //for (int i = 0; i < 20; i++)
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\Server\bin\Debug\Server.exe";
            startInfo.Arguments = String.Format("{0} {1} {2}", master_ip, master_port, server_port);
            server = Process.Start(startInfo);




        }

        public SampleApp()
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
        public void TestSampleApp()
        {
            bool res = false;
            PadInt pi_a, pi_b;
            PadiDstm.Init();

            // Create 2 PadInts
            //if ((args.Length > 0) && (args[0].Equals("C")))
            //{
                try
                {
                    res = PadiDstm.TxBegin();
                    Assert.IsTrue(res,"Failed to begin first transaction.");
                    pi_a = PadiDstm.CreatePadInt(1);
                    Assert.IsNotNull(pi_a, "Failed to create padint a.");
                    pi_b = PadiDstm.CreatePadInt(2000000000);
                    Assert.IsNotNull(pi_b, "Failed to create padint b.");

                    Console.WriteLine("####################################################################");
                    Console.WriteLine("BEFORE create commit. Press enter for commit.");
                    Console.WriteLine("####################################################################");
                    PadiDstm.Status();
                    res = PadiDstm.TxCommit();
                    Console.WriteLine("####################################################################");
                    Console.WriteLine("AFTER create commit returned " + res + " . Press enter for next transaction.");
                    Console.WriteLine("####################################################################");
                    Assert.IsTrue(res, "Failed to commit first transaction.");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    Console.WriteLine("####################################################################");
                    Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for next transaction.");
                    Console.WriteLine("####################################################################");
                    PadiDstm.TxAbort();
                    Assert.Fail("Failed to commit first transaction.");
                }
            //}

            try
            {
                res = PadiDstm.TxBegin();
                Assert.IsTrue(res, "Failed to begin second transaction.");
                pi_a = PadiDstm.AccessPadInt(1);
                Assert.IsNotNull(pi_a, "Failed to access padint a.");
                pi_b = PadiDstm.AccessPadInt(2000000000);
                Assert.IsNotNull(pi_b, "Failed to access padint b.");
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status after AccessPadint");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                //if ((args.Length > 0) && ((args[0].Equals("C")) || (args[0].Equals("A"))))
                //{
                    pi_a.Write(11);
                    pi_b.Write(12);
                //}
                //else
                //{
                    pi_a.Write(21);
                    pi_b.Write(22);
                //}
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status after write. Press enter for read.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.WriteLine("1 = " + pi_a.Read());
                Console.WriteLine("2000000000 = " + pi_b.Read());
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status after read. Press enter for commit.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                res = PadiDstm.TxCommit();
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status after commit. commit = " + res + "Press enter for verification transaction.");
                Console.WriteLine("####################################################################");
                Assert.IsTrue(res, "Failed to commit second transaction");
            }
            catch (Exception e)

            {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER r/w ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
                Console.WriteLine("####################################################################");
                PadiDstm.TxAbort();
                Assert.Fail("Failed to commit second transaction");

            }

            try
            {
                res = PadiDstm.TxBegin();
                Assert.IsTrue(res, "Failed to begin third transaction");
                PadInt pi_c = PadiDstm.AccessPadInt(1);
                Assert.IsNotNull(pi_c, "Failed to access padint 1 on third transaction.");
                PadInt pi_d = PadiDstm.AccessPadInt(2000000000);
                Assert.IsNotNull(pi_d, "Failed to access padint 2000000000 on third transaction.");
                Console.WriteLine("####################################################################");
                Console.WriteLine("1 = " + pi_c.Read());
                Console.WriteLine("2000000000 = " + pi_d.Read());
                Console.WriteLine("Status after verification read. Press enter for commit and exit.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                res = PadiDstm.TxCommit();
                Assert.IsTrue(res, "Failed to commit third transaction.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER verification ABORT. Commit returned " + res + " . Press enter for abort and exit.");
                Console.WriteLine("####################################################################");
                PadiDstm.TxAbort();
                Assert.Fail("Failed to commit third transaction.");

            }
        }
    }
}
