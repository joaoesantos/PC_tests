using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _14_15i_2
{
    public class UnsafeSpinSemaphore
    {
        private readonly int maximum;
        private volatile int count;
        public UnsafeSpinSemaphore(int i, int m)
        {
            if (i < 0 || m <= 0)
                throw new ArgumentException("i / m");
            count = i; maximum = m;
        }

        public void Acquire()
        {
            SpinWait sw = new SpinWait();
            int c;
            do
            {
                if ((c = count) == 0)
                    sw.SpinOnce();
            } while (Interlocked.CompareExchange(ref count, c - 1, c) != c);
        }

        public void Release(int rs)
        {
            int c;

            do
            {
                c = count;
                if (rs < 0 || rs + c > maximum)
                    throw new ArgumentException("rs");
            } while (Interlocked.CompareExchange(ref count, c + rs, c) != c);
        }
    }
}
