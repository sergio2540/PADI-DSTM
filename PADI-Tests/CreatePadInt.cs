using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;
using Services_DSTM;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Net.Sockets;
using CommonTypes;

namespace PADI_Tests
{
    [TestClass]
    public class CreatePadInt
    {

        static Process master;
        static Process server;
        private const int PAD_ID = 20;
        static InstanceProxy proxy;

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

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\Server\bin\Debug\Server.exe";
            startInfo.Arguments = String.Format("{0} {1} {2}", master_ip, master_port, server_port);
            server = Process.Start(startInfo);

            Thread.Sleep(1000);

            proxy = new InstanceProxy();
            proxy.Init();

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
        public void FailOnDuplicate()
        {

            PadInt pad = proxy.CreatePadInt(PAD_ID);

            Assert.IsNotNull(pad, "Failed to create first padInt");

            pad = proxy.CreatePadInt(PAD_ID);

            Assert.IsNull(pad, "Duplicated padint created");

        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            server.Kill();
            master.Kill();

        }



    }
}
