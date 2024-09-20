using System;
using System.Collections.Generic;
using System.Windows.Pivot;
using System.Windows;

namespace PivotViewerSample
{
    /// <summary>
    /// Override of PivotViewer that uses a delegate for GetCustomActionsForItem
    /// </summary>
    public class CustomPivotViewer : PivotViewer
    {
        /// <summary>
        /// Callback that this viewer will call to get custom actions for a specific item
        /// </summary>
        public Func<string, List<CustomAction>> GetCustomActionsForItemCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Return an add or remove custom action for the item, depending on whether it is in the cart
        /// </summary>
        protected override List<CustomAction> GetCustomActionsForItem(string itemId)
        {
            List<CustomAction> customActions = null;

            if (GetCustomActionsForItemCallback != null)
            {
                customActions = GetCustomActionsForItemCallback(itemId);
            }

            return customActions;
        }
    }
}
