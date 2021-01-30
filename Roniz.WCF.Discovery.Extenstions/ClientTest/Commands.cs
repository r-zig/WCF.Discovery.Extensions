using System.Windows.Input;

namespace Roniz.WCF.Discovery.Extenstions.ClientTest
{
    public class Commands
    {
        #region Custom Commands

        /// <summary>
        /// Command for Opening service
        /// </summary>
        public static RoutedCommand OpenServiceCommand =
          new RoutedCommand("OpenService", typeof(MainWindow));

        /// <summary>
        /// Command for Closing service
        /// </summary>
        public static RoutedCommand CloseServiceCommand =
          new RoutedCommand("CloseService", typeof(MainWindow));

        #endregion
    }
}
