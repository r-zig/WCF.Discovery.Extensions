using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml.Linq;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors
{
    /// <summary>
    /// interface that the developer should inherit to provide XElement that will be inserted into the EndpointDiscoveryBehavior Extensions collection
    /// </summary>
    public interface IExtensionProvider
    {
        XElement ProvideExtension(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,ServiceEndpoint endpoint);
    }
}
