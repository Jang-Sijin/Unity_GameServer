using System;
using System.Threading;
using System.Threading.Tasks;

namespace SeverCore
{
    // release mode일 때 컴파일러가 자동 코드 최적화를 진행한다는 점을 주의하자.
    class CompilerOptimized
    {
        volatile static bool _stop = false; // 캐시를 무시하고 최신 값을 가져와라. // C#에서는 사용하지 말아라. (전문가들이 비추천)

        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작!");

            while(_stop == false)
            {
                // 누군가가 stop 신호를 보내주기를 기다린다.
            }

            Console.WriteLine("쓰레드 종료!");
        }

        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중");

            t.Wait();
            Console.WriteLine("종료 성공");
        }
    }
}