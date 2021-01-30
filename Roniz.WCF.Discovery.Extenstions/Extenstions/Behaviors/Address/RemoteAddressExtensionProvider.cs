using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Xml.Linq;
using Roniz.Networking.Client;
using System.Threading.Tasks;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors.Address
{
    /// <summary>
    /// provide the service discovery with extension that add the real address of every endpoint.
    /// </summary>
    /// <remarks>When using discovery for your WCF service , 
    /// and the endpoint address listening on "localhost" address
    /// the discovery does not expose the real address.
    /// and client's that receive this information cannot connect to the service.
    /// Use the extensions and not the Address of the endpoint because this code will run when opening the service , after already read the address from configuration</remarks>
    internal sealed class RemoteAddressExtensionProvider : IExtensionProvider
    {
        #region Members
        private const string AddressXName = "RemoteAddress";
        //private static MemoryCache _addressCache;
        private IEndpointResolver endpointResolver;
        private AddressResolvingMode addressResolvingMode;
        private IPEndPoint serverIpEndPoint;
        #endregion

        #region Constructores
        public RemoteAddressExtensionProvider()
        {
            CacheAddressAbsoluteExpirationInterval = DateTimeOffset.MaxValue.Ticks;
            OnAddressResolvingModeChanged();
        }

        //static AddressExtensionProvider()
        //{
        //    _addressCache = new MemoryCache("address");
        //}
        #endregion

        #region Properties

        /// <summary>
        /// Whether to automatic start resolving the address when the resolver created , or wait until the service explicity need the information
        /// </summary>
        public bool AutoResolve { get; set; }

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
                if (serverIpEndPoint != value)
                {
                    serverIpEndPoint = value;
                    OnAddressResolvingModeChanged();
                }
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
                if (addressResolvingMode != value)
                {
                    addressResolvingMode = value;
                    OnAddressResolvingModeChanged();
                }
            }
        }

        public AddressingOptions AddressingOptions { get; set; }

        /// <summary>
        /// determine how much time a resolved address should stay in the cache
        /// if this value is null , it will stay there forever...
        /// </summary>
        public long? CacheAddressAbsoluteExpirationInterval { get; set; }

        #endregion

        #region Methods
        #region IExtensionProvider Members

        public XElement ProvideExtension(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, ServiceEndpoint endpoint)
        {
            XElement addressInfo = ResolveAddresses(endpoint.Address);
            return new XElement(AddressXName, addressInfo);
        }

        private XElement ResolveAddresses(EndpointAddress address)
        {
            XElement resolvedAddresses;
            string key = address.ToString();

            #region caching
            //resolvedAddresses = _addressCache.Get(key) as XElement;

            //if (resolvedAddresses != null)
            //    return resolvedAddresses;
            #endregion

            //resolve the address
            resolvedAddresses = ResolveAddressesCore(address);

            #region caching
            ////insert to the cache
            //var cacheItemPolicy = new CacheItemPolicy();
            //if (CacheAddressAbsoluteExpirationInterval.HasValue)
            //{
            //    DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
            //    try
            //    {
            //        if ((cacheItemPolicy.AbsoluteExpiration.Ticks + CacheAddressAbsoluteExpirationInterval.Value) >= DateTimeOffset.MaxValue.Ticks)
            //        {
            //            cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.MaxValue;
            //        }
            //        else
            //        {
            //            cacheItemPolicy.AbsoluteExpiration =
            //            dateTimeOffset.AddTicks(CacheAddressAbsoluteExpirationInterval.Value);
            //        }
            //    }
            //    catch(ArgumentOutOfRangeException)
            //    {
            //        cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.MaxValue;
            //    }

            //}
            //_addressCache.Add(key, resolvedAddresses, cacheItemPolicy);
            #endregion
            return resolvedAddresses;
        }

        private XElement ResolveAddressesCore(EndpointAddress address)
        {
            var resolvedAddresses = new XElement("Address");

            if (AddressingOptions == AddressingOptions.HostName)
            {
                resolvedAddresses.Add(GetHostName());
            }

            if (AddressingOptions == AddressingOptions.HostIp)
            {
                resolvedAddresses.Add(GetHostIp());
            }

            if (AddressingOptions == AddressingOptions.GlobalAddressIpv4)
            {
                resolvedAddresses.Add(GetGlobalAddressIpv4(AddressResolverConfiguration.Settings.Timeout));
            }

            if (AddressingOptions == AddressingOptions.HostIp)
            {
                resolvedAddresses.Add(GetGlobalAddressIpv6());
            }

            return resolvedAddresses;
        }

        private XElement GetGlobalAddressIpv4(TimeSpan timeout)
        {
            var waitHandle = new ManualResetEvent(true);
            if (endpointResolver.IsBusy || endpointResolver.ExternalV4Endpoint == null)
            {
                waitHandle.Reset();
                endpointResolver.ResolveExternalV4EndpointCompleted += (s, e) =>
                {
                    waitHandle.Set();
                };

                //if the ResolveExternalV4EndpointCompleted happen before we register to it
                if (endpointResolver.ExternalV4Endpoint != null)
                    waitHandle.Set();
            }

            #warning resolve problem of deadlock until resolve it correctly in the ResolveExternalV4EndpointAsync
            Task.Factory.StartNew(()=> endpointResolver.ResolveExternalV4EndpointAsync());
            //endpointResolver.ResolveExternalV4EndpointAsync();
            var signald = waitHandle.WaitOne(timeout);
            if(!signald)
                throw new TimeoutException(string.Format(ErrorMessages.GetGlobalAddressIpv4Timeout,timeout));
            var address = endpointResolver.ExternalV4Endpoint;
            if (address == null)
                throw new Exception(string.Format(ErrorMessages.GetGlobalAddressIpv4AddressIsNull, timeout));

            #region sync resolving
            /* 
             * will stack the all service open if will not found any external service 
             * to resolve it's own address
             */
            //var address = endpointResolver.ResolveExternalV4Endpoint();
            #endregion
            return new XElement(AddressingOptions.GlobalAddressIpv4.ToString(), address.ToString());
        }

        private XElement GetGlobalAddressIpv6()
        {
            IPAddress address;
            throw new NotImplementedException();
            return new XElement("GlobalAddressIpv6", address.ToString());
        }

        private static XElement GetHostName()
        {
            return new XElement("HostName", Dns.GetHostName());
        }

        private static XElement GetHostIp()
        {
            var element = new XElement("HostAddresses");
            try
            {
                var addresses = Dns.GetHostAddresses(Dns.GetHostName());
                element.Add(addresses);
            }
            catch
            {
                //TODO log?
            }
            return element;
        }

        /// <summary>
        /// When AddressResolvingMode property changed
        /// </summary>
        private void OnAddressResolvingModeChanged()
        {
            if (endpointResolver != null && endpointResolver.IsBusy)
            {
                endpointResolver.ResolveExternalV4EndpointAsyncCancel();
            }

            switch (AddressResolvingMode)
            {
                case AddressResolvingMode.Dynamic:
                    {
                        endpointResolver = EndpointResolverFactory.CreateDynamicEndpointResovler();
                        break;
                    }
                case AddressResolvingMode.Static:
                    {
                        if (ServerIpEndPoint != null)
                        {
                            endpointResolver = EndpointResolverFactory.CreateStaticEndpointResovler(ServerIpEndPoint.Address, ServerIpEndPoint.Port);
                        }
                        break;
                    }
            }

            if (AutoResolve)
            {
                endpointResolver.ResolveExternalV4EndpointAsync();
            }
        }
        #endregion

        public static IPEndPoint GetGlobalAddressIpv4(XElement xElement)
        {
            if (xElement == null)
                return null;

            var content = xElement.Value;
            var split = content.Split(':');
            IPAddress ipAddress;
            if (!IPAddress.TryParse(split[0], out ipAddress))
                return null;

            int port;
            if (!int.TryParse(split[1], out port))
                return null;
            var ipEndPoint = new IPEndPoint(ipAddress, port);
            return ipEndPoint;
        }
        #endregion
    }
}
