using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _16_17i_1
{
    public class CrossExchanger<T1, T2> where T1 : class where T2 : class
    {
        private readonly object monitor;
        private LinkedList<Request> t1Data;
        private LinkedList<Request> t2Data;

        class Request
        {
            internal bool done;
            internal T1 t1;
            internal T2 t2;
        }

        public T2 Exchange1(T1 mine, int timeout)
        {
            lock (monitor)
            {
                if (t2Data.Count > 0)
                {
                    LinkedListNode<Request> t2Node = t2Data.First;
                    t2Data.RemoveFirst();

                    t2Node.Value.done = true;
                    t2Node.Value.t1 = mine;
                    MonitorEx.Pulse(monitor, t2Node.Value);
                    return t2Node.Value.t2;
                }
                else
                {
                    Request request = new Request();
                    request.t1 = mine;
                    request.done = false;
                    t1Data.AddLast(request);
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
                                t1Data.Remove(request);
                                Thread.CurrentThread.Interrupt();
                                return request.t2;
                            }
                            throw e;
                        }
                    } while (!request.done);

                    t1Data.Remove(request);
                    return request.t2;
                }
            }
           
        }
        public T1 Exchange2(T2 mine, int timeout)
        {
            lock (monitor)
            {
                if (t1Data.Count > 0)
                {
                    LinkedListNode<Request> t1Node = t1Data.First;
                    t1Data.RemoveFirst();

                    t1Node.Value.done = true;
                    t1Node.Value.t2 = mine;
                    MonitorEx.Pulse(monitor,t1Node.Value);
                    return t1Node.Value.t1;
                }
                else
                {
                    Request request = new Request();
                    request.t2 = mine;
                    request.done = false;
                    t2Data.AddLast(request);
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
                                t2Data.Remove(request);
                                Thread.CurrentThread.Interrupt();
                                return request.t1;
                            }
                            throw e;
                        }
                    } while (!request.done);

                    t2Data.Remove(request);
                    return request.t1;
                }
            }
        }
    }
}
