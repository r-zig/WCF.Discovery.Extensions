using System.Windows;
using System.Windows.Controls;

namespace ServiceMonitor.MainApplication.TemplateSelectors
{
    public abstract class MultiOrSingleItemTemplateSelector : DataTemplateSelector
    {
        #region properties

        /// <summary>
        /// Template that support single item
        /// </summary>
        public DataTemplate SingleItemTemplate { get; set; }

        /// <summary>
        /// Template that support multiple items
        /// </summary>
        public DataTemplate MultiItemsTemplate { get; set; }
        #endregion

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return null;
            var isMultipleItems = IsMultipleItems(item);
            if (!isMultipleItems.HasValue)
                return null;
            return isMultipleItems.Value ? MultiItemsTemplate : SingleItemTemplate;
        }

        protected abstract bool? IsMultipleItems(object item);
    }
}
