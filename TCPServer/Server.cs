using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServer
{
    class Server
    {
        Socket server;
        public void Start(int port)
        {
            IPAddress local = IPAddress.Any;
            IPEndPoint iep = new IPEndPoint(local, 9000);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(20);

            Thread tcpThread = new Thread(new ThreadStart(TcpListen));
            tcpThread.Start();
        }
        private void TcpListen()
        {
            while (true)
            {
                try
                {
                    Socket client = server.Accept();
                    ClientThread newClient = new ClientThread(client);
                    Thread newThread = new Thread(new ThreadStart(newClient.ClientService));
                    newThread.Start();
                }
                catch {
                    Console.WriteLine("服务中断...");
                }
            }
        }

    }
}
