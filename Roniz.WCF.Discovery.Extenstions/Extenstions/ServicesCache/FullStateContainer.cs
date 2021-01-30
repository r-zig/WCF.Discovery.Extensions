using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roniz.WCF.P2P.Sync.Messages.BusinessLogic;
using System.Runtime.Serialization;
using System.ServiceModel.Discovery;

namespace Roniz.WCF.Discovery.Extenstions.ServicesCache
{
    /// <summary>
    /// Provide full state container of the online services known by the sending peer
    /// </summary>
    [DataContract]
    class FullStateContainer : BusinessLogicMessageBase
    {
        [DataMember]
        public Dictionary<Criteria, List<FindResponse>> OnlineServices { get; set; }
    }
}
