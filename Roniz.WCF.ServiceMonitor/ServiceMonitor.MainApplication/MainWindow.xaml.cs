using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Windows;
using System.Windows.Input;
using DrWPF.Windows.Data;
using Roniz.Diagnostics.Logging;
using Roniz.WCF.Discovery.Extenstions.Endpoints;
using System.Threading.Tasks;
using Roniz.WCF.Discovery.Extenstions.Client;

namespace ServiceMonitor.MainApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region members
        private AnnouncementService announcementService;
        private ServiceHost announcementServiceHost;
        private readonly ObservableDictionary<long, EndpointDiscoveryMetadata> announcementEvents;
        private readonly object syncLock = new object();
        #endregion

        #region commands
        public static RoutedCommand StartMonitorCommand = new RoutedCommand();
        public static RoutedCommand StopMonitorCommand = new RoutedCommand();
        public static RoutedCommand CleanMonitorCommand = new RoutedCommand();
        //private DiscoveryClientEx discoveryClient;
        private DiscoveryClient discoveryClient;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            announcementEvents = new ObservableDictionary<long, EndpointDiscoveryMetadata>();
            DataContext = announcementEvents;
        }

        #region methods

        #region command handlers
        private void CleanMonitorCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = announcementEvents != null && announcementEvents.Count > 0;
        }

        private void CleanMonitorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            announcementEvents.Clear();
        }

        private void StartMonitorCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = announcementService == null;
        }

        private void StartMonitorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (UseManagedDiscoveryMenuItem.IsChecked)
                OpenAnnouncementService();
            if (UseAdHocDiscoveryMenuItem.IsChecked)
                FindAlreadyRegisteredServices();
        }
        
        private void StopMonitorCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = announcementServiceHost != null && announcementServiceHost.State == CommunicationState.Opened;
        }

        private void StopMonitorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CloseAnnouncementService();
        }

        #endregion

        private void OpenAnnouncementService()
        {
            announcementService = new AnnouncementService();
            // Add event handlers  
            announcementService.OnlineAnnouncementReceived += OnOnlineAnnouncement;
            announcementService.OfflineAnnouncementReceived += OnOfflineAnnouncement;
            // Create the service host with a singleton  
            announcementServiceHost = new ServiceHost(announcementService);
            // Add the announcement endpoint

            AnnouncementEndpoint announcementEndpoint;
            if (UDPDiscoveryMenuItem.IsChecked)
                announcementEndpoint = new UdpAnnouncementEndpoint();
            else if (P2PDiscoveryMenuItem.IsChecked)
                announcementEndpoint = new P2PAnnouncementEndpoint();
            else
            {
                MessageBox.Show("Must use or P2P or UDP discovery mode");
                return;
            }
            announcementServiceHost.AddServiceEndpoint(announcementEndpoint);

            LogManager.GetCurrentClassLogger().Debug("Opening announcement service using {0}", announcementEndpoint);
            // Open the host asynchronously
            announcementServiceHost.BeginOpen(result =>
                {
                    announcementServiceHost.EndOpen(result);
                    LogManager.GetCurrentClassLogger().Debug("announcement service opened");
                }, null);
        }

        private void CloseAnnouncementService()
        {
            lock (syncLock)
            {
                announcementEvents.Clear();
            }
            LogManager.GetCurrentClassLogger().Debug("Closing announcement service");
            announcementServiceHost.Close();
            announcementService = null;
            LogManager.GetCurrentClassLogger().Debug("Announcement service closed");
        }

        private void FindAlreadyRegisteredServices()
        {
            DiscoveryEndpoint discoveryEndpoint;
            if (UDPDiscoveryMenuItem.IsChecked)
                discoveryEndpoint = new UdpDiscoveryEndpoint();
            else if (P2PDiscoveryMenuItem.IsChecked)
                discoveryEndpoint = new P2PDiscoveryEndpoint(true){IsSystemEndpoint = false};
            else
            {
                MessageBox.Show("Must use or P2P or UDP discovery mode");
                return;
            }

            if (discoveryClient != null)
            {
                discoveryClient.Close();
            }

            //discoveryClient = new DiscoveryClientEx(discoveryEndpoint);
            discoveryClient = new DiscoveryClient(discoveryEndpoint);
            LogManager.GetCurrentClassLogger().Debug("Find already registered services (Ad-Hoc) using {0}", discoveryEndpoint);
            discoveryClient.FindProgressChanged += DiscoveryClientFindProgressChanged;
            discoveryClient.FindCompleted += DiscoveryClientFindCompleted;
            discoveryClient.ResolveCompleted += DiscoveryClientResolveCompleted;
            discoveryClient.ProxyAvailable += DiscoveryClientProxyAvailable;

            discoveryClient.Open();
            Task.Factory.StartNew(()=>
            discoveryClient.FindAsync(new FindCriteria())
            //discoveryClient.ResolveAsync(new ResolveCriteria());
            ).HandleException();
        }

        private void DiscoveryClientProxyAvailable(object sender, AnnouncementEventArgs e)
        {
            LogManager.GetCurrentClassLogger().Debug("Discovery client proxy available {0} {1}", e.MessageSequence.InstanceId, e.EndpointDiscoveryMetadata.Address);
            AddOrUpdateService(e.MessageSequence,e.EndpointDiscoveryMetadata);
        }

        private void DiscoveryClientResolveCompleted(object sender, ResolveCompletedEventArgs e)
        {
            if(e.Error == null)
                LogManager.GetCurrentClassLogger().Debug("Discovery client resolve completed (EndpointDiscoveryMetadata: {0})", e.Result.EndpointDiscoveryMetadata != null ? e.Result.EndpointDiscoveryMetadata.Address.ToString() : "null");
            else
                LogManager.GetCurrentClassLogger().Error("Discovery client resolve completed (Error: {0})", e.Error);
        }

        private void DiscoveryClientFindCompleted(object sender, FindCompletedEventArgs e)
        {
            LogManager.GetCurrentClassLogger().Debug("Discovery find completed {0} endpoints", e.Result.Endpoints.Count);
        }

        private void DiscoveryClientFindProgressChanged(object sender, FindProgressChangedEventArgs e)
        {
            AddOrUpdateService(e.MessageSequence, e.EndpointDiscoveryMetadata);
        }

        private void OnOnlineAnnouncement(object sender, AnnouncementEventArgs e)
        {
            AddOrUpdateService(e.MessageSequence, e.EndpointDiscoveryMetadata);
        }

        private void OnOfflineAnnouncement(object sender, AnnouncementEventArgs e)
        {
            RemoveService(e.MessageSequence.InstanceId);
        }

        private void AddOrUpdateService(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            Dispatcher.BeginInvoke((Action)(() =>
                                                {
                                                    LogManager.GetCurrentClassLogger().Debug("AddOrUpdateService {0} {1}", messageSequence.InstanceId, endpointDiscoveryMetadata);
                                                    lock (syncLock)
                                                    {
                                                        announcementEvents[messageSequence.InstanceId] = endpointDiscoveryMetadata;
                                                    }
                                                }));
        }

        private void RemoveService(long instanceId)
        {
            Dispatcher.BeginInvoke((Action)(() =>
                                                 {
                                                     LogManager.GetCurrentClassLogger().Debug("RemoveService {0}",
                                                                                             instanceId);
                                                     lock (syncLock)
                                                     {
                                                         announcementEvents.Remove(instanceId);
                                                     }
                                                 }));
        }

        #region discovery options
        private void UdpDiscoveryMenuItemChecked(object sender, RoutedEventArgs e)
        {
            if (P2PDiscoveryMenuItem != null)
                P2PDiscoveryMenuItem.IsChecked = false;
        }

        private void UdpDiscoveryMenuItemUnchecked(object sender, RoutedEventArgs e)
        {
            if (P2PDiscoveryMenuItem != null)
                P2PDiscoveryMenuItem.IsChecked = true;
        }

        private void P2PDiscoveryMenuItemChecked(object sender, RoutedEventArgs e)
        {
            if (UDPDiscoveryMenuItem != null)
                UDPDiscoveryMenuItem.IsChecked = false;
        }

        private void P2PDiscoveryMenuItemUnchecked(object sender, RoutedEventArgs e)
        {
            if (UDPDiscoveryMenuItem != null)
                UDPDiscoveryMenuItem.IsChecked = true;
        }
        #endregion
        #endregion
    }
}
