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
    /*Tests if the committed are available one a next access to the padint.
     
     */

    [TestClass]
    public class CheckLoadBalancing
    {
        static Process master;
        static Process server;


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
            int server_port = 8080;

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            master = Process.Start(@"..\..\..\Master\bin\Debug\Master.exe");
            Thread.Sleep(1000);

            //2 Servidores
            for (int i = 1; i < 3; i++)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"..\..\..\Server\bin\Debug\Server.exe";
                startInfo.Arguments = String.Format("{0} {1} {2}", master_ip, master_port, server_port + i);
                server = Process.Start(startInfo);
                Thread.Sleep(1000);
            }




        }

        [TestMethod]
        public void TestCheckLoadBalancing()
        {

            bool beginSuccess = false;

            const int MAX_UID = 500;
            const int DEFAULT_PADINT_VALUE = 0;
            const int WRITE_VALUE = 5;

            bool result = PadiDstm.Init();

            Assert.IsTrue(result, "Failed to load library.");

            for (int i = 0; i < MAX_UID; i++)
            {
                beginSuccess = PadiDstm.TxBegin();

                Assert.IsTrue(beginSuccess, "Failed to begin transaction.");

                PadInt padInt1 = PadiDstm.CreatePadInt(i);
                Assert.IsNotNull(padInt1, "CreatePadint returned null for uid:" + i);


                int firstRead = padInt1.Read();
                Assert.AreEqual(firstRead, DEFAULT_PADINT_VALUE, String.Format("Read:{0} Expected:{1}", firstRead, DEFAULT_PADINT_VALUE));

                padInt1.Write(WRITE_VALUE);
                int secondRead = padInt1.Read();
                Assert.AreEqual(secondRead, WRITE_VALUE, String.Format("Read:{0} Expected:{1}", secondRead, WRITE_VALUE));
             

                bool didCommit = PadiDstm.TxCommit();
                //Thread.Sleep(1000);
            }

            bool status = PadiDstm.Status();

        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            //server.Kill();
            //master.Kill();

        }
    }
}
