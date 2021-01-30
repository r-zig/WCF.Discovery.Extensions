
namespace Roniz.WCF.Discovery.Extenstions.Behaviors.Address
{
    /// <summary>
    /// The mode to resolve address
    /// </summary>
    public enum AddressResolvingMode
    {
        /// <summary>
        /// Resolve using dynamic behavior (when there is no known service end point)
        /// </summary>
        Dynamic,

        /// <summary>
        /// Resolve using static behavior (when there is known service end point)
        /// </summary>
        Static
    }
}
