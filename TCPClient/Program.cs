using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //定义发送数据缓存
            byte[] data = new byte[1024];
            //定义字符串，用于控制台输出或输入
            string input, stringData;
            //定义主机的IP及端口
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Console.WriteLine("请输入服务器监听的端口：");
            while (int.TryParse(Console.ReadLine(), out int port))
            {
                IPEndPoint ipEnd = new IPEndPoint(ip, port);
                //定义套接字类型
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //尝试连接
                try
                {
                    socket.Connect(ipEnd);
                }
                //异常处理
                catch (SocketException e)
                {
                    Console.Write("连接失败...");
                    Console.Write(e.ToString());
                    return;
                }
                //定义接收数据的长度
                int recv = socket.Receive(data);
                //将接收的数据转换成字符串
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                //控制台输出接收到的数据
                Console.WriteLine(stringData);
              

                while (true)
                {
                    Console.WriteLine("请输入发送的内容：");
                    //定义从键盘接收到的字符串
                    input = Console.ReadLine();
                    //如果字符串是"exit"，退出while循环
                    if (input == "exit")
                    {
                        break;
                    }
                    //对data清零
                    data = new byte[1024];
                    //将从键盘获取的字符串转换成整型数据并存储在数组中    
                    data = Encoding.ASCII.GetBytes(input);
                    //发送该数组
                    socket.Send(data, data.Length, SocketFlags.None);

                   
                    //对data清零
                    data = new byte[1024];
                    //定义接收到的数据的长度
                    recv = socket.Receive(data);
                    //将接收到的数据转换为字符串
                    stringData = Encoding.ASCII.GetString(data, 0, recv);
                    //控制台输出字符串
                    Console.WriteLine("从服务器接收：" + stringData);
                    //发送收到的数据
                    socket.Send(data, recv, 0);

                }
                Console.Write("断开连接...");
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
}
}
