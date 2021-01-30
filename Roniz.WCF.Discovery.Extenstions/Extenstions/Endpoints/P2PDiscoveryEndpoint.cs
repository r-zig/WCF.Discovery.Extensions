using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using Roniz.Diagnostics.Logging;
using Roniz.WCF.Discovery.Extenstions.DiscoveryProxy;
using Roniz.WCF.P2P.Sync;
using Roniz.WCF.Discovery.Extenstions.ServicesCache;

namespace Roniz.WCF.Discovery.Extenstions.Endpoints
{
    /// <summary>
    /// A custom endpoint that is pre-configured for discovery operations over a NetPeerTcpBinding (PeerChannel).
    /// This endpoint inherits from DiscoveryEndpoint and similarly has a fixed contract and supports two WS-Discovery protocol versions.
    /// In addition, it has a fixed NetPeerTcpBinding and a default address.
    /// </summary>
    public class P2PDiscoveryEndpoint : DiscoveryEndpoint , IDiscoveryState
    {
        #region members
        private bool isReady;
        private DiscoveryProxyService discoveryProxyService;
        private ServiceHost discoveryProxyServiceHost;
        private SynchronizationStateManager syncManager;
        private SynchronizaedServiceCache serviceCache;
        #endregion

        #region properties
        ///// <summary>
        ///// A Uri instance that contains the multicast address.
        ///// </summary>
        ///// <remarks>The default value is </remarks>
        //public Uri MulticastAddress { get; set; }

        #endregion

        #region Constructores

        /// <summary>
        /// Creates a new instance of the P2PDiscoveryEndpoint class.
        /// </summary>
        public P2PDiscoveryEndpoint():this(P2PDiscoveryDefaults.DefaultDiscoveryVersion,P2PDiscoveryDefaults.DefaultAdHoc)
        {
            
        }

        /// <summary>
        /// Creates a new instance of the P2PDiscoveryEndpoint class.
        /// </summary>
        /// <param name="innerDiscoveryEndpoint">Whether this endpoint act as Inner endpoint to the real one</param>
        public P2PDiscoveryEndpoint(bool innerDiscoveryEndpoint)
            : this(innerDiscoveryEndpoint, P2PDiscoveryDefaults.DefaultDiscoveryVersion, P2PDiscoveryDefaults.DefaultAdHoc)
        {
        }

        /// <summary>
        /// Creates a new instance of the P2PDiscoveryEndpoint class with the specified DiscoveryVersion.  
        /// </summary>
        /// <param name="discoveryVersion">The DiscoveryVersion to use.</param>
        /// <remarks>Two versions of the WS-Discovery protocol are currently supported: WS-Discovery April 2005 and WS-Discovery V1.1, which correspond to the WSDiscoveryApril2005() and WSDiscoveryV11() enumeration values.</remarks>
        public P2PDiscoveryEndpoint(DiscoveryVersion discoveryVersion):this(discoveryVersion,P2PDiscoveryDefaults.DefaultAdHoc)
        {
            
        }

        /// <summary>
        ///   Creates a new instance of the P2PDiscoveryEndpoint class with the specified multicast address.  
        /// </summary>
        /// <param name="multicastAddress">The multicast address.</param>
        public P2PDiscoveryEndpoint(string multicastAddress):this(P2PDiscoveryDefaults.DefaultDiscoveryVersion,new Uri(multicastAddress))
        {
            
        }

        /// <summary>
        /// Creates a new instance of the P2PDiscoveryEndpoint class with the specified multicast address.
        /// </summary>
        /// <param name="multicastAddress">The multicast address.</param>
        public P2PDiscoveryEndpoint(Uri multicastAddress):this(P2PDiscoveryDefaults.DefaultDiscoveryVersion,multicastAddress)
        {
            
        }

        /// <summary>
        /// Creates a new instance of the P2PDiscoveryEndpoint class with the specified DiscoveryVersion and multicast address.
        /// </summary>
        /// <param name="discoveryVersion">The DiscoveryVersion to use.</param>
        /// <param name="multicastAddress">The multicast address for the P2P discovery endpoint.</param>
        public P2PDiscoveryEndpoint(DiscoveryVersion discoveryVersion, String multicastAddress):this(discoveryVersion,new Uri(multicastAddress))
        {
            
        }

        /// <summary>
        /// Creates a new instance of the P2PDiscoveryEndpoint class with the specified DiscoveryVersion and multicast address.  
        /// </summary>
        /// <param name="discoveryVersion">The DiscoveryVersion to use.</param>
        /// <param name="multicastAddress">The multicast address for the P2P discovery endpoint.</param>
        public P2PDiscoveryEndpoint(DiscoveryVersion discoveryVersion, Uri multicastAddress)
            :this(false,discoveryVersion,multicastAddress)
        {
        }

        /// <summary>
        /// Creates a new instance of the P2PDiscoveryEndpoint class with the specified DiscoveryVersion and multicast address.  
        /// </summary>
        /// <param name="innerDiscoveryEndpoint">Whether this endpoint act as Inner endpoint to the real one</param>
        /// <param name="discoveryVersion">The DiscoveryVersion to use.</param>
        /// <param name="multicastAddress">The multicast address for the P2P discovery endpoint.</param>
        public P2PDiscoveryEndpoint(bool innerDiscoveryEndpoint,DiscoveryVersion discoveryVersion, Uri multicastAddress)
            : base(discoveryVersion, P2PDiscoveryDefaults.DefaultDiscoveryMode, P2PDiscoveryDefaults.GetDefaultBinding(), new EndpointAddress(multicastAddress))
        {
            if (!innerDiscoveryEndpoint)
            {
                OpenDiscoveryProxy();
                OpenSyncManager();
            }
        }
        #endregion

        #region methods

        #region discovery proxy
        /// <summary>
        /// Open the discovery proxy host
        /// </summary>
        private void OpenDiscoveryProxy()
        {
            // Host the DiscoveryProxy service
            discoveryProxyService = new DiscoveryProxyService();
            discoveryProxyServiceHost = new ServiceHost(discoveryProxyService);

            try
            {
                var discoveryEndpoint = new P2PDiscoveryEndpoint(true) {IsSystemEndpoint = false};
                // Add DiscoveryEndpoint to receive Probe and Resolve messages
                discoveryProxyServiceHost.AddServiceEndpoint(discoveryEndpoint);

                // Add AnnouncementEndpoint to receive Hello and Bye announcement messages
                AnnouncementEndpoint announcementEndpoint = new P2PAnnouncementEndpoint();
                discoveryProxyServiceHost.AddServiceEndpoint(announcementEndpoint);
                
                discoveryProxyServiceHost.Open();

                LogManager.GetCurrentClassLogger().Debug("Discovery proxy service opened.");
            }
            catch (CommunicationException e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
            catch (TimeoutException e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
        }

        /// <summary>
        /// Close the discovery proxy host
        /// </summary>
        private void CloseDiscoveryProxy()
        {
            try
            {
                discoveryProxyServiceHost.Close();
                LogManager.GetCurrentClassLogger().Debug("Discovery proxy service closed.");
            }
            catch (CommunicationException e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
            catch (TimeoutException e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
        }

        private void OpenSyncManager()
        {
            serviceCache = new SynchronizaedServiceCache();
            syncManager = new SynchronizationStateManager(serviceCache);
            syncManager.Open();
        }

        private void CloseSyncManager()
        {
            syncManager.Close();
        }
        #endregion

        #endregion

        #region Implementation of IDiscoveryState

        public bool IsReady
        {
            get { return isReady; }
        }

        public event EventHandler<DiscoveryStateEventArgs> DiscoveryStateChanged;

        public void InvokeDiscoveryStateChanged(DiscoveryStateEventArgs e)
        {
            EventHandler<DiscoveryStateEventArgs> handler = DiscoveryStateChanged;
            if (handler != null) 
                handler(this, e);
        }

        #endregion
    }
}
