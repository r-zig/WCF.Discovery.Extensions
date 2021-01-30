using System.ServiceModel.Discovery;

namespace ServiceMonitor.MainApplication.TemplateSelectors
{
    public sealed class ContractNamesTemplateSelector : EndpointDiscoveryMetadataMultiOrSingleItemTemplateSelector
    {
        protected override bool? IsMultipleItems(EndpointDiscoveryMetadata item)
        {
            if (item.ContractTypeNames == null)
                return null;
            return item.ContractTypeNames.Count > 1;
        }
    }
}