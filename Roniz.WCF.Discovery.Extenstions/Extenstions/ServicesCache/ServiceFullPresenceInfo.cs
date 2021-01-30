using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roniz.WCF.P2P.Messages.Presence;
using System.Runtime.Serialization;
using System.ServiceModel.Discovery;

namespace Roniz.WCF.Discovery.Extenstions.ServicesCache
{
    /// <summary>
    /// represent response of one service
    /// </summary>
    [DataContract]
    public class ServiceFullPresenceInfo : FullPresenceInfo
    {
        [DataMember]
        public Criteria Criteria { get; set; }

        [DataMember]
        public List<FindResponse> FindResponse { get; set; }
    }
}
