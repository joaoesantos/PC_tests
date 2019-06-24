using System;
using System.Threading;

namespace _15_16v_1
{
    class UnsafeCountDownLatch
    {
        private static readonly int WAIT_GRAIN = 10;
        private int count;
        public UnsafeCountDownLatch(int initial) { if (initial > 0) count = initial; }
        public void signal() {
            if (count > 0) {
                int c;
                do
                {
                    c = count;
                } while (Interlocked.CompareExchange(ref count, c - 1, c) != c);
            }
            else throw new InvalidOperationException();
        }
    public bool await(long timeout) {
            if (timeout < 0)
            {
                timeout = long.MaxValue;
            }
       for (; count > 0 && timeout > 0; timeout =WAIT_GRAIN)
            Thread.SpinWait(WAIT_GRAIN);
       return count == 0;
    }
}
}
