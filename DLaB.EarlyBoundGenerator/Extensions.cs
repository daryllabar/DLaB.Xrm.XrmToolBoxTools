using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace DLaB.EarlyBoundGenerator
{
    public static class Extensions
    {
        public static IEnumerable<GridItem> GetAllGridItems(this PropertyGrid grid)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            var field = grid.GetType().GetField("gridView", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                field = grid.GetType().GetField("_gridView", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    throw new NotImplementedException("Unable to get _gridView in collection");
                }
            }

            var view = field.GetValue(grid);
            if (view == null)
            {
                yield break;
            }

            var collection = (GridItemCollection)view.GetType().InvokeMember("GetAllGridEntries", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, view, null);
            
            for (var i =0; i <collection.Count; i++)
            {
                yield return collection[i];
            }
        }
    }
}
