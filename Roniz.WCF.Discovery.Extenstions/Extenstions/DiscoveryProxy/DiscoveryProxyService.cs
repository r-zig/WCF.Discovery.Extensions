// AsyncResult.cs
//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Xml;
using Roniz.Diagnostics.Logging;

namespace Roniz.WCF.Discovery.Extenstions.DiscoveryProxy
{
    // Implement DiscoveryProxy by extending the DiscoveryProxy class and overriding the abstract methods
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class DiscoveryProxyService : System.ServiceModel.Discovery.DiscoveryProxy
    {
        #region members
        // Repository to store EndpointDiscoveryMetadata. A database or a flat file could also be used instead.
        private readonly Dictionary<EndpointAddress, EndpointDiscoveryMetadata> onlineServices;
        private readonly object syncLock = new object();
        #endregion

        #region events
        public event EventHandler<EndpointDiscoveryMetadataEventArgs> Added;
        public event EventHandler<EndpointDiscoveryMetadataEventArgs> Removed;
        #endregion

        #region constructor
        public DiscoveryProxyService()
        {
            this.onlineServices = new Dictionary<EndpointAddress, EndpointDiscoveryMetadata>();
        }
        #endregion

        #region methods

        private void InvokeAdded(EndpointDiscoveryMetadataEventArgs e)
        {
            EventHandler<EndpointDiscoveryMetadataEventArgs> handler = Added;
            if (handler != null) handler(this, e);
        }

        private void InvokeRemoved(EndpointDiscoveryMetadataEventArgs e)
        {
            EventHandler<EndpointDiscoveryMetadataEventArgs> handler = Removed;
            if (handler != null) handler(this, e);
        }

        // OnBeginOnlineAnnouncement method is called when a Hello message is received by the Proxy
        protected override IAsyncResult OnBeginOnlineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
        {
            this.AddOnlineService(endpointDiscoveryMetadata);
            return new OnOnlineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOnlineAnnouncement(IAsyncResult result)
        {
            OnOnlineAnnouncementAsyncResult.End(result);
        }

        // OnBeginOfflineAnnouncement method is called when a Bye message is received by the Proxy
        protected override IAsyncResult OnBeginOfflineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
        {
            this.RemoveOnlineService(endpointDiscoveryMetadata);
            return new OnOfflineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOfflineAnnouncement(IAsyncResult result)
        {
            OnOfflineAnnouncementAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Probe request message is received by the Proxy
        protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback, object state)
        {
            LogManager.GetCurrentClassLogger().Debug("OnBeginFind {0}", findRequestContext);
            this.MatchFromOnlineService(findRequestContext);
            LogManager.GetCurrentClassLogger().Debug("OnBeginFind after match {0}", findRequestContext);
            return new OnFindAsyncResult(callback, state);
        }

        protected override void OnEndFind(IAsyncResult result)
        {
            OnFindAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Resolve request message is received by the Proxy
        protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback, object state)
        {
            return new OnResolveAsyncResult(this.MatchFromOnlineService(resolveCriteria), callback, state);
        }

        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            return OnResolveAsyncResult.End(result);
        }

        // The following are helper methods required by the Proxy implementation
        public void AddOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            lock (this.syncLock)
            {
                this.onlineServices[endpointDiscoveryMetadata.Address] = endpointDiscoveryMetadata;
            }

            InvokeAdded(new EndpointDiscoveryMetadataEventArgs(endpointDiscoveryMetadata));
            PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Adding");
        }

        public void RemoveOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            if (endpointDiscoveryMetadata != null)
            {
                lock (this.syncLock)
                {
                    this.onlineServices.Remove(endpointDiscoveryMetadata.Address);
                }

                InvokeRemoved(new EndpointDiscoveryMetadataEventArgs(endpointDiscoveryMetadata));
                PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Removing");
            }
        }

        void MatchFromOnlineService(FindRequestContext findRequestContext)
        {
            lock (this.syncLock)
            {
                foreach (EndpointDiscoveryMetadata endpointDiscoveryMetadata in this.onlineServices.Values)
                {
                    if (findRequestContext.Criteria.IsMatch(endpointDiscoveryMetadata))
                    {
                        findRequestContext.AddMatchingEndpoint(endpointDiscoveryMetadata);
                    }
                }
            }
        }

        EndpointDiscoveryMetadata MatchFromOnlineService(ResolveCriteria criteria)
        {
            EndpointDiscoveryMetadata matchingEndpoint = null;
            lock (this.syncLock)
            {
                foreach (EndpointDiscoveryMetadata endpointDiscoveryMetadata in this.onlineServices.Values)
                {
                    if (criteria.Address == endpointDiscoveryMetadata.Address)
                    {
                        matchingEndpoint = endpointDiscoveryMetadata;
                    }
                }
            }
            return matchingEndpoint;
        }

        void PrintDiscoveryMetadata(EndpointDiscoveryMetadata endpointDiscoveryMetadata, string verb)
        {
            LogManager.GetCurrentClassLogger().Debug("\n**** " + verb + " service of the following type from cache. ");
            foreach (XmlQualifiedName contractName in endpointDiscoveryMetadata.ContractTypeNames)
            {
                LogManager.GetCurrentClassLogger().Debug("** " + contractName);
                break;
            }
            LogManager.GetCurrentClassLogger().Debug("**** Operation Completed");
        }
        #endregion        
    }
}
