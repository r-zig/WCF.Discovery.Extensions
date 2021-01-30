using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;

namespace Roniz.WCF.Discovery.Extenstions.Endpoints
{
    class P2PDiscoveryDefaults
    {
        #region members

        public static readonly DiscoveryVersion DefaultDiscoveryVersion = DiscoveryVersion.WSDiscovery11;
        public static readonly Uri DefaultAnnouncementUri = new Uri("net.p2p://Roniz.WCF.Discovery.DefaultAnnouncement");
        public static readonly Uri DefaultAdHoc = new Uri("net.p2p://Roniz.WCF.Discovery.DefaultAdHoc");

        public static ServiceDiscoveryMode DefaultDiscoveryMode;

        #endregion

        #region methods
        public static Binding GetDefaultBinding()
        {
            return new NetPeerTcpBinding
            {
                Security = new PeerSecuritySettings
                {
                    Mode = SecurityMode.None,
                    Transport = new PeerTransportSecuritySettings
                    {
                        CredentialType = PeerTransportCredentialType.Password
                    }
                }
            };
        }
        #endregion
    }
}
