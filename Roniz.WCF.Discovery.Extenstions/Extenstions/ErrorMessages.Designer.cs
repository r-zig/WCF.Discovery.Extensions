﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Roniz.WCF.Discovery.Extenstions {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Roniz.WCF.Discovery.Extenstions.ErrorMessages", typeof(ErrorMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot resolve the global ipv4 address for the current machine.\n Possible common reasons: your timeout {0} is too short, PNRP not enabled on your machine or any networking resolver service is not available (check if your networking service is up, and if is accessible) for more information go to www.roniz.net for Networking service troubleshooting.
        /// </summary>
        internal static string GetGlobalAddressIpv4AddressIsNull {
            get {
                return ResourceManager.GetString("GetGlobalAddressIpv4AddressIsNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An timeout of {0} occur when trying to resolve the global ipv4 address for the current machine.\n Possible common reasons: your timeout {0} is too short, PNRP not enabled on your machine or any networking resolver service is not available (check if your networking service is up, and if is accessible) for more information go to www.roniz.net for Networking service troubleshooting.
        /// </summary>
        internal static string GetGlobalAddressIpv4Timeout {
            get {
                return ResourceManager.GetString("GetGlobalAddressIpv4Timeout", resourceCulture);
            }
        }
    }
}