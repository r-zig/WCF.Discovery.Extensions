using System;

namespace Roniz.WCF.Discovery.Extenstions.DiscoveryProxy
{
    public partial class DiscoveryProxyService
    {
        sealed class OnFindAsyncResult : AsyncResult
        {
            public OnFindAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<OnFindAsyncResult>(result);
            }
        }
    }
}
