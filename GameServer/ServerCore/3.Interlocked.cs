using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static int number = 0;

        // atomic: 원자성(더 이상 쪼갤 수 없는 작업.) // 어떤 동작이 한 번에 일어나야 한다.
        // DB에서도 자칫 잘못하면 비정상적인 아이템 복사, 삭제가 되버릴 수 있다.
        static void Thread_1()
        {            
            for (int i = 0; i < 10000; ++i)
            {
                // All or Nothing
                // Interlocked는 lock의 하위 호환 같은 느낌으로, 하나의 변수에 대해 atomic한 연산을 보장합니다.          
                Interlocked.Increment(ref number);
            }
        }

        static void Thread_2()
        {           
            for (int i = 0; i < 10000; ++i)
            {
                Interlocked.Decrement(ref number);
            }
        }

        static void Main(String[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
       }
    }
}
