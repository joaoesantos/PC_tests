using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _15_16i_1
{
    public class Users
    {
        public interface Service
        {
            IAsyncResult BeginFindId(String name, String bdate, AsyncCallback cb, Object stt);
            int EndFindId(IAsyncResult asyncRes);
            Task<int> FindIdAsync(String name, String birthdate);
            IAsyncResult BeginObtainAvatarUri(int userId, AsyncCallback cb, Object stt);
            Uri EndObtainAvatarUri(IAsyncResult asyncRes);
            Task<Uri> ObtainAvatarUriAsync(int userId);
        }
        public static Task<Uri> GetUserAvatarAsync(Service svc, String name, String bdate)
        {
            Task<int> findID = svc.FindIdAsync(name, bdate);
            return findID.ContinueWith((t) => svc.ObtainAvatarUriAsync(t.Result)).Unwrap();

        }
    }
}
