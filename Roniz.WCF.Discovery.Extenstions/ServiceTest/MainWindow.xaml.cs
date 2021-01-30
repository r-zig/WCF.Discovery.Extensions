using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Windows;
using System.Windows.Input;
using Roniz.Diagnostics.Logging;
using Roniz.WCF.Discovery.Extenstions.Endpoints;
using Roniz.WCF.Discovery.Extenstions.WcfService;
using System.Threading.Tasks;

namespace Roniz.WCF.Discovery.Extenstions.ServiceTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members

        private ServiceHost serviceHostCode;
        private ServiceHost serviceHostConfig;
        #endregion

        #region Constructores
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods

        private bool ServiceIsOpened(ServiceHost serviceHost)
        {
            return serviceHost != null && serviceHost.State == CommunicationState.Opened;
        }

        #region Commands Methods
        private void OnClose(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnCanClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !ServiceIsOpened(serviceHostCode) && !ServiceIsOpened(serviceHostConfig);
        }

        private void OpenServiceExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                serviceHostCode = new ServiceHost(typeof(Service1));

                var serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
                
                AnnouncementEndpoint announcementEndpoint;
                DiscoveryEndpoint discoveryEndpoint; 
                if(UDPDiscoveryMenuItem.IsChecked)
                {
                    announcementEndpoint = new UdpAnnouncementEndpoint();
                    discoveryEndpoint = new UdpDiscoveryEndpoint();
                }
                else if (P2PDiscoveryMenuItem.IsChecked)
                {
                    announcementEndpoint = new P2PAnnouncementEndpoint();
                    discoveryEndpoint = new P2PDiscoveryEndpoint();
                }
                else
                {
                    MessageBox.Show("Must use or P2P or UDP discovery mode");
                    return;
                }

                // Announce the availability of the service
                if (UseManagedDiscoveryMenuItem.IsChecked)
                {
                    serviceDiscoveryBehavior.AnnouncementEndpoints.Add(announcementEndpoint);
                    LogManager.GetCurrentClassLogger().Debug("service opening using announcement {0}", announcementEndpoint);
                }

                // Make the service discoverable
                serviceHostCode.Description.Behaviors.Add(serviceDiscoveryBehavior);

                if (UseAdHocDiscoveryMenuItem.IsChecked)
                {
                    serviceHostCode.AddServiceEndpoint(discoveryEndpoint);
                    LogManager.GetCurrentClassLogger().Debug("service opening using ad-hoc discovery {0}", discoveryEndpoint);
                }

                Task.Factory.StartNew(()=>
                serviceHostCode.BeginOpen(result =>
                                          {
                                              try
                                              {
                                                  serviceHostCode.EndOpen(result);
                                                  LogManager.GetCurrentClassLogger().Debug("service opened");
                                              }
                                              catch (Exception exception)
                                              {
                                                  LogManager.GetCurrentClassLogger().Error(exception);
                                              }
                                          }, null)).HandleException();
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Error(exception);
            }
        }

        private void OpenServiceCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = serviceHostCode == null || (serviceHostCode.State != CommunicationState.Opened && serviceHostCode.State != CommunicationState.Opening);
        }

        private void CloseServiceExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            {
                try
                {
                    serviceHostCode.Close();
                    LogManager.GetCurrentClassLogger().Debug("service closed");
                }
                catch (Exception exception)
                {
                    LogManager.GetCurrentClassLogger().Error(exception);
                }
                finally
                {
                    if (serviceHostCode.State != CommunicationState.Closed)
                    {
                        serviceHostCode.Abort();
                        LogManager.GetCurrentClassLogger().Debug("service aborted");
                    }
                }
            }
        }

        private void CloseServiceCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ServiceIsOpened(serviceHostCode);
        }

        private void OpenServiceConfigExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                serviceHostConfig = new ServiceHost(typeof(Service2));

                Task.Factory.StartNew(() =>
                serviceHostConfig.BeginOpen(result =>
                {
                    try
                    {
                        serviceHostConfig.EndOpen(result);
                        LogManager.GetCurrentClassLogger().Debug("service from configuration opened");
                    }
                    catch (Exception exception)
                    {
                        LogManager.GetCurrentClassLogger().Error(exception);
                    }
                }, null)).HandleException();
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Error(exception);
            }
        }

        private void OpenServiceConfigCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = serviceHostConfig == null || (serviceHostConfig.State != CommunicationState.Opened && serviceHostConfig.State != CommunicationState.Opening);
        }

        private void CloseServiceConfigExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            {
                try
                {
                    serviceHostConfig.Close();
                    LogManager.GetCurrentClassLogger().Debug("service (from configuration) closed");
                }
                catch (Exception exception)
                {
                    LogManager.GetCurrentClassLogger().Error(exception);
                }
                finally
                {
                    if (serviceHostConfig.State != CommunicationState.Closed)
                    {
                        serviceHostConfig.Abort();
                        LogManager.GetCurrentClassLogger().Debug("service (from configuration) aborted");
                    }
                }
            }
        }

        private void CloseServiceConfigCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ServiceIsOpened(serviceHostConfig);
        } 
        #endregion

        #region discovery menu options
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
