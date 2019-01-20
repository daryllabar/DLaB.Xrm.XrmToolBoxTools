using System.Collections.Generic;
using System.Windows.Forms;

namespace DLaB.XrmToolBoxCommon.Forms
{
    class Helper
    {
        private static readonly Dictionary<ListView, int> ColumnSortIndexByListView = new Dictionary<ListView, int>();

        public static void SortListView_OnColumnClick(object sender, ColumnClickEventArgs e)
        {
            var lv = (ListView)sender;
            if (!ColumnSortIndexByListView.TryGetValue(lv, out var index))
            {
                ColumnSortIndexByListView.Add(lv, 0);
            }
            
            if (e.Column == index)
            {
                lv.Sorting = lv.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

                lv.ListViewItemSorter = new ListViewItemComparer(e.Column, lv.Sorting);
            }
            else
            {
                ColumnSortIndexByListView[lv] = e.Column;
                lv.ListViewItemSorter = new ListViewItemComparer(e.Column, SortOrder.Ascending);
            }
        }
    }
}
