/*
 * based on the article of 
 * http://www.codeproject.com/KB/WCF/WCFDiscovery.aspx
 * http://mikeperetz.blogspot.com/
 */
namespace Roniz.WCF.Discovery.Extenstions.Behaviors.Binding
{
    /// <summary>
    /// provide the service discovery with extension that add the binding of every endpoint. 
    /// </summary>
    /// <remarks>When using discovery for your WCF service , the discovery does not expose the binding , only the address</remarks>
    public sealed class BindingDiscoveryServiceBehavior : ExtensionsServiceBehaviorBase
    {
        #region Constructores
        public BindingDiscoveryServiceBehavior():base(typeof(BindingExtensionProvider))
        {

        }
        #endregion
    }
}
