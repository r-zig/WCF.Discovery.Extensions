using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;

namespace Roniz.WCF.Discovery.Extenstions.Endpoints
{
    /// <summary>
    /// A custom endpoint that is used by services to send announcement messages over p2p channel
    /// </summary>
    public class P2PAnnouncementEndpoint : AnnouncementEndpoint
    {
        #region members
        
        #endregion

        #region properties
        #endregion

        #region Constructores
        /// <summary>
        /// Creates a new instance of the P2PAnnouncementEndpoint class.
        /// </summary>
        public P2PAnnouncementEndpoint()
            : this(P2PDiscoveryDefaults.DefaultDiscoveryVersion, P2PDiscoveryDefaults.DefaultAnnouncementUri)
        {
            
        }

        /// <summary>
        /// Creates a new instance of the P2PAnnouncementEndpoint class that is configured to use the specified DiscoveryVersion.  
        /// </summary>
        /// <param name="discoveryVersion">The DiscoveryVersion to use.</param>
        public P2PAnnouncementEndpoint(DiscoveryVersion discoveryVersion)
            : this(discoveryVersion, P2PDiscoveryDefaults.DefaultAnnouncementUri)
        {
            
        }

        /// <summary>
        /// Creates a new instance of the P2PAnnouncementEndpoint class with the specified multicast address.
        /// </summary>
        /// <param name="multicastAddress">The multicast address for the P2P discovery endpoint.</param>
        public P2PAnnouncementEndpoint(String multicastAddress)
            : this(P2PDiscoveryDefaults.DefaultDiscoveryVersion, new Uri(multicastAddress))
        {
            
        }

        /// <summary>
        /// Creates a new instance of the P2PAnnouncementEndpoint class with the specified multicast address.
        /// </summary>
        /// <param name="multicastAddress">The multicast address for the P2P discovery endpoint.</param>
        public P2PAnnouncementEndpoint(Uri multicastAddress)
            : this(P2PDiscoveryDefaults.DefaultDiscoveryVersion, multicastAddress)
        {
            
        }

        /// <summary>
        /// Creates a new instance of the P2PAnnouncementEndpoint that is configured to use the specified DiscoveryVersion and multicast address.
        /// </summary>
        /// <param name="discoveryVersion">The DiscoveryVersion to use.</param>
        /// <param name="multicastAddress">The multicast address for the P2P discovery endpoint.</param>
        public P2PAnnouncementEndpoint(DiscoveryVersion discoveryVersion, String multicastAddress):this(discoveryVersion,new Uri(multicastAddress))
        {
            
        }

        /// <summary>
        /// Creates a new instance of the P2PAnnouncementEndpoint that is configured to use the specified DiscoveryVersion and multicast address.
        /// </summary>
        /// <param name="discoveryVersion">The DiscoveryVersion to use.</param>
        /// <param name="multicastAddress">The multicast address for the P2P discovery endpoint.</param>
        public P2PAnnouncementEndpoint(DiscoveryVersion discoveryVersion, Uri multicastAddress)
            : base(discoveryVersion, P2PDiscoveryDefaults.GetDefaultBinding(), new EndpointAddress(multicastAddress))
        {
        }

        #endregion

        #region methods
        
        #endregion
    }
}
