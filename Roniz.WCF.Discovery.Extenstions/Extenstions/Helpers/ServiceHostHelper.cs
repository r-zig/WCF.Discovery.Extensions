using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using Roniz.Diagnostics.Logging;

namespace Roniz.WCF.Discovery.Extenstions.Helpers
{
    /// <summary>
    /// Provide helper methods for service host
    /// </summary>
    public static class ServiceHostHelper
    {
        #region members

        private static ReadOnlyCollection<EndpointDiscoveryMetadata> EmptyCollection = new ReadOnlyCollection<EndpointDiscoveryMetadata>(new List<EndpointDiscoveryMetadata>(0));

        #endregion

        #region methods

        /// <summary>
        /// Get a read-only collection of published endpoints of the service
        /// </summary>
        /// <param name="serviceHost">The service to find his endpoints</param>
        /// <returns>A read-only collection of published endpoints. or empty if there is no discovery</returns>
        /// <remarks>
        /// If the service host is not opened yet it will return empty collection.
        /// Prefer use the event based pattern that wait until the service host will move to opened state and raise the response.
        /// </remarks>
        private static ReadOnlyCollection<EndpointDiscoveryMetadata> GetInternalPublishedEndpoints(this ServiceHostBase serviceHost)
        {
            Contract.Requires<ArgumentNullException>(serviceHost != null, "serviceHost cannot be null");
            DiscoveryServiceExtension extension = serviceHost.Extensions.Find<DiscoveryServiceExtension>();
            if (extension == null)
                return EmptyCollection;

            return extension.PublishedEndpoints;
        }

        /// <summary>
        /// Asynchronously get a read-only collection of published endpoints of the service - by calling the provide callback and give the response there
        /// </summary>
        /// <param name="serviceHost">The service to find his endpoints</param>
        /// <param name="callback">
        /// The callback that will raise when the response is ready.
        /// The callback accept first parameter of ReadOnlyCollection<EndpointDiscoveryMetadata> of the response endpoints
        /// And second parameter Boolean flag that determine if timeout occur (true) or not (false).
        /// </param>
        /// <param name="timeout">optional parameter to specify timeout for waiting to response</param>
        /// <remarks>
        /// This method will response asynchronously the published endpoints for the given service host.
        /// If the service host is not opened yet it will wait until it will opened or if timeout specified until it occur.
        /// if timeout occur it will return empty collection.
        /// </remarks>
        public static void GetPublishedEndpointsAsync(this ServiceHostBase serviceHost, Action<ReadOnlyCollection<EndpointDiscoveryMetadata>, bool> callback, int timeout = -1)
        {
            Contract.Requires<ArgumentNullException>(serviceHost != null, "serviceHost cannot be null");
            Contract.Requires<ArgumentException>(serviceHost.State != CommunicationState.Faulted, "serviceHost cannot be in Faulted state");
            Contract.Requires<ArgumentException>(serviceHost.State != CommunicationState.Closing, "serviceHost cannot be in closing state");
            Contract.Requires<ArgumentException>(serviceHost.State != CommunicationState.Closed, "serviceHost cannot be in closed state");
            Contract.Requires<ArgumentNullException>(callback != null, "callback cannot be null");
            Contract.Requires<ArgumentOutOfRangeException>(timeout >= -1, "timeout must be >= -1 (-1 mean wait forever as in WaitHandle.Wait())");

            ManualResetEventSlim waitHandle = new ManualResetEventSlim(serviceHost.State == CommunicationState.Opened);

            EventHandler opened = (s, e) =>
            {
                waitHandle.Set();
            };

            serviceHost.Opened += opened;

            //if the service moved to opened state before the registration to the event complete
            if (serviceHost.State == CommunicationState.Opened)
            {
                var result = serviceHost.GetInternalPublishedEndpoints();
                callback(result, false);

                serviceHost.Opened -= opened;
                return;
            }

            Task.Factory.StartNew(() =>
            {
                bool signald = false;
                try
                {
                    signald = waitHandle.Wait(timeout);
                }
                catch (InvalidOperationException exception)
                {
                    LogManager.GetCurrentClassLogger().Error(exception);
                }
                serviceHost.Opened -= opened;
                if (!signald)
                {
                    callback(EmptyCollection, true);
                }
                else
                {
                    callback(serviceHost.GetInternalPublishedEndpoints(), false);
                }
            }).HandleException();
        }

        /// <summary>
        /// Get a read-only collection of published endpoints of the service
        /// </summary>
        /// <param name="serviceHost">The service to find his endpoints</param>
        /// <param name="timeout">optional parameter to specify timeout for waiting to response</param>
        /// <remarks>
        /// This method will response synchronously the published endpoints for the given service host.
        /// If the service host is not opened yet it will wait until it will opened or if timeout specified until it occur.
        /// if timeout occur it will return empty collection.
        /// </remarks>
        public static ReadOnlyCollection<EndpointDiscoveryMetadata> GetPublishedEndpoints(this ServiceHostBase serviceHost, int timeout = -1)
        {
            Contract.Requires<ArgumentNullException>(serviceHost != null, "serviceHost cannot be null");
            Contract.Requires<ArgumentException>(serviceHost.State != CommunicationState.Faulted, "serviceHost cannot be in Faulted state");
            Contract.Requires<ArgumentException>(serviceHost.State != CommunicationState.Closing, "serviceHost cannot be in closing state");
            Contract.Requires<ArgumentException>(serviceHost.State != CommunicationState.Closed, "serviceHost cannot be in closed state");
            Contract.Requires<ArgumentOutOfRangeException>(timeout >= -1, "timeout must be >= -1 (-1 mean wait forever as in WaitHandle.Wait())");

            ManualResetEventSlim waitHandle = new ManualResetEventSlim(serviceHost.State == CommunicationState.Opened);

            EventHandler opened = (s, e) =>
            {
                waitHandle.Set();
            };

            serviceHost.Opened += opened;

            //if the service moved to opened state before the registration to the event complete
            if (serviceHost.State == CommunicationState.Opened)
            {
                var result = serviceHost.GetInternalPublishedEndpoints();
                serviceHost.Opened -= opened;
                return result;
            }

            bool signald = false;
            try
            {
                signald = waitHandle.Wait(timeout);
            }
            catch (InvalidOperationException exception)
            {
                LogManager.GetCurrentClassLogger().Error(exception);
            }
            serviceHost.Opened -= opened;
            if (!signald)
            {
                return EmptyCollection;
            }
            else
            {
                return serviceHost.GetInternalPublishedEndpoints();
            }
        }
        #endregion
    }
}
