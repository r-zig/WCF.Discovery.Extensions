using System;
using System.ServiceModel.Discovery;

namespace Roniz.WCF.Discovery.Extenstions.DiscoveryProxy
{
    public partial class DiscoveryProxyService
    {
        sealed class OnResolveAsyncResult : AsyncResult
        {
            readonly EndpointDiscoveryMetadata matchingEndpoint;

            public OnResolveAsyncResult(EndpointDiscoveryMetadata matchingEndpoint, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.matchingEndpoint = matchingEndpoint;
                this.Complete(true);
            }

            public static EndpointDiscoveryMetadata End(IAsyncResult result)
            {
                OnResolveAsyncResult thisPtr = AsyncResult.End<OnResolveAsyncResult>(result);
                return thisPtr.matchingEndpoint;
            }
        }
    }
}
