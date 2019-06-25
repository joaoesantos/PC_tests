using System;
using System.Threading;

namespace _15_16i_1
{
    public class SafeSpinLazy<T> where T : class
    {
        private const int UNCREATED = 0, BEING_CREATED = 1, CREATED = 2;
        private volatile int state = UNCREATED;
        private volatile Func<T> factory;
        private volatile T value;
        public SafeSpinLazy(Func<T> factory) { this.factory = factory; }
        public bool IsValueCreated { get { return state == CREATED; } }
        public T Value {
            get {
                SpinWait sw = new SpinWait();
                do
                {
                    if (state == CREATED)
                    {
                        break;
                    }
                    else if (state == UNCREATED)
                    { int s;
                        do {
                            do
                            {
                                s = state;
                            } while (Interlocked.CompareExchange(ref state, BEING_CREATED, s) != s);

                            T t;
                            do
                            {
                                t = value;
                            } while (Interlocked.CompareExchange(ref value, factory(), t) != t);
                            s = state;
                        } while (Interlocked.CompareExchange(ref state, CREATED, s) != s);

                        break;
                    }
                    sw.SpinOnce();
                } while (true);
                return value;
            }
        }
    }
}
