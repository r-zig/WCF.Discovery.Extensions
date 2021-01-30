using System;

namespace Roniz.WCF.Discovery.Extenstions.Endpoints
{
    /// <summary>
    /// Used by the DiscoveryChanged event to expose the new state
    /// </summary>
    public sealed class DiscoveryStateEventArgs : EventArgs
    {
        #region members
        private readonly bool isReady;
        #endregion

        #region constructor
        public DiscoveryStateEventArgs(bool isReady)
        {
            this.isReady = isReady;
        }
        #endregion

        #region properties
        public bool IsReady
        {
            get { return isReady; }
        }
        #endregion
    }
}