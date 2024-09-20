using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Pivot;

namespace PivotViewerSample
{
    public partial class MainPage : UserControl
    {
        #region Constructors

        /// <summary>
        /// Construct an instance of the MainPage view
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Setup custom strings to replace some of PivotViewer's default strings (comment this line to use all defaults)
            System.Windows.Pivot.PivotViewer.SetResourceManager(SampleLocalizedStrings.ResourceManager);

            // Load the initial collection
            string initialCollectionUri;
            if (!App.Current.Host.InitParams.TryGetValue("initialCollectionUri", out initialCollectionUri))
            {
                // Otherwise use the default startup collection
                initialCollectionUri = s_defaultStartupCollectionUri.ToString();
            }
            PivotViewer.LoadCollection(initialCollectionUri, string.Empty);

            PivotViewer.CollectionLoadingFailed += new EventHandler<CollectionErrorEventArgs>(PivotViewer_CollectionLoadingFailed);

            // Enable the shopping cart (comment this line to disable the shopping cart)
            ShoppingCart.Create(PivotViewer);

            // Enable the travel log (comment this line to disable the travel log)
            TravelLog.Create(PivotViewer, new Uri(initialCollectionUri));
        }

        #endregion

        #region Error handling

        /// <summary>
        /// Basic error reporting when the collection fails to load
        /// </summary>
        private void PivotViewer_CollectionLoadingFailed(object sender, CollectionErrorEventArgs args)
        {
            string errorMessage = args.ErrorCode.ToString();
            string details = string.Format("Error parsing {0}", PivotViewer.CollectionUri);

            ErrorWindow errorWin = new ErrorWindow(errorMessage, details);
            errorWin.Show();
        }

        #endregion

        #region Link handling (metadata link clicked, item double-clicked)

        /// <summary>
        /// Handle links clicked in the metadata pane
        /// </summary>
        private void PivotViewer_LinkClicked(object sender, LinkEventArgs args)
        {
            OpenLink(args.Link.ToString());
        }

        /// <summary>
        /// Handle double-clicks on collection items
        /// </summary>
        private void PivotViewer_ItemDoubleClicked(object sender, ItemEventArgs args)
        {
            string linkUriString = PivotViewer.GetItem(args.ItemId).Href;
            if (!string.IsNullOrWhiteSpace(linkUriString))
            {
                PivotViewer.CurrentItemId = args.ItemId;
                OpenLink(linkUriString);
            }
            else
            {
                ErrorWindow errorWin = new ErrorWindow("No Associated Web Page", "The item that was double-clicked has no value for the 'Href' field");
                errorWin.Show();
            }
        }

        /// <summary>
        /// Open a URI to either a collection or a web page
        /// </summary>
        private void OpenLink(string linkUri)
        {
            if (linkUri.Contains(".cxml"))
            {
                // Link points to a collection file, open it in place
                LoadCollection(linkUri);
            }
            else
            {
                // Link points to a normal web page. Open it in a new tab.
                HtmlPage.Window.Navigate(new Uri(linkUri, UriKind.RelativeOrAbsolute), "PivotViewerSampleTargetFrame");
            }
        }

        /// <summary>
        /// Helper to load a collection from a Uri that may contain a viewerState fragment
        /// </summary>
        private void LoadCollection(string collectionUri)
        {
            string baseCollectionUri;
            string viewerState;
            BreakupCollectionUri(collectionUri, out baseCollectionUri, out viewerState);
            PivotViewer.LoadCollection(baseCollectionUri, viewerState);
        }

        /// <summary>
        /// Break a collection Uri into the base Uri and the viewerState fragment
        /// </summary>
        private void BreakupCollectionUri(string collectionUri, out string baseCollectionUri, out string viewerState)
        {
            string linkUriString = collectionUri;
            string[] linkParts = linkUriString.Split(new char[] { '#' });
            baseCollectionUri = linkParts[0];
            viewerState = (linkParts.Length > 1) ? linkParts[1] : string.Empty;
        }

        #endregion

        #region Pre-defined filters feature

        /// <summary>
        /// Load new cars collection
        /// </summary>
        private void OnCarsCollectionClicked(object sender, RoutedEventArgs args)
        {
            PivotViewer.LoadCollection(s_bingNewCarsCollectionUri.ToString(), string.Empty);
        }

        /// <summary>
        /// Load AMG Top-rated movies collection
        /// </summary>
        private void OnAMGTopMoviesClicked(object sender, RoutedEventArgs args)
        {
            PivotViewer.LoadCollection(s_AMGMoviesCollectionUri.ToString(), string.Empty);
        }

        /// <summary>
        /// Apply pre-defined "Hybrid SUVs" filter to new cars collection
        /// </summary>
        private void OnHybridSUVsClicked(object sender, RoutedEventArgs args)
        {
            PivotViewer.LoadCollection(s_bingNewCarsCollectionUri.ToString(), c_hybridSUVsViewerState);
        }

        /// <summary>
        /// Apply pre-defined "American Cars" filter to new cars collection
        /// </summary>
        private void OnAmericanCarsClicked(object sender, RoutedEventArgs args)
        {
            PivotViewer.LoadCollection(s_bingNewCarsCollectionUri.ToString(), c_americanCarsViewerState);
        }

        #endregion

        #region Private fields

        /// <summary>
        /// Address of collection to load at startup, if none is specified in the InitParams in HTML
        /// </summary>
        private static readonly Uri s_defaultStartupCollectionUri = s_bingNewCarsCollectionUri;

        /// <summary>
        /// Address of AMG Movies collection
        /// </summary>
        private static readonly Uri s_AMGMoviesCollectionUri = new Uri("http://content.getpivot.com/collections/amg/Top_Movies.cxml");

        /// <summary>
        /// Address of Bing New Cars collection
        /// </summary>
        private static readonly Uri s_bingNewCarsCollectionUri = new Uri("http://content.getpivot.com/Collections/bingnewcars/bingnewcars.cxml");

        /// <summary>
        /// Pre-defined viewer state for the Bing New Cars collection that filters to only Hybrid SUVs/Crossoveers
        /// </summary>
        private const string c_hybridSUVsViewerState = "Body%20Style=EQ.SUV%2FCrossover&Fuel=EQ.Hybrid&%24view%24=1&%24facet0%24=Experts'%20Rating";

        /// <summary>
        /// Pre-defined viewer state for the Bing New Cars collection that filters to only American Cars
        /// </summary>
        private const string c_americanCarsViewerState = "Make=EQ.Ford&Make=EQ.Chevrolet&Make=EQ.Dodge&Make=EQ.Hummer&Make=EQ.Buick&Make=EQ.Jeep&Make=EQ.Saturn&Make=EQ.Pontiac&Make=EQ.Lincoln&Make=EQ.Mercury&Make=EQ.Chrysler&Make=EQ.Cadillac&Make=EQ.GMC&%24view%24=1&%24facet0%24=Experts'%20Rating";

        #endregion
    }
}
