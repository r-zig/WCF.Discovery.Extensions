using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml.Linq;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors.ServiceUniqueness
{
    sealed class UniqueServiceExtensionProvider : IExtensionProvider
    {
        #region Members
        private const string UniqueIdXName = "UniqueID";
        private readonly string uniqueId;
        #endregion

        #region Constructores
        public UniqueServiceExtensionProvider()
        {
            uniqueId = Guid.NewGuid().ToString();
        }
        #endregion

        #region Methods
        #region IExtensionProvider Members

        public XElement ProvideExtension(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, ServiceEndpoint endpoint)
        {
            return new XElement(UniqueIdXName, uniqueId);
        }

        #endregion
        #endregion
    }
}
