using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    class ClientThread
    {
        public Socket client = null;
        int i;
        public ClientThread(Socket k)
        {
            client = k;
        }
        public void ClientService()
        {
            string data = null;
            byte[] bytes = new byte[4096];
            Console.WriteLine("新建连接...");
            try
            {
                while ((i = client.Receive(bytes)) != 0)
                {
                    if (i < 0)
                    {
                        break;
                    }
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("收到数据: {0}", data);
                    data = data.ToUpper();
                    bytes = System.Text.Encoding.ASCII.GetBytes(data);
                    client.Send(bytes);
                }
            }
            catch (System.Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
            client.Close();
            Console.WriteLine("断开连接...");
        }
    }
}
