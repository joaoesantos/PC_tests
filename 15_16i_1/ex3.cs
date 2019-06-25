using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _15_16i_1
{
    public class Rendezvous<T>
    {
        private object monitor;
        List<Request> waiters;
        public Rendezvous(){
            monitor = new object();
        }
        class Request
        {
            internal bool done;
            internal T mydata;
            internal T yourData;
            internal int rvKey;

            internal Request(T _mData, int _rvkey)
            {
                mydata = _mData;
                rvKey = _rvkey;
                done = false;
            }
        }
        public bool DoIt(int rvkey, T mydata, int timeout, out T yourData)
        {
            Request request = new Request(mydata, rvkey);
            TimeoutHolder th = new TimeoutHolder(timeout);

            lock (monitor)
            {
                Request temp = waiters.Where((r) => r.rvKey == rvkey).FirstOrDefault();
                if (temp != null)
                {
                    waiters.Remove(temp);
                    temp.yourData = mydata;
                    yourData = temp.mydata;
                    temp.done = request.done = true;

                    MonitorEx.Pulse(monitor, temp);
                    return true;
                }
                else
                {
                    waiters.Add(request);
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
                                yourData = request.yourData;
                                Thread.CurrentThread.Interrupt();
                                return true;
                            }
                            throw e;
                        }
                    } while (!request.done);
                    waiters.Remove(request);
                    yourData = request.yourData;
                    return true;

                }
            }
        }
    }
}
