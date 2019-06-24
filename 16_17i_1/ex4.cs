using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _16_17i_1
{
    public class TapExecute<A,B,C,D>
    {
        public interface TapServices
        {
            Task<A> Oper1Sync();
            Task<B> Oper2Sync(A a);
            Task<C> Oper3Sync(A a);
            Task<D> Oper4Sync(B b, C c);
        }
        public static async Task<D> Run(TapServices svc)
        {
            Task<A> a =  svc.Oper1Sync();
            Task<B> b = a.ContinueWith((t) => svc.Oper2Sync(a.Result)).Unwrap();
            Task<C> c = a.ContinueWith((t) => svc.Oper3Sync(a.Result)).Unwrap();
            return await svc.Oper4Sync(await b, await c);
        }
    }
}
