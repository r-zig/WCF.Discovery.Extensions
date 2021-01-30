using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors.Address
{
    public sealed class LocalAddressDiscoveryServiceBehavior : ExtensionsServiceBehaviorBase
    {
        #region members
        #endregion

        #region Properties
        #endregion

        #region Constructores
        public LocalAddressDiscoveryServiceBehavior()
        {
            var instance = new LocalAddressExtensionProvider();
            Initialize(instance);
        }

        #endregion
    }
}
