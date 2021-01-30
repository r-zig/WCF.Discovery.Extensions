using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using Roniz.WCF.Discovery.Extenstions.ServicesCache;
using Roniz.WCF.P2P.Sync;

namespace Roniz.WCF.Discovery.Extenstions.Client
{
    /// <summary>
    /// Wrapper around the DiscoveryClient that provide more functionality
    /// and enable discover also with OneWay MEP instead of only request-replay MEP
    /// </summary>
    public class DiscoveryClientEx //: ICommunicationObject, IDisposable
    {
        #region members
        /// <summary>
        /// inner discovery client
        /// </summary>
        private DiscoveryClient innerDiscoveryClient;

        /// <summary>
        /// if this instance support request-replay WCF pattern or not
        /// </summary>
        /// <remarks>When use OneWay binding (from the discovery endpoint) 
        /// some methods cannot used as regular such as Find(Async) because there is no ReplyContext
        /// when it's happen there is some workaround in order to let the method working anyway
        /// see also <see>http://msdn.microsoft.com/en-us/library/aa751839.aspx</see>
        /// </remarks>
        private readonly bool isSupportRequestReplay;

        private SynchronizaedServiceCache serviceCache;
        private SynchronizationStateManager syncManager;
        #endregion

        #region constructores

        public DiscoveryClientEx()
        {
            innerDiscoveryClient = new DiscoveryClient();
#warning should set here the isSupportRequestReplay
            Initialize();
        }

        /// <summary>
        /// Creates a new instance of the Roniz.WCF.Discovery.Extenstions.DiscoveryClientEx class.
        /// </summary>
        /// <param name="discoveryEndpoint">The discovery endpoint to use when sending discovery messages.</param>
        public DiscoveryClientEx(DiscoveryEndpoint discoveryEndpoint)
        {
            isSupportRequestReplay = IsBindingSupportRequestReplay(((System.ServiceModel.Description.ServiceEndpoint)(discoveryEndpoint)).Binding);
            innerDiscoveryClient = new DiscoveryClient(discoveryEndpoint);
            Initialize();
        }

        /// <summary>
        /// Creates a new instance of the Roniz.WCF.Discovery.Extenstions.DiscoveryClientEx
        /// class with the specified endpoint configuration.
        /// </summary>
        /// <param name="endpointConfigurationName">The endpoint configuration name to use.</param>
        public DiscoveryClientEx(string endpointConfigurationName)
        {
#warning should set here the isSupportRequestReplay
            innerDiscoveryClient = new DiscoveryClient(endpointConfigurationName);
            Initialize();
        }

        #endregion

        #region properties

        // Summary:
        //     Gets the channel factory for the Roniz.WCF.Discovery.Extenstions.DiscoveryClientEx.
        //
        // Returns:
        //     A channel factory.
        public ChannelFactory ChannelFactory
        {
            get
            {
                return innerDiscoveryClient.ChannelFactory;
            }
        }
        //
        // Summary:
        //     Gets the client credentials for the Roniz.WCF.Discovery.Extenstions.DiscoveryClientEx.
        //
        // Returns:
        //     The client credentials for the Roniz.WCF.Discovery.Extenstions.DiscoveryClientEx.
        public ClientCredentials ClientCredentials
        {
            get
            {
                return innerDiscoveryClient.ClientCredentials;
            }
        }
        //
        // Summary:
        //     Gets the endpoint used to send discovery messages.
        //
        // Returns:
        //     The endpoint used to send discovery messages.
        public ServiceEndpoint Endpoint
        {
            get
            {
                return innerDiscoveryClient.Endpoint;
            }
        }
        //
        // Summary:
        //     The channel used to send discovery messages.
        //
        // Returns:
        //     The channel used to send discovery messages.
        public IClientChannel InnerChannel
        {
            get
            {
                return innerDiscoveryClient.InnerChannel;
            }
        }

        #endregion

        #region events

        // Summary:
        //     Occurs when the entire find operation completes.
        public event EventHandler<FindCompletedEventArgs> FindCompleted
        {
            add
            {
                lock (serviceCache)
                {
                    serviceCache.FindCompleted += value;
                }
            }
            remove
            {
                lock (serviceCache)
                {
                    serviceCache.FindCompleted -= value;
                }
            }
        }
        //
        // Summary:
        //     Occurs every time the client receives a response from a particular service.
        public event EventHandler<FindProgressChangedEventArgs> FindProgressChanged
        {
            add
            {
                lock (serviceCache)
                {
                    serviceCache.FindProgressChanged += value;
                }
            }
            remove
            {
                lock (serviceCache)
                {
                    serviceCache.FindProgressChanged -= value;
                }
            }
        }
        //
        // Summary:
        //     Occurs when a multicast suppression message is received from a discovery
        //     proxy in response to the find or resolve operation.
        public event EventHandler<AnnouncementEventArgs> ProxyAvailable;
        //
        // Summary:
        //     Occurs when an asynchronous resolve operation is completed.
        public event EventHandler<ResolveCompletedEventArgs> ResolveCompleted;

        #endregion

        #region methods

        // Summary:
        //     Cancels a pending asynchronous operation.
        //
        // Parameters:
        //   userState:
        //     A user specified state object that is passed to the Roniz.WCF.Discovery.Extenstions.DiscoveryClientEx.FindAsync()
        //     method or one of the Roniz.WCF.Discovery.Extenstions.DiscoveryClientEx.ResolveAsync()
        //     methods. It identifies the pending asynchronous operation to cancel.
        public void CancelAsync(object userState)
        {
            innerDiscoveryClient.CancelAsync(userState);
        }
        //
        // Summary:
        //     Closes the discovery client.
        public void Close()
        {
            innerDiscoveryClient.Close();
            syncManager.Close();
        }
        //
        // Summary:
        //     Sends a request to find services that match the specified criteria.
        //
        // Parameters:
        //   criteria:
        //     The criteria for finding services.
        //
        // Returns:
        //     The discoverable metadata of the matching service endpoints.
        public FindResponse Find(FindCriteria criteria)
        {
            if (isSupportRequestReplay)
                return innerDiscoveryClient.Find(criteria);
            else
                return serviceCache.Find(criteria).FirstOrDefault();
        }
        //
        // Summary:
        //     Begins an asynchronous find operation with the specified criteria.
        //
        // Parameters:
        //   criteria:
        //     The criteria for finding services.

        public void FindAsync(FindCriteria criteria)
        {
            FindAsync(criteria, null);
        }
        //
        // Summary:
        //     Begins an asynchronous find operation with the specified criteria and user
        //     defined state object.
        //
        // Parameters:
        //   criteria:
        //     The criteria for finding services.
        //
        //   userState:
        //     A user specified object to identify the asynchronous find operation.
        public void FindAsync(FindCriteria criteria, object userState)
        {
            if (isSupportRequestReplay)
                innerDiscoveryClient.FindAsync(criteria, userState);
            else
                serviceCache.FindAsync(criteria, userState);
        }
        //
        // Summary:
        //     Opens the Roniz.WCF.Discovery.Extenstions.DiscoveryClientEx.
        public void Open()
        {
            innerDiscoveryClient.Open();
            syncManager.Open();
        }
        //
        // Summary:
        //     Begins an asynchronous resolve operation with the specified criteria.
        //
        // Parameters:
        //   criteria:
        //     The criteria for matching a service endpoint.
        //
        // Returns:
        //     The discoverable metadata of the service endpoint at the specified address,
        //     or null if no service endpoint is found at the specified address.
        public ResolveResponse Resolve(ResolveCriteria criteria)
        {
            return innerDiscoveryClient.Resolve(criteria);
        }
        //
        // Summary:
        //     Begins an asynchronous resolve operation with the specified criteria.
        //
        // Parameters:
        //   criteria:
        //     The criteria for matching a service endpoint.
        public void ResolveAsync(ResolveCriteria criteria)
        {
            innerDiscoveryClient.ResolveAsync(criteria);
        }
        //
        // Summary:
        //     Begins an asynchronous resolve operation with the specified criteria and
        //     user-defined state object.
        //
        // Parameters:
        //   criteria:
        //     The criteria for matching a service endpoint.
        //
        //   userState:
        //     A user specified object to identify the asynchronous resolve operation.
        public void ResolveAsync(ResolveCriteria criteria, object userState)
        {
            innerDiscoveryClient.ResolveAsync(criteria, userState);
        }

        #region private methods

        private void Initialize()
        {
            serviceCache = new SynchronizaedServiceCache();
            syncManager = new SynchronizationStateManager(serviceCache);
        }

        /// <summary>
        /// determine if the giving binding support request - replay messages
        /// </summary>
        /// <param name="binding">The binding to test</param>
        /// <returns>true if support , otherwise - false</returns>
        private bool IsBindingSupportRequestReplay(Binding binding)
        {
            if (binding is NetPeerTcpBinding)
            {
                return false;
            }
            if (binding is NetMsmqBinding)
            {
                return false;
            }
            if (binding is System.ServiceModel.MsmqIntegration.MsmqIntegrationBinding)
            {
                return false;
            }
            return true;
        }

        #endregion
        #endregion
    }
}
