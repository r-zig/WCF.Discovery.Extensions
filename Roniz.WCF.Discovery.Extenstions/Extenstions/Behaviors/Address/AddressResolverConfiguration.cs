using System;
using System.Configuration;
using Roniz.Common.Configuration;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors.Address
{
    /// <summary>
    /// Provide default configuration for the Address resolving behavior
    /// </summary>
    public class AddressResolverConfiguration : BaseConfigurationSection<AddressResolverConfiguration>
    {
        #region private members
        #endregion

        #region public members
        public const string TimeoutProperty = "Timeout";
        public const string TimeoutDefaultValue = "00:01:00";
        #endregion


        #region Properties
        
        /// <summary>
        /// Timeout attribute.
        /// Used to determine how much time the process of resolving the global address can take before timeout occur
        /// </summary>
        [ConfigurationProperty(TimeoutProperty, DefaultValue = TimeoutDefaultValue)]
        [PositiveTimeSpanValidator]
        public TimeSpan Timeout
        {
            get
            {
                return (TimeSpan)this[TimeoutProperty];
            }
            set
            {
                this[TimeoutProperty] = value;
            }
        }

        #endregion
    }
}
