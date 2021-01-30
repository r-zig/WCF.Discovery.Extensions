using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ServiceModel.Description;
using System.ServiceModel;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors.Address
{
    /// <summary>
    /// provide the service discovery with extension that add the real local address of every endpoint.
    /// </summary>
    /// <remarks>When using discovery for your WCF service , 
    /// and the endpoint address listening on "localhost" address with port 0 (zero)
    /// the discovery does not expose the real port.
    /// and client's that receive this information cannot connect to the service.
    /// Use the extensions and not the Address of the endpoint because this code will run when opening the service , after already read the address from configuration</remarks>
    internal sealed class LocalAddressExtensionProvider : IExtensionProvider
    {
        #region Members

        private const string AddressXName = "LocalAddress";

        #endregion

        #region Constructores
        public LocalAddressExtensionProvider()
        {
        }
        #endregion

        #region Properties

        #endregion

        #region Methods

        #region IExtensionProvider Members

        public XElement ProvideExtension(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, ServiceEndpoint endpoint)
        {
#warning return port ZERO //TODO
            return new XElement(AddressXName, endpoint.Address.ToString());
        }

        #endregion

        #endregion
    }
}
