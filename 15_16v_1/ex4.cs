using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _15_16v_1
{
    public class TapExec
    {
        public interface TapServices<S, R>
        {
            Task<Uri> PingServerAsync(Uri server);
            Task<R> ExecServiceAsync(Uri server, S service);
        }
        public async Task<R> ExecOnNearServer<S, R>(TapServices<S, R> svc, Uri[] servers, S service)
        {
            Task<Uri>[] tasks = new Task<Uri>[servers.Length];
            R res;
            for(int i = 0; i < servers.Length; i++){
                int t = i;
                tasks[i] = svc.PingServerAsync(servers[t]);
            }

            return await svc.ExecServiceAsync(await Task.WhenAny(tasks).Unwrap(), service);
        }
    }
}
