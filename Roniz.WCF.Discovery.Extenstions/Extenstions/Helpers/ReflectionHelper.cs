using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Roniz.WCF.Discovery.Extenstions.Helpers
{
    /// <summary>
    /// Helper class to initiate discovery classes that cannot accesses in regular way
    /// via reflection
    /// </summary>
    static class ReflectionHelper
    {
        /// <summary>
        /// Initiate new DiscoveryMessageSequence instance
        /// </summary>
        /// <returns>instance of DiscoveryMessageSequence</returns>
        /// <remarks>Since DiscoveryMessageSequence does not expose public constructor this method initiate it</remarks>
        internal static DiscoveryMessageSequence Create()
        {
            ConstructorInfo constructorInfoObj = typeof(DiscoveryMessageSequence).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
            DiscoveryMessageSequence sequence = (DiscoveryMessageSequence)constructorInfoObj.Invoke(null);
            return sequence;
        }

        /// <summary>
        /// Initiate new DiscoveryMessageSequence instance
        /// </summary>
        /// <param name="instanceId">the instance ID.</param>
        /// <param name="messageNumber">the message number.</param>
        /// <param name="sequenceId">the sequence ID.</param>
        /// <returns>instance of DiscoveryMessageSequence</returns>
        /// <remarks>Since DiscoveryMessageSequence does not expose public constructor this method initiate it</remarks>
        internal static DiscoveryMessageSequence Create(long instanceId, long messageNumber, Uri sequenceId)
        {
            Type argsType = typeof(DiscoveryMessageSequence);
            Type[] ctorParams = new Type[] { typeof(long), typeof(Uri), typeof(long) };
            var actualParams = new object[] { instanceId, sequenceId,messageNumber };
            return Create<DiscoveryMessageSequence>(actualParams, ctorParams);
        }

        /// <summary>
        /// Initiate new FindResponse instance
        /// </summary>
        /// <returns>instance of FindResponse</returns>
        /// <remarks>Since FindResponse does not expose public constructor this method initiate it</remarks>
        internal static FindResponse CreateFindResponse()
        {
            Dictionary<EndpointDiscoveryMetadata, DiscoveryMessageSequence> endpoints = new Dictionary<EndpointDiscoveryMetadata, DiscoveryMessageSequence>(0);
            return Create(endpoints);
        }

        /// <summary>
        /// Add endpoint & sequence to the given FindResponse instance
        /// </summary>
        /// <param name="findResponse">the instance that should update</param>
        /// <param name="metadata">Gets the EndpointDiscoveryMetadata instance associated with the current ongoing find operation.</param>
        /// <param name="sequence">Gets the DiscoveryMessageSequence instance associated with the current ongoing find operation.</param>
        internal static void AddEndpointMetadata(this FindResponse findResponse, EndpointDiscoveryMetadata metadata, DiscoveryMessageSequence sequence)
        {
            Type type = typeof(FindResponse);
            MethodInfo method = type.GetMethod("AddDiscoveredEndpoint", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] args = new object[] { metadata, sequence };
            method.Invoke(findResponse, args);
        }
        /// <summary>
        /// Initiate new FindResponse instance
        /// </summary>
        /// <param name="findResponse">Gets the FindResponse returned by the find operation.</param>
        /// <param name="error">Gets a value indicating which error occurred during an asynchronous operation.</param>
        /// <param name="cancelled">Gets a value indicating whether an asynchronous operation has been canceled.</param>
        /// <param name="userState">Gets the unique identifier for the asynchronous task.</param>
        /// <returns>instance of FindResponse</returns>
        /// <see cref="FindCompletedEventArgs"/>
        /// <remarks>Since FindResponse does not expose public constructor this method initiate it</remarks>
        internal static FindResponse Create(this Dictionary<EndpointDiscoveryMetadata,DiscoveryMessageSequence> endpoints)
        {
            Type type = typeof(FindResponse);
            ConstructorInfo ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
            FindResponse findResponse = (FindResponse)ctor.Invoke(null);

            if(endpoints == null || endpoints.Count == 0)
                return findResponse;

            MethodInfo method = type.GetMethod("AddDiscoveredEndpoint", BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in endpoints)
	        {
                object[] args = new object[] { item.Key, item.Value };
                method.Invoke(findResponse, args);
            }
            return findResponse;
        }

        /// <summary>
        /// Initiate new FindCompletedEventArgs instance
        /// </summary>
        /// <param name="findResponse">Gets the FindResponse returned by the find operation.</param>
        /// <param name="error">Gets a value indicating which error occurred during an asynchronous operation.</param>
        /// <param name="cancelled">Gets a value indicating whether an asynchronous operation has been canceled.</param>
        /// <param name="userState">Gets the unique identifier for the asynchronous task.</param>
        /// <returns>instance of FindCompletedEventArgs</returns>
        /// <see cref="FindCompletedEventArgs"/>
        /// <remarks>Since FindCompletedEventArgs does not expose public constructor this method initiate it</remarks>
        internal static FindCompletedEventArgs Create(this FindResponse findResponse,Exception error = null,bool cancelled = false, object userState = null)
        {
            Type[] ctorParams = new Type[] { typeof(Exception), typeof(bool), typeof(object), typeof(FindResponse) };
            var actualParams = new object[] { error, cancelled, userState, findResponse };
            return Create<FindCompletedEventArgs>(actualParams, ctorParams);
        }

        /// <summary>
        /// Initiate new FindProgressChangedEventArgs instance
        /// </summary>
        /// <param name="metadata">Gets the EndpointDiscoveryMetadata instance associated with the current ongoing find operation.</param>
        /// <param name="sequence">Gets the DiscoveryMessageSequence instance associated with the current ongoing find operation.</param>
        /// <param name="progressPercentage">Gets the asynchronous task progress percentage.</param>
        /// <param name="userState">Gets the unique identifier for the asynchronous task.</param>
        /// <returns>instance of FindProgressChangedEventArgs</returns>
        /// <see cref="FindCompletedEventArgs"/>
        /// <remarks>Since FindProgressChangedEventArgs does not expose public constructor this method initiate it</remarks>
        internal static FindProgressChangedEventArgs Create(this EndpointDiscoveryMetadata metadata, DiscoveryMessageSequence sequence = null, int progressPercentage = 0, object userState = null)
        {
            Type argsType = typeof(FindProgressChangedEventArgs);
            Type[] ctorParams = new Type[] { typeof(int), typeof(object), typeof(EndpointDiscoveryMetadata), typeof(DiscoveryMessageSequence) };            
            var actualParams = new object[] { progressPercentage, userState, metadata, sequence };
            return Create<FindProgressChangedEventArgs>(actualParams, ctorParams);
        }

        #region private methods

        private static T Create<T>(object[] actualParams, Type[] ctorParams)
        {
            Type argsType = typeof(T);
            ConstructorInfo constructorInfoObj = argsType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, ctorParams, null);
            T eventArgs = (T)constructorInfoObj.Invoke(actualParams);
            return eventArgs;
        }
        #endregion
    }
}
