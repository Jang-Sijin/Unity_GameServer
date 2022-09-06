using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static void MainThread(object state)
        {           
            while(true)
                Console.WriteLine("Hello Thread!");         
        }

        static void Main(string[] args)
        {
            //ThreadPool.SetMinThreads(1, 1);
            //ThreadPool.SetMaxThreads(5, 5);

            // 일감 단위를 정의해서 사용하겠다? // 어떤 쓰레드가 먼저 실행 될 것인지는 모른다. 스레드 스케줄링 정책에 따라 실행됨.
            //for (int i = 0; i < 5; ++i)
            //{            
            //    Task task = new Task(() => { Console.WriteLine(i); while (true) { } }, TaskCreationOptions.LongRunning); // TaskCreationOptions.LongRunning: 오래걸리는 작업이라고 명시. // 별도 처리
            //    // TaskCreationOptions.LongRunning가 없다면 ThreadPool.QueueUserWorkItem(MainThread);가 실행되지 않음. 모든 쓰레드를 점유하고 있기 때문.
            //    task.Start();
            //}

            //쓰레드 풀 static 함수들로 이루어져 있음. 닷넷에서 제공해주는 쓰레드. // 쓰레드 풀링
            //for(int i = 0; i < 4; ++i)
            //    ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });

            ThreadPool.QueueUserWorkItem(MainThread);

            //Thread t = new Thread(MainThread);
            //t.Name = "Test Thread"; // 쓰레드에 이름을 설정해줄 수 있다.
            //t.IsBackground = true; // true: 메인 쓰레드가 종료가 되면 같이 종료, false: 메인 쓰레드가 종료되어도 계속 수행(default)
            //t.Start(); // 쓰레드 시작.
            Console.WriteLine("Waiting for Thread!");

            //t.Join(); // Join이 있을 경우 메인 쓰레드가 자식들을 기다리지 않고 종료 // Main이 끝나면 쓰레드에서 수행중인 코드들이 중지되고 프로그램 종료.
            //Console.WriteLine("Hello World!");
        }
    }
}
