using System.Collections.Generic;
using System.ServiceModel.Discovery;

namespace ServiceMonitor.MainApplication.TemplateSelectors
{
    public abstract class EndpointDiscoveryMetadataMultiOrSingleItemTemplateSelector : MultiOrSingleItemTemplateSelector
    {
        protected override bool? IsMultipleItems(object item)
        {
            return IsMultipleItems(((KeyValuePair<long, EndpointDiscoveryMetadata>) item).Value);
        }

        protected abstract bool? IsMultipleItems(EndpointDiscoveryMetadata item);
    }
}