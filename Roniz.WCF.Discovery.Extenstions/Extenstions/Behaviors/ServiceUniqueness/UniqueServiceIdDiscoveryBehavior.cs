namespace Roniz.WCF.Discovery.Extenstions.Behaviors.ServiceUniqueness
{
    /// <summary>
    /// provide the service discovery with extension that add UniqueId of the service , 
    /// that help distinguish between the various instances of the same service from the same address
    /// </summary>
    /// <remarks>When using discovery for your WCF service , 
    /// it is possible that you have various instances of the same service that can run on the same time.
    /// If there is other service (or client) that should take action for the specific instance , you can use this service behavior on the service that should be "discovered".
    /// and on the there side when discovery happen you can use the UniqueId to hold some hash table that contain the same services per id</remarks>
    public sealed class UniqueServiceIdDiscoveryBehavior : ExtensionsServiceBehaviorBase
    {
        #region Constructores
        public UniqueServiceIdDiscoveryBehavior():base(typeof(UniqueServiceExtensionProvider))
        {

        }
        #endregion
    }
}