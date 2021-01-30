using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Xml.Linq;
using Roniz.WCF.Discovery.Extenstions.Behaviors.Address;

namespace ServiceMonitor.MainApplication.Converters
{
    /// <summary>
    /// extract the ip endpoint from the extension of the EndpointDiscoveryMetadata
    /// </summary>
    public sealed class GlobalAddressConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as Collection<XElement>;
            if (collection == null)
                return null;

            var xElement = collection.FirstOrDefault(x => x.Name == "Address");
            return AddressExtensionProvider.GetGlobalAddressIpv4(xElement);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
