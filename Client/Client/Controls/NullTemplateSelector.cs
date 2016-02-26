using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class NullTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NullTemplate { get; set; }
        public DataTemplate NotNullTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item == null)
            {
                return NullTemplate;
            }
            return NotNullTemplate;
        }
    }
}
