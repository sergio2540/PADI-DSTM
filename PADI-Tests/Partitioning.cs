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
