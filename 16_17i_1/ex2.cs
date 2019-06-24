using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _16_17i_1
{
    public class AdvertisingPanel<M> where M : class
    {
        private M message;
        private TimeoutHolder th;
        private readonly object monitor;
        private LinkedList<Request> waiters;
        public AdvertisingPanel()
        {
            monitor = new object();
            th = new TimeoutHolder(0);
        }

        class Request
        {
            internal M data;
            internal bool done;
        }
        public void Publish(M message, int exposureTime)
        {
            lock (monitor)
            {
                this.message = message;
                th = new TimeoutHolder(exposureTime);

                if (waiters.Count > 0)
                {
                    foreach (Request r in waiters)
                    {
                        r.done = true;
                        r.data = message;
                    }
                    waiters = new LinkedList<Request>();
                    Monitor.PulseAll(monitor);
                }
            }
        }
        public M Consume(int timeout)
        {
            lock (monitor)
            {
                if (th.Value > 0)
                {
                    return message;
                }
                else
                {
                    Request request = new Request();
                    request.done = false;

                    waiters.AddLast(request);
                    do
                    {
                        try
                        {
                            MonitorEx.Wait(monitor, request);
                        }
                        catch (ThreadInterruptedException e)
                        {
                            if (request.done)
                            {
                                waiters.Remove(request);
                                Thread.CurrentThread.Interrupt();
                                return request.data;
                            }
                            throw e;
                        }
                    } while (!request.done);
                    return request.data;

                }
            }
        }
    }
}
