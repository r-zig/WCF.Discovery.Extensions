using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Roniz.WCF.P2P.Sync.Messages.BusinessLogic;

namespace Roniz.WCF.Discovery.Extenstions.ServicesCache
{
    /// <summary>
    /// Contain collection of criteria instances
    /// </summary>
    [DataContract]
    class CriteriasContainer : BusinessLogicMessageBase
    {
        [DataMember]
        public List<Criteria> Criterias { get; set; }
    }
}
