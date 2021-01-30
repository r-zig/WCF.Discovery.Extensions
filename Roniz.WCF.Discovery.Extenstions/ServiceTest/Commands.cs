using System.Windows.Input;

namespace Roniz.WCF.Discovery.Extenstions.ServiceTest
{
    public class Commands
    {
        #region Custom Commands

        /// <summary>
        /// Command for Opening service from code
        /// </summary>
        public static RoutedCommand OpenServiceCommand =
          new RoutedCommand("OpenService", typeof(MainWindow));

        /// <summary>
        /// Command for Closing service from code
        /// </summary>
        public static RoutedCommand CloseServiceCommand =
          new RoutedCommand("CloseService", typeof(MainWindow));

        /// <summary>
        /// Command for Opening service from configuration
        /// </summary>
        public static RoutedCommand OpenServiceConfigCommand =
          new RoutedCommand("OpenServiceConfig", typeof(MainWindow));

        /// <summary>
        /// Command for Closing service from configuration
        /// </summary>
        public static RoutedCommand CloseServiceConfigCommand =
          new RoutedCommand("CloseServiceConfig", typeof(MainWindow));

        #endregion
    }
}
