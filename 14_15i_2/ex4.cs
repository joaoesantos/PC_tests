using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _14_15i_2
{
    public class SyncOps
    {
        public interface Services
        {
            String GetDeviceAddress(int devId);
            int GetVersionFromDevice(String addr);
            int GetStoredVersion(int devId);
        }
        public bool CheckDeviceVersion(Services svc, int devId)
        {
            String addr = svc.GetDeviceAddress(devId);
            int devVer = svc.GetVersionFromDevice(addr);
            int stoVer = svc.GetStoredVersion(devId);
            return devVer == stoVer;
        }

        public interface TapServices : Services
        {
            Task<String> GetDeviceAddressAsync(int devId);
            Task<int> GetVersionFromDeviceAsync(String addr);
            Task<int> GetStoredVersionAsync(int devId);
        }
    }

    public class ApmOps
    {
        SyncOps.TapServices tapServices;
        public async Task<bool> CheckDeviceVersionAsync(int devID)
        {
            Task<int>[] tasks = new Task<int>[2];
            tasks[0] = tapServices.GetDeviceAddressAsync(devID)
                .ContinueWith((ts) => tapServices.GetVersionFromDeviceAsync(ts.Result)).Unwrap();

            tasks[1] = tapServices.GetStoredVersionAsync(devID);

            await Task.WhenAll(tasks);
            return tasks[0].Result == tasks[1].Result;
        }
    }

    internal class GenericAsyncResult<T> : IAsyncResult
    {
        public bool IsCompleted => throw new NotImplementedException();

        public WaitHandle AsyncWaitHandle => throw new NotImplementedException();

        public object AsyncState => throw new NotImplementedException();

        public bool CompletedSynchronously => throw new NotImplementedException();
    }
}
