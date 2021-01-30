using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;

namespace Roniz.WCF.Discovery.Extenstions.ServicesCache
{
    public interface IDiscoveryClient
    {
        #region events
        
        // Summary:
        //     Occurs when the entire find operation completes.
        event EventHandler<FindCompletedEventArgs> FindCompleted;
        //
        // Summary:
        //     Occurs every time the client receives a response from a particular service.
        event EventHandler<FindProgressChangedEventArgs> FindProgressChanged;

        #endregion
    }
}
