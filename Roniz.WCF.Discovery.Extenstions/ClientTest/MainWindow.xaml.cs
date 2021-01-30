using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace Roniz.WCF.Discovery.Extenstions.ClientTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members
        private AnnouncementService announcementService;
        private ServiceHost announcementServiceHost;

        #endregion

        #region Constructores
        public MainWindow()
        {
            InitializeComponent();

            announcementService = new AnnouncementService();

            // Subscribe to the announcement events
            announcementService.OnlineAnnouncementReceived += OnAnnouncementOnlineEvent;
            announcementService.OfflineAnnouncementReceived += OnAnnouncementOfflineEvent;
        }

        #endregion

        #region Methods

        #region Static Methods
		static string ToStringEndpointDiscoveryMetadata(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            var sb = new StringBuilder();

            sb.Append("\nContractTypeNames");
            foreach (XmlQualifiedName contractTypeName in endpointDiscoveryMetadata.ContractTypeNames)
            {
                sb.AppendFormat("\n\tContractTypeName: {0}", contractTypeName);
            }

            sb.AppendFormat("\nAddress: {0}", endpointDiscoveryMetadata.Address);

            sb.Append("\nListenUris:");
            foreach (Uri listenUri in endpointDiscoveryMetadata.ListenUris)
            {
                sb.AppendFormat("\n\tListenUri: {0}", listenUri);
            }

            sb.Append("\nScopes");
            foreach (Uri scope in endpointDiscoveryMetadata.Scopes)
            {
                sb.AppendFormat("\n\tScope: {0}", scope);
            }

            sb.Append("\nExtensions");
            foreach (var extension in endpointDiscoveryMetadata.Extensions)
            {
                sb.AppendFormat("\n\tExtension: {0}", extension);
            }
            return sb.ToString();
        } 
	#endregion

        private void Log(string subject, string message)
        {
            richTextBoxLog.AppendText("\n" + subject);
            richTextBoxLog.AppendText(message);
            richTextBoxLog.AppendText("\n------------------------------------------------------------");
        }

        private void OnAnnouncementOfflineEvent(object sender, AnnouncementEventArgs e)
        {
            Log("OnAnnouncementOfflineEvent", ToStringEndpointDiscoveryMetadata(e.EndpointDiscoveryMetadata));
        }

        private void OnAnnouncementOnlineEvent(object sender, AnnouncementEventArgs e)
        {
            Log("OnAnnouncementOnlineEvent", ToStringEndpointDiscoveryMetadata(e.EndpointDiscoveryMetadata));
        }

        private bool ServiceIsOpened()
        {
            return announcementServiceHost != null && announcementServiceHost.State == CommunicationState.Opened;
        }

        #region Commands Methods
        private void OnClose(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnCanClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !ServiceIsOpened();
        }

        private void OpenServiceExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                announcementServiceHost = new ServiceHost(announcementService);

                // Listen for the announcements sent over UDP multicast
                announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());

                announcementServiceHost.Open();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "error opening service");
            }
        }

        private void OpenServiceCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !ServiceIsOpened();
        }

        private void CloseServiceExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            {
                try
                {
                    announcementServiceHost.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(this, exception.Message, "error closing service , service will abort");
                }
                finally
                {
                    if (announcementServiceHost.State != CommunicationState.Closed)
                        announcementServiceHost.Abort();
                }
            }
        }

        private void CloseServiceCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ServiceIsOpened();
        }
        #endregion

        #endregion
    }
}
