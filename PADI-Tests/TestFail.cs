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
    public class TestFail
    {
        const int SERVER_MAX = 3+1;
        
        const int UMAX = 400;
        const int LMAX = -400;

        int kill = 1 + new Random().Next(SERVER_MAX-1);
        Process master;
        Process [] server = new Process[SERVER_MAX];

        String failedServerURL;

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

            for (int i = 1; i <= SERVER_MAX; i++)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"..\..\..\Server\bin\Debug\Server.exe";
                startInfo.Arguments = String.Format("{0} {1} {2}", master_ip, master_port, server_port + i);
                
                if (i == kill)
                {
                    failedServerURL = String.Format("tcp://{0}:{1}/Server", master_ip, server_port + i);
                }
                
                server[i-1] = Process.Start(startInfo);
                Thread.Sleep(1000);
            }
        }


        [TestMethod]
        public void TestFailServer()
        {
            bool result = PadiDstm.Init();

            bool beginSuccess = PadiDstm.TxBegin();

            PadInt padInt1;
            for (int i = 0; i < UMAX; i++)
            {
                padInt1 = PadiDstm.CreatePadInt(i);
                padInt1.Write(i);
            }

            PadInt padInt2;
            for (int i = LMAX; i < 0; i++)
            {
                padInt2 = PadiDstm.CreatePadInt(i);
                padInt2.Write(i);
            }

            Thread.Sleep(5000);
            PadiDstm.TxCommit();

           // Thread.Sleep(10000);
            //Antes de falhar
            //PadiDstm.Status();
            
            
            //Servers de 1 a 2
            //Fail do server 1
            //try
            //{
            
            server[kill].Kill();
            
            
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("kill exception" + e.Message);
            //}


            //Depois de falhar
            //PadiDstm.Status();


            //server[(kill + 1) % SERVER_MAX].Kill();
            //server[(kill + 2) % SERVER_MAX].Kill();
            
            Thread.Sleep(1000);

            beginSuccess = PadiDstm.TxBegin();


            for (int i = 0; i < UMAX; i++)
            {
                padInt1 = PadiDstm.AccessPadInt(i);
                int i1 = padInt1.Read();
                Assert.AreEqual(i1, i);
                padInt1.Write(i);
            }
            
            for (int i = LMAX; i < 0; i++)
            {
                padInt2 = PadiDstm.AccessPadInt(i);
                int i2 = padInt2.Read();
                Assert.AreEqual(i2,i);
                padInt2.Write(i);
            }
           
            PadiDstm.TxCommit();


            server[(kill + 2) % SERVER_MAX].Kill();


            beginSuccess = PadiDstm.TxBegin();


            for (int i = 0; i < UMAX; i++)
            {
                padInt1 = PadiDstm.AccessPadInt(i);
                int i1 = padInt1.Read();
                Assert.AreEqual(i1, i);
                padInt1.Write(i);
            }

            for (int i = LMAX; i < 0; i++)
            {
                padInt2 = PadiDstm.AccessPadInt(i);
                int i2 = padInt2.Read();
                Assert.AreEqual(i2, i);
                padInt2.Write(i);
            }

            PadiDstm.TxCommit();

            //server[(kill + 3) % SERVER_MAX].Kill();



        }


        [TestMethod]
        public void TestFailWhileInTransaction()
        {

            const int UID1 = 10;
            const int UID2 = -10;
            const int WRITE = 5;

            const int MAX = 100;
            
            bool result = PadiDstm.Init();

            
            bool beginSuccess = PadiDstm.TxBegin();
            
            PadInt padInt1 = PadiDstm.CreatePadInt(UID1);
            padInt1.Write(WRITE);
            

            PadInt padInt2 = PadiDstm.CreatePadInt(UID2);
            padInt2.Write(WRITE);


            //Servers de 1 a 2
            //Fail do server 1

            server[kill].Kill();


            Thread.Sleep(3000);
            PadiDstm.TxCommit();

            //Depois de falhar
            PadiDstm.Status();

            //PadiDstm.Fail(failedServerURL);
            bool f = PadiDstm.Freeze(failedServerURL);
            Assert.IsFalse(f);
            bool r = PadiDstm.Recover(failedServerURL);
            Assert.IsFalse(r);
           
            //Server falhou antes de fazer commit de t1,
            //logo deve 

            Thread.Sleep(1000);

            //Server nao consegui fazer commit
            //fez abort dos creates
            //access deve retornar null
            beginSuccess = PadiDstm.TxBegin();
            padInt1 = PadiDstm.AccessPadInt(UID1);
            Assert.IsNull(padInt1);


            padInt2 = PadiDstm.AccessPadInt(UID2);
            Assert.IsNull(padInt2);
            PadiDstm.TxCommit();


        }



        [TestMethod]
        public void TestFailWhileInTransactionWithRead()
        {

            const int UID1 = 10;
            const int UID2 = -10;
            const int WRITE = 5;

            bool result = PadiDstm.Init();

            bool beginSuccess = PadiDstm.TxBegin();
            PadInt padInt1 = PadiDstm.CreatePadInt(UID1);
            padInt1.Write(WRITE);

            PadInt padInt2 = PadiDstm.CreatePadInt(UID2);
            padInt1.Write(WRITE);


            //Servers de 1 a 2
            //Fail do server 1

            server[kill].Kill();


            Thread.Sleep(1000);
            PadiDstm.TxCommit();

            //Depois de falhar
            PadiDstm.Status();


            //Server falhou antes de fazer commit de t1,
            //logo deve 

            Thread.Sleep(1000);

            //Server nao consegui fazer commit
            //fez abort dos creates
            //access deve retornar null
            //A seguir cria pad ints e escreve
            beginSuccess = PadiDstm.TxBegin();
            padInt1 = PadiDstm.AccessPadInt(UID1);
            Assert.IsNull(padInt1);
            
            padInt1 = PadiDstm.CreatePadInt(UID1);
            Assert.IsNotNull(padInt1);
            padInt1.Write(WRITE);

            padInt2 = PadiDstm.AccessPadInt(UID2);
            Assert.IsNull(padInt2);

            padInt2 = PadiDstm.CreatePadInt(UID2);
            Assert.IsNotNull(padInt2);
            padInt2.Write(WRITE);

            PadiDstm.TxCommit();


            //Desta vez vai ter sucesso a 
            //aceder
            beginSuccess = PadiDstm.TxBegin();

            padInt1 = PadiDstm.AccessPadInt(UID1);
            Assert.IsNotNull(padInt1);
            int r1 = padInt1.Read();
            Assert.AreEqual(r1, WRITE);

            padInt2 = PadiDstm.AccessPadInt(UID2);
            Assert.IsNotNull(padInt2);
            int r2 = padInt2.Read();
          
            Assert.AreEqual(r2, WRITE);
            
            PadiDstm.TxCommit();


        }
    }
}
