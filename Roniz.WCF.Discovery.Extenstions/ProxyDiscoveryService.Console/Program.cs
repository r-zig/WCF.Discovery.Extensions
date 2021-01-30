// program.cs
//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Xml;
using Roniz.WCF.Discovery.Extenstions.DiscoveryProxy;
using Roniz.WCF.Discovery.Extenstions.Endpoints;

namespace ProxyDiscoveryService.Console
{
    class Program
    {
        public static void Main()
        {
            // Host the DiscoveryProxy service
            var discoveryProxyService = new DiscoveryProxyService();
            var serviceHost = new ServiceHost(discoveryProxyService);
            ServiceHost proxyServiceHost = serviceHost;

            try
            {
                System.Console.WriteLine("Press Spacebar key to inject dummy metadata into the discovery service , or any other key to continue normally");
                var readKey = System.Console.ReadKey();
                if(readKey.Key == ConsoleKey.Spacebar)
                    discoveryProxyService.AddOnlineService(new EndpointDiscoveryMetadata
                                                               {
                                                                   Address = new EndpointAddress("http://dummyurl.blabla")
                                                               });
                // Add DiscoveryEndpoint to receive Probe and Resolve messages
                //Uri probeEndpointAddress = new Uri("net.tcp://localhost:8001/Probe");
                //Uri announcementEndpointAddress = new Uri("net.tcp://localhost:9021/Announcement");
                //DiscoveryEndpoint discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(probeEndpointAddress));
                //DiscoveryEndpoint discoveryEndpoint = new P2PDiscoveryEndpoint(false);
                DiscoveryEndpoint discoveryEndpoint = new UdpDiscoveryEndpoint();
                discoveryEndpoint.IsSystemEndpoint = false;

                // Add AnnouncementEndpoint to receive Hello and Bye announcement messages
                //AnnouncementEndpoint announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(), new EndpointAddress(announcementEndpointAddress));
                //AnnouncementEndpoint announcementEndpoint = new P2PAnnouncementEndpoint();
                AnnouncementEndpoint announcementEndpoint = new UdpAnnouncementEndpoint();

                proxyServiceHost.AddServiceEndpoint(discoveryEndpoint);
                //proxyServiceHost.AddServiceEndpoint(announcementEndpoint);

                proxyServiceHost.Open();

                System.Console.WriteLine("Proxy Service started.");
                System.Console.WriteLine();
                System.Console.WriteLine("Press <ENTER> to terminate the service.");
                System.Console.WriteLine();
                System.Console.ReadLine();

                proxyServiceHost.Close();
            }
            catch (CommunicationException e)
            {
                System.Console.WriteLine(e.Message);
            }
            catch (TimeoutException e)
            {
                System.Console.WriteLine(e.Message);
            }

            if (proxyServiceHost.State != CommunicationState.Closed)
            {
                System.Console.WriteLine("Aborting the service...");
                proxyServiceHost.Abort();
            }
        }
    }
}
