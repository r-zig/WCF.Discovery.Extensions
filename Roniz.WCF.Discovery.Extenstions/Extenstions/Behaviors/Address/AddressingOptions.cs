using System;

namespace Roniz.WCF.Discovery.Extenstions.Behaviors.Address
{
    [Flags]
    public enum AddressingOptions
    {
        //provide the machine name that DNS can replace it with real address
        HostName = 1,
        //provide the machine address that when don't want to use DNS
        HostIp = 2,
        //provide global ip address for ipv4
        GlobalAddressIpv4 = 4,
        //provide global ip address for ipv6
        GlobalAddressIpv6 = 8,
    }
}