using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Xml.Linq;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors
{
    public abstract class ExtensionsServiceBehaviorBase : ServiceBehaviorBase
    {
        #region Members
        protected IExtensionProvider ExtensionProvider;
        #endregion

        #region Constructores

        protected ExtensionsServiceBehaviorBase()
        {
        }

        protected ExtensionsServiceBehaviorBase(IExtensionProvider extensionProvider)
        {
            Initialize(extensionProvider);
        }

        protected ExtensionsServiceBehaviorBase(Type extensionProviderType)
        {
            var instance = (IExtensionProvider)Activator.CreateInstance(extensionProviderType);
            Initialize(instance);
        }

        #endregion

        #region Methods

        protected override void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, ServiceEndpoint endpoint)
        {
            bool shouldAddBehavior = false;
            var endpointDiscoveryBehavior = endpoint.Behaviors.Find<EndpointDiscoveryBehavior>();
            if (endpointDiscoveryBehavior == null)
            {
                shouldAddBehavior = true;
                endpointDiscoveryBehavior = new EndpointDiscoveryBehavior();
            }

            // add the binding information to the endpoint
            endpointDiscoveryBehavior.Extensions.Add(ProvideExtension(serviceDescription, serviceHostBase, endpoint));

            // add the extension
            if (shouldAddBehavior)
                endpoint.Behaviors.Add(endpointDiscoveryBehavior);
        }

        private void ThrowIfNotInitialize()
        {
            if (ExtensionProvider == null)
            {
                throw new Exception("The service behavior not initialized , verify if called parameter less constructor without called Initialize in the derived behavior");
            }
        }

        protected void Initialize(IExtensionProvider instance)
        {
            ExtensionProvider = instance;
        }

        protected virtual XElement ProvideExtension(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, ServiceEndpoint endpoint)
        {
            ThrowIfNotInitialize();
            return ExtensionProvider.ProvideExtension(serviceDescription, serviceHostBase, endpoint);
        }
        
        #endregion

        #region Properties
        #endregion
    }
}
