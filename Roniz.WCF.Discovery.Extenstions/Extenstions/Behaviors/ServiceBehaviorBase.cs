using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors
{
    public abstract class ServiceBehaviorBase : Attribute, IServiceBehavior
    {
        #region Properties
        /// <summary>
        /// whether to add the discovery behavior to SystemEndpoints or not
        /// </summary>
        protected bool IncludeSystemEndpoints { get; set; }
        #endregion

        #region IServiceBehavior Members
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var endpoints = serviceDescription.Endpoints;

            foreach (ServiceEndpoint endpoint in endpoints)
            {
                if (!IncludeSystemEndpoints && endpoint.IsSystemEndpoint)
                    continue;

                ApplyDispatchBehavior(serviceDescription, serviceHostBase, endpoint);
            }
        }

        protected abstract void ApplyDispatchBehavior(ServiceDescription serviceDescription,ServiceHostBase serviceHostBase, ServiceEndpoint endpoint);
        #endregion
    }
}