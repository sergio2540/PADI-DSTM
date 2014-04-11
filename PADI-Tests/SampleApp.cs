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
    public class SampleApp
    {
        static Process master;
        static Process server;
        static String[] servers_port = { "8081", "8082" };
           

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
            
           

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            master = Process.Start(@"..\..\..\Master\bin\Debug\Master.exe");
            Thread.Sleep(2000);

            for (int i = 0; i < 2; i++)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"..\..\..\Server\bin\Debug\Server.exe";
                startInfo.Arguments = String.Format("{0} {1} {2}", master_ip, master_port,servers_port[i]);
                server = Process.Start(startInfo);
                Thread.Sleep(2000);
            }




        }

        [TestMethod]
        public void TestSampleApp()
        {
            bool res;

            PadiDstm.Init();

            res = PadiDstm.TxBegin();
            PadInt pi_a = PadiDstm.CreatePadInt(0);
            PadInt pi_b = PadiDstm.CreatePadInt(1);
            res = PadiDstm.TxCommit();

            res = PadiDstm.TxBegin();
            pi_a = PadiDstm.AccessPadInt(0);
            pi_b = PadiDstm.AccessPadInt(1);

            pi_a.Write(36);
            pi_b.Write(37);

            int av = pi_a.Read();
            int bv = pi_b.Read();

            Assert.AreEqual(av, 36);
            Assert.AreEqual(bv, 37);


            PadiDstm.Status();

            // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
            //res = PadiDstm.Freeze(String.Format("tcp://localhost:{0}/Server", servers_port[0]));
            //res = PadiDstm.Recover(String.Format("tcp://localhost:{0}/Server", servers_port[0]));
            //res = PadiDstm.Fail(String.Format("tcp://localhost:{0}/Server", servers_port[1]));

            res = PadiDstm.TxCommit();



        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            //server.Kill();
            //master.Kill();

        }
    }
}
