using System.ServiceModel.Discovery;

namespace ServiceMonitor.MainApplication.TemplateSelectors
{
    public sealed class ListenUrisTemplateSelector : EndpointDiscoveryMetadataMultiOrSingleItemTemplateSelector
    {
        protected override bool? IsMultipleItems(EndpointDiscoveryMetadata item)
        {
            if (item.ListenUris == null)
                return null;
            return item.ListenUris.Count > 1;
        }
    }
}