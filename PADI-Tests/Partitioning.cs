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
using System.Net;
using System.Net.Sockets;

namespace PADI_Tests
    {


    //Test uid partitioning and transfer of files from one server to another

    [TestClass]
    public class Partitioning
    {
        static Process master;
        static Process server1;
        static Process server2;
        
        static String master_ip = GetIp();
        static String master_port = "8080";

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
            String server_port = "8081";
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            master =  Process.Start(@"..\..\..\Master\bin\Debug\Master.exe");
            Thread.Sleep(3000);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\Server\bin\Debug\Server.exe";
            startInfo.Arguments = String.Format("{0} {1} {2}", master_ip, master_port,server_port);
            server1 = Process.Start(startInfo);
         

            //new Thread(new ThreadStart(run)).Start();
            
        }

        public static void StartServer() {
            String server_port = "8082";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\Server\bin\Debug\Server.exe";
            startInfo.Arguments = String.Format("{0} {1} {2}", master_ip, master_port,server_port);
            server2 = Process.Start(startInfo);
        }
         
        [TestMethod]
        public void TestPartitioning()
        {
            
            bool beginSuccess = false;
            int uid2 = -1000; //object 1 uid
            int uid3 = 1000;
            // int uid2 = 2; //object 2 uid
            const int DEFAULT_PADINT_VALUE = 0;
            const int WRITE_VALUE = 5;

            bool result = PadiDstm.Init();

            Assert.IsTrue(result);

            //T1
            beginSuccess = PadiDstm.TxBegin();
            //bool status = PadiDstm.Status();
            Assert.IsTrue(beginSuccess);

            PadInt padInt1 = PadiDstm.CreatePadInt(uid2);
            PadInt padInt2 = PadiDstm.CreatePadInt(uid3);

            Assert.IsNotNull(padInt1);


            int firstRead = padInt1.Read();
            Assert.AreEqual(firstRead, DEFAULT_PADINT_VALUE);


            padInt2.Write(WRITE_VALUE);
            //int secondRead = padInt1.Read();
            //Assert.AreEqual(secondRead, WRITE_VALUE, String.Format("Read:{0} Expected:{1}", secondRead, WRITE_VALUE));
         
            bool didCommit = PadiDstm.TxCommit();

            //bool status = PadiDstm.Status();

            StartServer();

            Console.WriteLine("Depois de arrancar o 2 server");


            
            Thread.Sleep(5000);
            bool status = PadiDstm.Status();
           
            beginSuccess = PadiDstm.TxBegin();

            Assert.IsTrue(beginSuccess);
            
            PadInt pad = PadiDstm.AccessPadInt(uid3);
            Assert.IsNotNull(pad);


            int readAfterStart = pad.Read();
            Assert.AreEqual(readAfterStart, WRITE_VALUE, String.Format("Read:{0} Expected:{1}", readAfterStart, WRITE_VALUE));



            Assert.IsTrue(didCommit, "Failed to commit transaction.");
            didCommit = PadiDstm.TxCommit();

            Assert.IsTrue(didCommit, "Failed to commit transaction.");

            status = PadiDstm.Status();

        }

        [ClassCleanup]
        public static void ClassCleanUp() {
            //server1.Kill();
            //server2.Kill();
            //master.Kill();
        
        }

        }
    }
