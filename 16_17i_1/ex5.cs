using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _16_17i_1
{
    public class ex5
    {
        public static List<O> MapSelectedItems<I, O>(IEnumerable<I> items, Predicate<I> selector,
 Func<I, O> mapper, CancellationToken ctoken)
        {
            object monitor = new object();
            var result = new List<O>();

            Parallel.ForEach(
                items,
                new ParallelOptions { CancellationToken = ctoken },
                () => new List<O>(),
                (item, loopState, partial) =>
                {
                    if (ctoken.IsCancellationRequested)
                    {
                        return partial;
                    }
                    else
                    {
                        if (selector(item))
                        {
                            partial.Add(mapper(item));
                        }
                            return partial;
                    }
                },
                (partial)=> {
                    lock (monitor)
                    {
                         result.AddRange(partial);
                    }
                });
            return result;
        }
    }
    
}
