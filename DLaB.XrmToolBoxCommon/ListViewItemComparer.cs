using System;
using System.Collections;
using System.Windows.Forms;

namespace DLaB.XrmToolBoxCommon
{
    /// <summary>
    /// Compares two list view items for sorting
    /// </summary>
    public class ListViewItemComparer : IComparer
    {
        #region Variables

        /// <summary>
        /// Index of sorting column
        /// </summary>
        private readonly int col;

        /// <summary>
        /// Sort order
        /// </summary>
        private readonly SortOrder innerOrder;

        #endregion Variables

        #region Constructors

        /// <summary>
        /// Initializes a new instance of class ListViewItemComparer
        /// </summary>
        public ListViewItemComparer()
        {
            col = 0;
            innerOrder = SortOrder.Ascending;
        }

        /// <summary>
        /// Initializes a new instance of class ListViewItemComparer
        /// </summary>
        /// <param name="column">Index of sorting column</param>
        /// <param name="order">Sort order</param>
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            innerOrder = order;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Compare tow objects
        /// </summary>
        /// <param name="x">object 1</param>
        /// <param name="y">object 2</param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            return Compare((ListViewItem)x, (ListViewItem)y);
        }

        /// <summary>
        /// Compare tow listview items
        /// </summary>
        /// <param name="x">Listview item 1</param>
        /// <param name="y">Listview item 2</param>
        /// <returns></returns>
        public int Compare(ListViewItem x, ListViewItem y)
        {
            if (innerOrder == SortOrder.Ascending)
            {
                return string.CompareOrdinal(x.SubItems[col].Text.ToUpper(), y.SubItems[col].Text.ToUpper());
            }
            else
            {
                return string.CompareOrdinal(y.SubItems[col].Text.ToUpper(), x.SubItems[col].Text.ToUpper());
            }
        }

        #endregion Methods
    }
}