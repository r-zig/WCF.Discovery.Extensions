//credit http://www.codeproject.com/KB/WPF/XMLViewer.aspx
//see license there also
//TODO later I will go to see all requirements for this license

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;

namespace XMLViewer
{
    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : UserControl
    {
        #region dependency properties

        public static readonly DependencyProperty XElementProperty = DependencyProperty.RegisterAttached("XElement", typeof (XElement),
                                                                                         typeof (Viewer),
                                                                                         new UIPropertyMetadata(null,
                                                                                                                OnXElementChanged));

        private static void OnXElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = d as Viewer;
            if(viewer == null)
                return;

            var xElement = e.NewValue as XElement;
            
            if (xElement != null)
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(xElement.CreateReader());
                viewer.XmlDocument = xmlDocument;
            }
        }

        #endregion

        #region members
        private XmlDocument xmldocument;
        #endregion

        #region constructors
        public Viewer()
        {
            InitializeComponent();
        }
        #endregion

        #region properties

        public XElement XElement
        {
            get
            {
                return (XElement) GetValue(XElementProperty);
            }
            set
            {
                SetValue(XElementProperty,value);
            }
        }

        public XmlDocument XmlDocument
        {
            get { return xmldocument; }
            set
            {
                xmldocument = value;
                BindXmlDocument();
            }
        }
        #endregion

        #region methods
        private void BindXmlDocument()
        {
            if (xmldocument == null)
            {
                xmlTree.ItemsSource = null;
                return;
            }

            XmlDataProvider provider = new XmlDataProvider();
            provider.Document = xmldocument;
            Binding binding = new Binding();
            binding.Source = provider;
            binding.XPath = "child::node()";
            xmlTree.SetBinding(TreeView.ItemsSourceProperty, binding);
        }
        #endregion
    }
}
