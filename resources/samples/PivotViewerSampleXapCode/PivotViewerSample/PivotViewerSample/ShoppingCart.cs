using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Pivot;

namespace PivotViewerSample
{
    /// <summary>
    /// Implement a simple add to cart / remove from cart for the Bing New Cars
    /// collection.
    /// </summary>
    public class ShoppingCart
    {
        /// <summary>
        /// Setup the shopping cart feature
        /// </summary>
        public static void Create(CustomPivotViewer pivotViewer)
        {
            // Don't need to save a reference to the object created here, because the
            // ShoppingCart will be referenced by the PivotViewer to which it is
            // attached
            new ShoppingCart(pivotViewer);
        }

        /// <summary>
        /// Construct an instance of ShoppingCart
        /// </summary>
        private ShoppingCart(CustomPivotViewer pivotViewer)
        {
            if (pivotViewer.GetCustomActionsForItemCallback != null)
            {
                throw new InvalidOperationException(
                    "Shopping cart must be the only handler for a CustomPivotViewer's GetCustomActionsForItemCallback");
            }

            // Redirect calls to our PivotViewer's GetCustomActionsForItem to our own method
            pivotViewer.GetCustomActionsForItemCallback = this.GetCustomActionsForItem;

            m_pivotViewer = pivotViewer;

            // Hook the ItemActionExecuted event handler
            m_pivotViewer.ItemActionExecuted += PivotViewer_ItemActionExecuted;

            // Check for the CartContentsGrid element, and if it exists make it visible. Doing this
            // dynamically allows the application to build & run if the element is removed
            FrameworkElement cartContentsGrid = m_pivotViewer.FindName("CartContentsGrid") as FrameworkElement;
            if (cartContentsGrid != null)
            {
                cartContentsGrid.Visibility = Visibility.Visible;
            }

            // Build the "Add to Cart" custom action
            s_addCustomAction =
                new CustomAction(
                    "Add to Cart",
                    GetIconUri("Add.png"),
                    "Adds the item to the cart",
                    AddItemActionId);

            // Build the "Remove from Cart" custom action
            s_removeCustomAction =
                new CustomAction(
                    "Remove from Cart",
                    GetIconUri("Remove.png"),
                    "Removes the item from the cart",
                    RemoveItemActionId);
        }

        /// <summary>
        /// Handle a custom action execution
        /// </summary>
        private void PivotViewer_ItemActionExecuted(object sender, ItemActionEventArgs args)
        {
            PivotViewer pivotViewer = (PivotViewer)sender;

            // Add or remove the item from the cart, depending the action Id.
            if (args.CustomActionId == AddItemActionId)
            {
                m_itemIdsInCart.Add(args.ItemId);
            }
            else if (args.CustomActionId == RemoveItemActionId)
            {
                m_itemIdsInCart.Remove(args.ItemId);
            }

            // After an action has been executed, the ListBox is stale, so update it
            RefreshCartContentsListBox();
        }

        /// <summary>
        /// Refresh the items in the cart listbox when an item has been added/removed
        /// </summary>
        private void RefreshCartContentsListBox()
        {
            // Check for the CartContents list box, and if it exists update it. Doing this dynamically allows
            // the application to build & run if the element is removed
            ListBox cartContents = m_pivotViewer.FindName("CartContents") as ListBox;
            if (cartContents != null)
            {
                // Refresh cart contents
                List<string> itemNames = new List<string>();
                foreach (string itemId in m_itemIdsInCart)
                {
                    PivotItem item = m_pivotViewer.GetItem(itemId);
                    itemNames.Add(item.Name);
                }
                itemNames.Sort();
                cartContents.ItemsSource = itemNames;
            }
        }

        /// <summary>
        /// Given an itemId, build a list of custom actions to display for that item
        /// </summary>
        private List<CustomAction> GetCustomActionsForItem(string itemId)
        {
            // Start with an empty list
            List<CustomAction> customActions = new List<CustomAction>();

            // Only show custom buttons for the New Cars collection. To support this across all collections, it would
            // be necessary to maintain separate HashSets holding the items in the cart for each collection (since itemIds
            // might overlap across collections).
            if (m_pivotViewer.CollectionUri.ToString().IndexOf("/bingnewcars.cxml", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                if (!m_itemIdsInCart.Contains(itemId))
                {
                    // Item is not yet in the cart, display an "Add to Cart" action
                    customActions.Add(s_addCustomAction);
                }
                else
                {
                    // Item is already in the cart, display a "Remove from Cart" action
                    customActions.Add(s_removeCustomAction);
                }
            }

            return customActions;
        }

        /// <summary>
        /// Using a relative path to an icon, and the current web page address as a base, build an absolute Uri to the icon
        /// </summary>
        private Uri GetIconUri(string iconPath)
        {
            // Reference the icon from this application's resources
            Uri iconUri = new Uri(string.Format("/PivotViewerSample;component/Resources/{0}", iconPath), UriKind.Relative);

            return iconUri;
        }

        /// <summary>
        /// Set of item Ids that are in the shopping cart
        /// </summary>
        public HashSet<string> m_itemIdsInCart = new HashSet<string>();

        private const string AddItemActionId = "addItem";

        private const string RemoveItemActionId = "removeItem";

        private static CustomAction s_addCustomAction;

        private static CustomAction s_removeCustomAction;

        private PivotViewer m_pivotViewer;
    }
}
