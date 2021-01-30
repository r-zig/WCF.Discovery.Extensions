using System;
using System.ServiceModel.Discovery;

namespace Roniz.WCF.Discovery.Extenstions.DiscoveryProxy
{
    /// <summary>
    /// Used by the DiscoveryProxyService Adding and Removed events to expose the services metadata
    /// </summary>
    public sealed class EndpointDiscoveryMetadataEventArgs : EventArgs
    {
        #region members
        private readonly EndpointDiscoveryMetadata endpointDiscoveryMetadata;
        #endregion

        #region constructor
        public EndpointDiscoveryMetadataEventArgs(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            this.endpointDiscoveryMetadata = endpointDiscoveryMetadata;
        }
        #endregion

        #region properties
        public EndpointDiscoveryMetadata EndpointDiscoveryMetadata
        {
            get { return endpointDiscoveryMetadata; }
        }
        #endregion
    }
}