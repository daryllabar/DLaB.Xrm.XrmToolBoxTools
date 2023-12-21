using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace DLaB.XrmToolBoxCommon.Controls
{
    public partial class SearchablePropertyGrid : UserControl
    {
        private bool _searchActive;
        private const string SearchText = "Search...";

        public object SelectedObject
        {
            get => FilterableGrid.SelectedObject;
            set => FilterableGrid.SelectedObject = value;
        }

        public SearchablePropertyGrid()
        {
            InitializeComponent();

            txtSearch.Text = SearchText;
            txtSearch.ForeColor = System.Drawing.SystemColors.ControlText;
        }


        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!_searchActive)
                return;

            FilterableGrid.FilterProperties = txtSearch.Text;
        }


        private void TxtSearch_LostFocus(object sender, EventArgs e)
        {
            _searchActive = false;
            txtSearch.ForeColor = System.Drawing.SystemColors.GrayText;
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                txtSearch.Text = SearchText;
                FilterableGrid.FilterProperties = null;
            }
        }

        private void TxtSearch_GotFocus(object sender, EventArgs e)
        {
            _searchActive = true;
            txtSearch.ForeColor = System.Drawing.SystemColors.ControlText;
            if (txtSearch.Text == SearchText)
                txtSearch.Text = "";
        }
    }
    public class FilteredPropertyGrid : PropertyGrid
    {
        private string _filterProperties;

        public string FilterProperties
        {
            get => _filterProperties;
            set
            {
                if (_filterProperties == value)
                {
                    return;
                }
                _filterProperties = value;

                Refresh();
            }
        }

        public IEnumerable<GridItem> GetAllGridItems()
        {
            var field = typeof(PropertyGrid).GetField("gridView", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                field = typeof(PropertyGrid).GetField("_gridView", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    throw new NotImplementedException("Unable to get _gridView in collection");
                }
            }

            var view = field.GetValue(this);
            if (view == null)
            {
                yield break;
            }

            var collection = (GridItemCollection)view.GetType().InvokeMember("GetAllGridEntries", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, view, null);

            for (var i = 0; i < collection.Count; i++)
            {
                yield return collection[i];
            }
        }
    }
}
