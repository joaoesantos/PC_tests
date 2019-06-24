using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _15_16v_1
{
    class TransientSignal
    {
        private LinkedList<Request> waiters;
        private readonly object monitor;

        class Request{
            internal bool done;
        }
        public TransientSignal()
        {
            monitor = new object();
        }
        public bool await(int timeout)
        {
            LinkedListNode<Request> newNode = new LinkedListNode<Request>(new Request());
            newNode.Value.done = false;
            TimeoutHolder th = new TimeoutHolder(timeout);
            lock (monitor)
            {
                waiters.AddLast(newNode);
              
                do
                {
                    try
                    {
                        if ((timeout = th.Value) == 0)
                            return false;
                        MonitorEx.Wait(monitor, newNode);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        if (newNode.Value.done)
                        {
                            waiters.Remove(newNode);
                            Thread.CurrentThread.Interrupt();
                            return newNode.Value.done;
                        }
                        throw e;
                    }
                } while (!newNode.Value.done);
                waiters.Remove(newNode);
                return newNode.Value.done;
            }
            
        }
        public void signal()
        {
            lock (monitor)
            {
                if (waiters.Count > 0)
                {
                    LinkedListNode<Request> node = waiters.First;
                    waiters.RemoveFirst();
                    node.Value.done = true;

                    MonitorEx.Pulse(monitor, node);

                }
            }

        }
        public void signalAll()
        {
            if (waiters.Count > 0)
            {
                foreach(Request req in waiters)
                {
                    req.done = true;
                }

                Monitor.PulseAll(monitor);
            }
        }
    }
}
