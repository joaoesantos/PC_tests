using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _15_16i_1
{
    class ex5
    {
        static Info ProcessItems(Data[] items, Session session)
        {
            Info info = new Info();
            object _lock = new object();
            Parallel.ForEach(
                items,
                () => new Info(),
                (item,loopstate, partial) =>
                {
                    partial = MergeInfo(partial, ExtractInfo(item, session));
                    return partial;
                },
                (partial) =>
                {
                    lock (_lock)
                    {
                        info = MergeInfo(info, partial);
                    }
                });

            for (int i = 0; i < items.Length; ++i)
                info = MergeInfo(info, ExtractInfo(items[i], session));
            return info;
        }

        static Info MergeInfo(Info info, object p)
        {
            throw new NotImplementedException();
        }

        static object ExtractInfo(Data data, Session session)
        {
            throw new NotImplementedException();
        }
    }
    
}
