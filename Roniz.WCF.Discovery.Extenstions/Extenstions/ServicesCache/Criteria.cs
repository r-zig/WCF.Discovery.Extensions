using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;
using System.Runtime.Serialization;
using Roniz.WCF.P2P.Messages.Presence;

namespace Roniz.WCF.Discovery.Extenstions.ServicesCache
{
    /// <summary>
    /// act as key for the resolve / find of services
    /// wrapper around the <ref>ResolveCriteria</ref> and/or <ref>FindCriteria</ref> and the optional userState
    /// </summary>
    [DataContract]
    public class Criteria : CompactPresenceInfo
    {
        #region members
        [DataMember]
        private object userState;
        
        [DataMember]
        private FindCriteria findCriteria;
        
        [DataMember]
        private ResolveCriteria resolveCriteria;
        #endregion

        #region properties

        /// <summary>
        /// The find criteria
        /// </summary>

        public FindCriteria FindCriteria
        {
            get
            {
                return findCriteria;
            }
        }

        /// <summary>
        /// The resolve criteria
        /// </summary>
        [DataMember]
        public ResolveCriteria ResolveCriteria
        {
            get
            {
                return resolveCriteria;
            }
        }

        /// <summary>
        /// the user state
        /// </summary>
        [DataMember]
        public object UserState
        {
            get
            {
                return userState;
            }
        }

        #endregion

        #region constructores
        public Criteria()
        {

        }
        /// <summary>
        /// initiate instance using FindCriteria
        /// </summary>
        /// <param name="findCriteria">the find criteria instance</param>
        public Criteria(FindCriteria findCriteria,object userState = null)
        {
            this.findCriteria = findCriteria;
            this.userState = userState;
        }

        /// <summary>
        /// initiate instance using ResolveCriteria
        /// </summary>
        /// <param name="resolveCriteria">the resolve criteria instance</param>
        public Criteria(ResolveCriteria resolveCriteria, object userState = null)
        {
            this.resolveCriteria = resolveCriteria;
            this.userState = userState;
        }
        #endregion
    }
}
