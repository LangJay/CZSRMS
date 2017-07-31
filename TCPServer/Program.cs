using System;
using System.Net;
using System.Net.Sockets;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //定义接收数据长度变量
            int recv;
            //定义接收数据的缓存
            byte[] data = new byte[1024];
            //定义侦听端口
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 9000);
            //定义套接字类型
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //连接
            socket.Bind(ipEnd);
            //开始侦听
            socket.Listen(10);
           
            while (true)
            {
                //控制台输出侦听状态
                Console.WriteLine("等待客户端连接...");
                //一旦接受连接，创建一个客户端
                Socket client = socket.Accept();
                //获取客户端的IP和端口
                IPEndPoint ipEndClient = (IPEndPoint)client.RemoteEndPoint;
                //输出客户端的IP和端口
                Console.WriteLine("Connect with {0} at port {1}", ipEndClient.Address, ipEndClient.Port);
                //定义待发送字符
                string welcome = "Welcome to my server";
                //数据类型转换
                data = System.Text.Encoding.ASCII.GetBytes(welcome);
                //发送
                client.Send(data, data.Length, SocketFlags.None);
                try
                {

                    while (true)
                    {
                        //对data清零
                        data = new byte[1024];

                        //获取收到的数据的长度
                        recv = client.Receive(data);
                        //如果收到的数据长度为0，则退出
                        if (recv != 0)
                        {
                            string str = System.Text.Encoding.ASCII.GetString(data, 0, recv);
                            //输出接收到的数据
                            Console.WriteLine("从客户端接收：" + str);
                            if(str == "exit")
                            {
                                Console.WriteLine("Disconnect form{0}", ipEndClient.Address);
                                client.Close();
                                break;
                            }

                            //将接收到的数据再发送出去
                            client.Send(data, recv, SocketFlags.None);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Disconnect form{0}", ipEndClient.Address);
                    client.Close();
                }
                Console.WriteLine("Disconnect form{0}", ipEndClient.Address);
                client.Close();
            }
        }
    }
}
