using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _15_16v_1
{
    public class ParallelAggregate
    {
        public class Result
        {
            internal int value;
            internal static Result OVERFLOW = new Result(-1);
            internal Result(int v)
            {
                value = v;
            }
            internal Result() : this(0) { }
        }

        public class Data
        {
            internal int value;

            internal Data(int v) { value = v; }
        }

        internal static Result Map(Data datum)
        {
            Thread.SpinWait(100000);
            return new Result(datum.value);
        }

        internal static Result Aggregate(Result r, Result r2)
        {
            if (r.value > 50000)
                return Result.OVERFLOW;
            return new Result(r.value + r2.value);
        }

        public static Result ParallelMapAggregate(IEnumerable<Data> data)
        {
            object _lock = new object();
            Result result = new Result();
            Parallel.ForEach(
                data,
                () => new Result(),
                (item, loopState, partial) =>
                {
                  return  partial = Aggregate(partial, Map(item));
                },
                (partial) =>
                {
                    lock (_lock)
                    {
                        result = Aggregate(result, partial);
                    }
                }
                );

            return result;
        }
    }
   
}
