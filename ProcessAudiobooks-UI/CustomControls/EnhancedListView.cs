using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProcessAudiobooks_UI.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ProcessAudiobooks_UI.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ProcessAudiobooks_UI.CustomControls;assembly=ProcessAudiobooks_UI.CustomControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:EnhancedListView/>
    ///
    /// </summary>
    public partial class EnhancedListView : ListView
    {
        public EnhancedListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnhancedListView), new FrameworkPropertyMetadata(typeof(EnhancedListView)));
            this.Initialized += EnhancedListView_Initialized;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            this.SaveColumnOrder();
        }


        private void EnhancedListView_Initialized(object sender, EventArgs e)
        {
            this.SetColumnOrder();
        }

        private void SetColumnOrder()
		{
			if (!EnhancedListViewSettings.Default.ColumnOrder.ContainsKey(this.Name))
				return;

			List<ColumnOrderItem> columnOrder =
				EnhancedListViewSettings.Default.ColumnOrder[this.Name];

			GridView gv = (GridView)this.View;

			if (columnOrder != null)
			{
                foreach (ColumnOrderItem item in columnOrder)
				{
					gv.Columns[item.ColumnIndex].Width = item.Width;
				}
			}
		}
		//---------------------------------------------------------------------
		private void SaveColumnOrder()
		{
			GridView gv = (GridView)this.View;
			if (gv.AllowsColumnReorder)
			{
				List<ColumnOrderItem> columnOrder = new List<ColumnOrderItem>();
				GridViewColumnCollection columns = gv.Columns;
				for (int i = 0; i < columns.Count; i++)
				{
					columnOrder.Add(new ColumnOrderItem
					{
						ColumnIndex = i,
						Width = columns[i].Width
					});
				}
                EnhancedListViewSettings.Default.ColumnOrder[this.Name] = columnOrder;
				EnhancedListViewSettings.Default.Save();
			}
		}
    }

    internal sealed class EnhancedListViewSettings : global::System.Configuration.ApplicationSettingsBase
    {
        private static EnhancedListViewSettings _defaultInstace =
            (EnhancedListViewSettings)System.Configuration.ApplicationSettingsBase
            .Synchronized(new EnhancedListViewSettings());
        //---------------------------------------------------------------------
        public static EnhancedListViewSettings Default
        {
            get { return _defaultInstace; }
        }
        //---------------------------------------------------------------------
        // Because there can be more than one DGV in the user-application
        // a dictionary is used to save the settings for this DGV.
        // As key the name of the control is used.
        [System.Configuration.UserScopedSetting]
        [System.Configuration.SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        [System.Configuration.DefaultSettingValue("")]
        public Dictionary<string, List<ColumnOrderItem>> ColumnOrder
        {
            get { return this["ColumnOrder"] as Dictionary<string, List<ColumnOrderItem>>; }
            set { this["ColumnOrder"] = value; }
        }
    }

    [Serializable]
    public sealed class ColumnOrderItem
    {
        public double Width { get; set; }
        public int ColumnIndex { get; set; }
    }
}
