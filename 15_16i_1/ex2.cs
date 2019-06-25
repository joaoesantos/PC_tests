using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _15_16i_1
{
    public class ChannelWithPriority<T>
    {
        private readonly object monitor;
        private LinkedList<T> highPriorityMsg;
        private LinkedList<T> lowPriorityMsg;
        private LinkedList<Request> requests;

        class Request
        {
            internal bool done;
            internal T data;
        }

        public void Put(T msg, bool urgent)
        {
            lock (monitor)
            {
                if (requests.Count > 0)
                {
                    LinkedListNode<Request> node = requests.First;
                    requests.RemoveFirst();

                    node.Value.done = true;
                    node.Value.data = msg;

                    MonitorEx.Pulse(monitor, node);                    
                }
                else
                {
                    if (urgent)
                    {
                        highPriorityMsg.AddLast(msg);
                    }
                    else
                    {
                        lowPriorityMsg.AddLast(msg);
                    }
                }
            }
        }
        public bool Take(int timeout, out T rcvdMsg)
        {
            LinkedListNode<Request> newNode = new LinkedListNode<Request>(new Request());
            newNode.Value.done = false;
            lock (monitor)
            {
                do
                {
                    if (highPriorityMsg.Count > 0)
                    {
                        LinkedListNode<T> node = highPriorityMsg.First;
                        highPriorityMsg.RemoveFirst();
                        newNode.Value.data = node.Value;
                    }
                    else if (lowPriorityMsg.Count > 0)
                    {
                        LinkedListNode<T> node = lowPriorityMsg.First;
                        lowPriorityMsg.RemoveFirst();
                        newNode.Value.data = node.Value;
                    }
                    else
                    {
                        try
                        {
                            MonitorEx.Wait(monitor, newNode);
                        }catch(ThreadInterruptedException e)
                        {
                            if (newNode.Value.done)
                            {
                                rcvdMsg = newNode.Value.data;
                                return true;
                            }
                            rcvdMsg = default;
                            return false;
                        }
                    }
                } while (!newNode.Value.done);
                rcvdMsg = newNode.Value.data;
                return true;
            }
        }
    }
}
