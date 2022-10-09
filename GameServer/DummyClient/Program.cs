using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // DNS (Domain Name System)           
            // 도메인 등록 후 주소를 찾는다.
            // IP주소 생성
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 포트번호: 7777 부여

            while (true)
            {
                // 휴대폰 설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // 문지기한테 입장 문의
                    socket.Connect(endPoint); // 블록킹됨
                    Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()}");

                    // 보낸다. (5회)
                    for (int i = 0; i < 5; ++i)
                    {
                        byte[] sendBuffer = Encoding.UTF8.GetBytes($"Hello World! {i}");
                        int sendBytes = socket.Send(sendBuffer); // 블록킹됨
                    }

                    // 받는다.
                    byte[] recvBuffer = new byte[1024];
                    int recvBytes = socket.Receive(recvBuffer); // 블록킹됨
                    string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);
                    Console.WriteLine($"[From Server] {recvData}");

                    // 나간다.
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Thread.Sleep(100);
        }
    }
}
