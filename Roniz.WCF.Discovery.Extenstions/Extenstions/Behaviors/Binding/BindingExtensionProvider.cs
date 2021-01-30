using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml.Linq;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors.Binding
{
    internal sealed class BindingExtensionProvider: IExtensionProvider
    {
        #region Members
        private const string BindingXName = "Binding";
        #endregion

        #region Constructores
        public BindingExtensionProvider()
        {
            
        }
        #endregion

        #region Methods
        #region IExtensionProvider Members

        public XElement ProvideExtension(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, ServiceEndpoint endpoint)
        {
            throw new NotImplementedException("should implement WsdlExporter/WsdlImporter...");
            object[] bindingInfo;
            return new XElement(BindingXName, bindingInfo);
        }

        #endregion
        #endregion
    }
}