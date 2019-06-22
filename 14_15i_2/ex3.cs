using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _14_15i_2
{
    public class BulletinBoard<W> where W : class
    {
        private W warning;
        private TimeoutHolder th;
        private List<Request> waiters;
        private object monitor;

        BulletinBoard()
        {
            waiters = new List<Request>();
            monitor = new object();
        }

        class Request
        {
            internal W data;
            internal bool done;
        }
        public void Post(W warning, int validity)
        {
            lock (monitor)
            {
                if (validity == 0 && waiters.Count > 0)
                {
                    foreach (Request req in waiters)
                    {
                        req.done = true;
                        req.data = warning;
                    }
                    Monitor.PulseAll(monitor);
                }
                else
                {
                    if(waiters.Count > 0)
                    {
                        foreach (Request req in waiters)
                        {
                            req.done = true;
                            req.data = warning;
                        }
                        Monitor.PulseAll(monitor);
                    }
                    else
                    {
                        if (validity > 0)
                        {
                            this.warning = warning;
                            th = new TimeoutHolder(validity);
                        }
                    }
                }
            }
            

        }
        public W Receive()
        {
            lock (monitor)
            {
                if (th.Value > 0)
                    return warning;

                Request request = new Request();
                request.done = false;
                waiters.Add(request);
                do
                {
                    try
                    {

                    }catch(ThreadInterruptedException e)
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

                waiters.Remove(request);
                return request.data;
            }
        }
        public void Clear()
        {
            lock (monitor)
            {
                warning = null;
                th = new TimeoutHolder(0);
            }
        }
    }
}
