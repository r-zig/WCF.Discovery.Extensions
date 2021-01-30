using System;
using Roniz.WCF.Discovery.Extenstions.Behaviors.Address;
using Roniz.WCF.Discovery.Extenstions.Behaviors.ServiceUniqueness;

namespace Roniz.WCF.Discovery.Extenstions.WcfService
{
    [UniqueServiceIdDiscoveryBehavior]
    [AddressDiscoveryServiceBehavior(AddressingOptions = AddressingOptions.GlobalAddressIpv4)]
    [LocalAddressDiscoveryServiceBehavior]
    //[BindingDiscoveryServiceBehavior]
    public class Service1 : IService1 , IService2
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string GetData2(int value)
        {
            return value.ToString();
        }
    }
}
