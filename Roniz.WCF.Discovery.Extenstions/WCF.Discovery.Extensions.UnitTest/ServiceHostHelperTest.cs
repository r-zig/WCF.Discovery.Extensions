using Roniz.WCF.Discovery.Extenstions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Collections.ObjectModel;
using System.Threading;

namespace WCF.Discovery.Extensions.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ServiceHostHelperTest and is intended
    ///to contain all ServiceHostHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ServiceHostHelperTest
    {

        ServiceHost serviceHostForAsyncTests;
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (serviceHostForAsyncTests != null)
            {
                serviceHostForAsyncTests.Close();
                serviceHostForAsyncTests = null;
            }
        }
        
        #endregion

        private ServiceHost GetServiceHostWithEndpoints(int numperOfDiscoveredEndpoints)
        {
            string baseAddresses = "net.pipe://localhost/Async1MockTest";
            serviceHostForAsyncTests = new ServiceHost(typeof(Mock.MockService));

            for (int i = 0; i < numperOfDiscoveredEndpoints; i++)
            {
                serviceHostForAsyncTests.AddServiceEndpoint(typeof(IMockServiceContract), new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), string.Format("{0}/{1}", baseAddresses, i));
            }

            return serviceHostForAsyncTests;
        }

        private void MakeServiceHostDiscover(ServiceHostBase serviceHost)
        {
            var serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
            serviceDiscoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
            serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
            serviceHost.Description.Behaviors.Add(serviceDiscoveryBehavior);
        }

        /// <summary>
        ///A test for GetPublishedEndpointsAsync
        ///</summary>
        [TestMethod()]
        public void GetPublishedEndpointsAsyncTest()
        {
            ManualResetEvent waitHandle = new ManualResetEvent(false);
            int expectedNumperOfDiscoveredEndpoints = 3;
            int numperOfDiscoveredEndpoints = -1;
            int _timeout = 10000; //less then this is risk for timeout due to service opened + udp find
            bool isTimeout = false;
            bool expectedTimeout = false;

            ServiceHostBase serviceHost = GetServiceHostWithEndpoints(expectedNumperOfDiscoveredEndpoints);

            // Make the service discoverable
            MakeServiceHostDiscover(serviceHost);

            Action<ReadOnlyCollection<EndpointDiscoveryMetadata>, bool> callback = (result,timeout) =>
            {
                isTimeout = timeout;
                numperOfDiscoveredEndpoints = result.Count;
                
                waitHandle.Set();
            };

            serviceHost.GetPublishedEndpointsAsync(callback, _timeout);
            serviceHost.Open();

            waitHandle.WaitOne(_timeout * 2);
            Assert.AreEqual(expectedTimeout, isTimeout);
            Assert.AreEqual(expectedNumperOfDiscoveredEndpoints, numperOfDiscoveredEndpoints);
        }

        /// <summary>
        ///A test for GetPublishedEndpoints
        ///</summary>
        [TestMethod()]
        public void GetPublishedEndpointswithTimeoutTest()
        {
            int expectedNumperOfDiscoveredEndpoints = 5;
            ServiceHostBase serviceHost = GetServiceHostWithEndpoints(expectedNumperOfDiscoveredEndpoints);
            MakeServiceHostDiscover(serviceHost);
            serviceHost.Open();
            int timeout = 10000; //less then this is risk for timeout due to service opened + udp find
            ReadOnlyCollection<EndpointDiscoveryMetadata> actual;
            actual = serviceHost.GetPublishedEndpoints(timeout);
            Assert.AreEqual(expectedNumperOfDiscoveredEndpoints, actual.Count);
        }

        /// <summary>
        ///A test for GetPublishedEndpoints
        ///</summary>
        [TestMethod()]
        public void GetPublishedEndpointswithoutTimeoutTest()
        {
            int expectedNumperOfDiscoveredEndpoints = 15;
            ServiceHostBase serviceHost = GetServiceHostWithEndpoints(expectedNumperOfDiscoveredEndpoints);
            MakeServiceHostDiscover(serviceHost);
            serviceHost.Open();
            ReadOnlyCollection<EndpointDiscoveryMetadata> actual;
            actual = serviceHost.GetPublishedEndpoints();
            Assert.AreEqual(expectedNumperOfDiscoveredEndpoints, actual.Count);
        }
    }
}
