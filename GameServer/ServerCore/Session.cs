using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        int _disconnected = 0;
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        bool _pending = false;
        object _lock = new object();


        public void Start(Socket socket)
        {
            _socket = socket;

            // [1.Recv]
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // 추가적으로 넘겨주고 싶은 정보 UserToken으로 넘겨준다.
            // recvArgs.UserToken = this;
            // recvBuffer: recv 내용
            recvArgs.SetBuffer(new byte[1024], 0, 1024); // (버퍼 공간, 시작 위치, 크기)

            // [2.Send]
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuffer)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuffer);

                // _pending == false → 데이터를 바로 보낼 수 있는 상태.
                if (_pending == false)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신

        private void RegisterSend()
        {
            _pending = true;
            byte[] buffer = _sendQueue.Dequeue();
            _sendArgs.SetBuffer(buffer, 0, buffer.Length);

            bool pending = _socket.SendAsync(_sendArgs); // 운영체제 커널에서 처리를 한다. // 호출이 많으면 비용이 커진다. // 재사용할 수 있도록 만들면 좋음.
            if (pending == false)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                // 몇 바이트를 보냈는지(Send)
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    // To do
                    try
                    {
                        // 보내야 할 메시지가 있을 때
                        if (_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                        else // 더이상 보내야 할 메시지가 없을 때).
                        {
                            _pending = false;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnRecvCompleted Failed {e}");
                    }
                }
                else
                {
                    // To do Disconnect
                    Disconnect();
                }
            }
        }

        private void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);

            // pending == false → 받을 데이터가 있음
            if (pending == false)
            {
                OnRecvCompleted(null, args);
            }
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 몇 바이트를 받았는지
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                // To do
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");

                    RegisterRecv(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                // To do Disconnect
                Disconnect();
            }
        }
        #endregion
    }
}
