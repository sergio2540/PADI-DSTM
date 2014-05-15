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
    [TestClass]
    public class TestCreate
    {
        static int SERVER_MAX = 3;
        Process master;
        Process[] server = new Process[SERVER_MAX];

        private string GetIp()
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress ipv4Addresses = Array.FindLast(ipEntry.AddressList, x => x.AddressFamily == AddressFamily.InterNetwork);
            return ipv4Addresses.ToString();

        }


        [TestInitialize]
        public void TestSetup()
        {
            String master_ip = GetIp();
            String master_port = "8080";
            int server_port = 8080;

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            master = Process.Start(@"..\..\..\Master\bin\Debug\Master.exe");
            Thread.Sleep(1000);

            for (int i = 1; i < SERVER_MAX; i++)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"..\..\..\Server\bin\Debug\Server.exe";
                startInfo.Arguments = String.Format("{0} {1} {2}", master_ip, master_port, server_port + i);
                server[i] = Process.Start(startInfo);
                Thread.Sleep(1000);
            }
        }


        [TestMethod]
        public void TestCreateTransaction()
        {
            const int UID1 =  10;
            const int UID2 = -10;
            const int WRITE = 5;
    
            bool result = PadiDstm.Init();

            //Falha create
            bool beginSuccess = PadiDstm.TxBegin();
            PadInt padInt1 = PadiDstm.CreatePadInt(UID1);
            padInt1.Write(WRITE);
            PadInt padInt2 = PadiDstm.CreatePadInt(UID2);
            padInt1.Write(WRITE);
            PadiDstm.TxAbort();


            //Falha access
        
            beginSuccess = PadiDstm.TxBegin();
            padInt1 = PadiDstm.AccessPadInt(UID1);
            Assert.IsNull(padInt1);
            padInt2 = PadiDstm.AccessPadInt(UID2);
            Assert.IsNull(padInt2);
            PadiDstm.TxCommit();


            //Pode criar
            beginSuccess = PadiDstm.TxBegin();
            padInt1 = PadiDstm.CreatePadInt(UID1);
            padInt1.Write(WRITE);
            padInt2 = PadiDstm.CreatePadInt(UID2);
            padInt2.Write(WRITE);
            PadiDstm.TxCommit();

            //Pode ler
            beginSuccess = PadiDstm.TxBegin();
            Assert.IsTrue(beginSuccess);

            padInt1 = PadiDstm.AccessPadInt(UID1);
            Assert.IsNotNull(padInt1);
            int r1 = padInt1.Read();
            Assert.AreEqual(r1,WRITE);

            padInt2 = PadiDstm.AccessPadInt(UID2);
            Assert.IsNotNull(padInt2);
            int r2 = padInt2.Read();
            Assert.AreEqual(r2, WRITE);
            
            PadiDstm.TxCommit();


            //Ja esta criado
            beginSuccess = PadiDstm.TxBegin();
            padInt1 = PadiDstm.CreatePadInt(UID1);
            Assert.IsNull(padInt1);
            padInt2 = PadiDstm.CreatePadInt(UID2);
            Assert.IsNull(padInt1);
            PadiDstm.TxCommit();


        }
    }
}