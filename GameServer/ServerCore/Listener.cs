using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Listener
    {
        private Socket _listenSocket;
        private Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            // 문지기          
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            // backlog: 최대 대기수(10명)
            _listenSocket.Listen(10);

            
            // 이벤트 생성
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            // 이벤트 등록
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); // 이벤트 콜백
            RegisterAccept(args);
        }

        private void RegisterAccept(SocketAsyncEventArgs args)
        {
            // SocketAsyncEventArgs AcceptSocket 기존의 값을 초기화한다.
            args.AcceptSocket = null;

            // 비동기 - non blocking
            bool pending = _listenSocket.AcceptAsync(args);

            // pending 없이 바로 완료됨 (pending == false → 바로 처리가 되는 경우)
            if (pending == false)
                OnAcceptCompleted(null, args);
        }

        // [이벤트] - 멀티 쓰레드로 실행될 수 있다. (주 쓰레드, 작업 쓰레드에서 동시에 수행되면 Data Race가 발생될 수 있음.)
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 소켓 에러 성공여부
            if(args.SocketError == SocketError.Success)
            {
                // To do(소켓이 성공하여 ~을 수행한다.)
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args);
        }

        public Socket Accept()        
        {
            return _listenSocket.Accept();
        }
    }
}
