using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roniz.WCF.Discovery.Extenstions.DiscoveryProxy
{
    public partial class DiscoveryProxyService
    {
        sealed class OnOfflineAnnouncementAsyncResult : AsyncResult
        {
            public OnOfflineAnnouncementAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<OnOfflineAnnouncementAsyncResult>(result);
            }
        }
    }
}
