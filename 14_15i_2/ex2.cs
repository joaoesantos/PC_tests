using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _14_15i_2
{
    class ex2
    {
        public class DataDistributor<D>
        {
            private int max;
            private readonly object monitor;
            private LinkedList<Request> waiters;
            private List<D> msgQueue;

            class Request
            {
                internal bool done;
                internal List<D> data;
            }

            public DataDistributor(int n)
            {
                max = n;
                monitor = new object();
            }
            public void Put(List<D> data)
            {
                lock (monitor) {
                    if (waiters.Count > 0)
                    {
                        Monitor.PulseAll(monitor);
                    }
                    else
                    {
                        msgQueue.AddRange(data);
                    }
                }
            }

            // obtém entre 1 e n elementos
            public List<D> Take()
            {

                LinkedListNode<Request> newNode = new LinkedListNode<Request>(new Request());

                do
                {
                    if (msgQueue.Count > 0)
                    {
                        newNode.Value.done = true;
                        newNode.Value.data = msgQueue.Take(max).ToList();
                        msgQueue.RemoveRange(0, max);
                    }
                    else
                    {
                        try
                        {
                            waiters.AddFirst(newNode);
                            MonitorEx.Wait(monitor, newNode);
                        }
                        catch(ThreadInterruptedException e)
                        {
                            if (newNode.Value.done)
                            {
                                waiters.Remove(newNode);
                                Thread.CurrentThread.Interrupt();
                                return newNode.Value.data;
                            }
                            throw e;
                        }
                    }
                } while (!newNode.Value.done);
                waiters.Remove(newNode);
                return newNode.Value.data;

            }
        }
    }
}
