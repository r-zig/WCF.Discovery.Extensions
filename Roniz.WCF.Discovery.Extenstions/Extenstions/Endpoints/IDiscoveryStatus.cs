using System;

namespace Roniz.WCF.Discovery.Extenstions.Endpoints
{
    /// <summary>
    /// provide information about the state of the discovery
    /// </summary>
    /// <remarks>
    /// For example in P2P discovery , the P2P should initiate before any discovery action executed
    /// The classes that expose this behavior should expose also the discovery state
    /// </remarks>
    public interface IDiscoveryState
    {
        bool IsReady { get;}
        event EventHandler<DiscoveryStateEventArgs> DiscoveryStateChanged;
    }
}
