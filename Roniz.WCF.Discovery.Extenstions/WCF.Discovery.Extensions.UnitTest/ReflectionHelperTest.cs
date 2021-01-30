using Roniz.WCF.Discovery.Extenstions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Roniz.WCF.Discovery.Extenstions.Helpers;

namespace WCF.Discovery.Extensions.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ReflectionHelperTest and is intended
    ///to contain all ReflectionHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReflectionHelperTest
    {


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
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Create that return FindProgressChangedEventArgs
        ///</summary>
        [TestMethod()]
        public void CreateFindProgressChangedEventArgsTest()
        {
            EndpointDiscoveryMetadata metadata = EndpointDiscoveryMetadata.FromServiceEndpoint(new ServiceEndpoint(new ContractDescription("test"), new NetTcpBinding(), new EndpointAddress(EndpointAddress.AnonymousUri)));
            DiscoveryMessageSequence sequence = null;
            int progressPercentage = 0;
            object userState = Tuple.Create<int>(5);
            FindProgressChangedEventArgs actual;
            actual = ReflectionHelper.Create(metadata, sequence, progressPercentage, userState);
            Assert.AreEqual(metadata, actual.EndpointDiscoveryMetadata);
            Assert.AreEqual(sequence, actual.MessageSequence);
            Assert.AreEqual(progressPercentage, actual.ProgressPercentage);
            Assert.AreEqual(userState, actual.UserState);
        }

        /// <summary>
        ///A test for Create that return FindProgressChangedEventArgs with exception
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.Reflection.TargetInvocationException), "An exception occurred during the operation, making the result invalid.  Check InnerException for exception details.")]
        public void CreateFindCompleteEventArgsWithExceptionTest()
        {
            FindResponse findResponse = CreateMockFindResponse();
            Exception error = new Exception("test");
            bool cancelled = true;
            object userState = Tuple.Create<int>(5);
            var actual = ReflectionHelper.Create(findResponse, error, cancelled, userState);
            Assert.AreEqual(cancelled, actual.Cancelled);
            Assert.AreEqual(error, actual.Error);
            Assert.AreEqual(userState, actual.UserState);
            Assert.AreEqual(findResponse, actual.Result);
        }

        /// <summary>
        ///A test for Create that return FindProgressChangedEventArgs without exception but with cancelled operation
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException),"Operation has been cancelled.")]
        public void CreateFindCompleteEventArgsCancelledTest()
        {
            FindResponse findResponse = CreateMockFindResponse();
            Exception error = null;
            bool cancelled = true;
            object userState = Tuple.Create<int>(5);
            var actual = ReflectionHelper.Create(findResponse, error, cancelled, userState);
            Assert.AreEqual(cancelled, actual.Cancelled);
            Assert.AreEqual(error, actual.Error);
            Assert.AreEqual(userState, actual.UserState);
            Assert.AreEqual(findResponse, actual.Result);
        }

        /// <summary>
        ///A test for Create that return FindProgressChangedEventArgs that complete ok
        ///</summary>
        [TestMethod()]
        public void CreateFindCompleteEventArgsOkTest()
        {
            FindResponse findResponse = CreateMockFindResponse();
            Exception error = null;
            bool cancelled = false;
            object userState = Tuple.Create<int>(5);
            var actual = ReflectionHelper.Create(findResponse, error, cancelled, userState);
            Assert.AreEqual(cancelled, actual.Cancelled);
            Assert.AreEqual(error, actual.Error);
            Assert.AreEqual(userState, actual.UserState);
            Assert.AreEqual(findResponse, actual.Result);
        }

        /// <summary>
        ///A test for Create FindResponse without parameters
        ///</summary>
        [TestMethod()]
        public void CreateFindResponseWithEmptyParamsTest()
        {
            FindResponse actual = ReflectionHelper.CreateFindResponse();
            Assert.AreEqual(0, actual.Endpoints.Count);
        }

        /// <summary>
        ///A test for Create FindResponse with empty metadata
        ///</summary>
        [TestMethod()]
        public void CreateFindResponseWithEmptyMetadaTest()
        {
            Dictionary<EndpointDiscoveryMetadata, DiscoveryMessageSequence> endpoints = new Dictionary<EndpointDiscoveryMetadata, DiscoveryMessageSequence>();
            FindResponse actual;
            actual = ReflectionHelper.Create(endpoints);
            Assert.AreEqual(endpoints.Count, actual.Endpoints.Count);
        }
        /// <summary>
        ///A test for Create FindResponse with metadata
        ///</summary>
        [TestMethod()]
        public void CreateFindResponseWithMetaDataTest()
        {
            Dictionary<EndpointDiscoveryMetadata, DiscoveryMessageSequence> endpoints = CreateMockEndpointDiscoveryMetadata();
            FindResponse actual;
            actual = ReflectionHelper.Create(endpoints);
            foreach (var kvp in endpoints)
            {
                DiscoveryMessageSequence expectedMessageSequence = kvp.Value;
                Assert.AreEqual(expectedMessageSequence, actual.GetMessageSequence(kvp.Key));
            }
            var expectedCollection = new Collection<EndpointDiscoveryMetadata>();
            foreach (var item in endpoints.Keys)
	        {
                expectedCollection.Add(item);
	        }

            for (int i = 0; i < endpoints.Count; i++)
            {
                Assert.AreEqual<EndpointDiscoveryMetadata>(expectedCollection[i], actual.Endpoints[i]);
            }
        }

        /// <summary>
        ///A test for Create DiscoveryMessageSequence with arguments
        ///</summary>
        [TestMethod()]
        public void CreateDiscoveryMessageSequenceWithArgsTest()
        {
            Random rand = new Random();
            long instanceId = rand.Next();
            long messageNumber = rand.Next();
            Uri sequenceId = new Uri("urn:Test"); // TODO: Initialize to an appropriate value
            DiscoveryMessageSequence actual, other;
            actual = ReflectionHelper.Create(instanceId, messageNumber, sequenceId);
            other = ReflectionHelper.Create(instanceId, messageNumber, sequenceId);
            Assert.AreEqual(instanceId, actual.InstanceId);
            Assert.AreEqual(messageNumber, actual.MessageNumber);
            Assert.AreEqual(sequenceId, actual.SequenceId);
            Assert.IsTrue(actual.Equals(other));
        }

        /// <summary>
        ///A test for Create DiscoveryMessageSequence without arguments
        ///</summary>
        [TestMethod()]
        public void CreateDiscoveryMessageSequenceWithoutArgsTest()
        {
            DiscoveryMessageSequence actual, other;
            actual = ReflectionHelper.Create();
            other = ReflectionHelper.Create();
            Assert.IsTrue(actual.Equals(other));
        }

        private FindResponse CreateMockFindResponse()
        {
            var inputData = CreateMockEndpointDiscoveryMetadata();
            FindResponse findResponse = ReflectionHelper.Create(inputData);
            return findResponse;
        }

        private Dictionary<EndpointDiscoveryMetadata,DiscoveryMessageSequence> CreateMockEndpointDiscoveryMetadata()
        {
            Dictionary<EndpointDiscoveryMetadata, DiscoveryMessageSequence> dic = new Dictionary<EndpointDiscoveryMetadata, DiscoveryMessageSequence>();
            var ep = EndpointDiscoveryMetadata.FromServiceEndpoint(new ServiceEndpoint(new ContractDescription("test"), new NetTcpBinding(), new EndpointAddress(EndpointAddress.AnonymousUri)));
            DiscoveryMessageSequence sequence = ReflectionHelper.Create(1, 8, new Uri("urn:blabla"));
            dic.Add(ep, sequence);
            return dic;
        }
    }
}
