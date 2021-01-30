using System.Net;
namespace Roniz.WCF.Discovery.Extenstions.Behaviors.Address
{
    public sealed class AddressDiscoveryServiceBehavior : ExtensionsServiceBehaviorBase
    {
        #region members
        
        private AddressingOptions addressingOptions = AddressingOptions.GlobalAddressIpv4;
        private AddressResolvingMode addressResolvingMode = AddressResolvingMode.Dynamic;
        private IPEndPoint serverIpEndPoint;
        private bool autoResolve = false;
        #endregion

        #region Properties

        /// <summary>
        /// Whether to automatic start resolving the address when the resolver created , or wait until the service explicity need the information
        /// </summary>
        public bool AutoResolve 
        {
            get
            {
                return autoResolve;
            }
            set
            {
                if (AddressExtensionProvider != null)
                {
                    AddressExtensionProvider.AutoResolve = value;
                }
                autoResolve = value;
            }
        }

        /// <summary>
        /// when using AddressResolvingMode.Static the inner resolver will use this property to obtain the destination service
        /// </summary>
        public IPEndPoint ServerIpEndPoint
        {
            get
            {
                return serverIpEndPoint;
            }
            set
            {
                if (AddressExtensionProvider != null)
                {
                    AddressExtensionProvider.ServerIpEndPoint = value;
                }
                serverIpEndPoint = value;
            }
        }
        /// <summary>
        /// The resolving mode that is used to resolve network properties
        /// </summary>
        public AddressResolvingMode AddressResolvingMode
        {
            get
            {
                return addressResolvingMode;
            }
            set
            {
                if (AddressExtensionProvider != null)
                {
                    AddressExtensionProvider.AddressResolvingMode = value;
                }
                addressResolvingMode = value;
            }
        }

        public AddressingOptions AddressingOptions
        {
            get
            {
                return addressingOptions;
            }
            set
            {
                if (AddressExtensionProvider != null)
                {
                    AddressExtensionProvider.AddressingOptions = value;
                }
                addressingOptions = value;
            }
        }

        private RemoteAddressExtensionProvider AddressExtensionProvider
        {
            get
            {
                return ((RemoteAddressExtensionProvider)ExtensionProvider);
            }
        }
        
        #endregion

        #region Constructores
        public AddressDiscoveryServiceBehavior()
        {
            var instance = new RemoteAddressExtensionProvider
                                              {
                                                  AddressResolvingMode = AddressResolvingMode,
                                                  AddressingOptions = AddressingOptions,
                                                  ServerIpEndPoint = serverIpEndPoint,
                                                  AutoResolve = autoResolve
                                              };
            Initialize(instance);
        }

        #endregion

        #region Methods

        #endregion
    }
}