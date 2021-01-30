using System.ServiceModel.Discovery;

namespace ServiceMonitor.MainApplication.TemplateSelectors
{
    public sealed class ScopesTemplateSelector : EndpointDiscoveryMetadataMultiOrSingleItemTemplateSelector
    {
        protected override bool? IsMultipleItems(EndpointDiscoveryMetadata item)
        {
            if (item.Scopes == null)
                return null;
            return item.Scopes.Count > 1;
        }
    }
}