using System;
using System.Threading;

namespace _16_17i_1
{
    public class SafeCyclicBarrier
    {
        private readonly int partners;
        private volatile int remaining, currentPhase;
        public SafeCyclicBarrier(int partners)
        {
            if (partners <= 0) throw new ArgumentOutOfRangeException();
            this.partners = this.remaining = partners;
        }
        public void signalAndAwait()
        {
            int phase = currentPhase;
            int r = remaining;
            if (r == 0) throw new InvalidOperationException();
            while (Interlocked.CompareExchange(ref remaining, --r, r + 1) != r)
            {
                r = remaining;
            }
            if(r == 0)
            {
                int oldPhase;
                int oldRemaining;
                do
                {
                    r = remaining;
                    oldRemaining = Interlocked.CompareExchange(ref remaining, partners, r);
                    phase = currentPhase;
                    oldPhase = Interlocked.CompareExchange(ref currentPhase, phase - 1, phase);
                } while (oldRemaining != r  && oldPhase != phase);
            }
            else
            {
                while (phase == currentPhase) Thread.Yield();
            }
        }
    }
}
