using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Pivot;

namespace PivotViewerSample
{
    /// <summary>
    /// The travel log feature integrates the browser back/forward buttons with the ViewerState of the PivotViewer
    /// control, so that hitting the "Back" button will take you back to previous ViewerStates (and will also take you
    /// back to the current item, if there was one when you last left that ViewerState). The feature works through two
    /// event handlers, one hooked to the PivotViewer, and one hooked to the browser:
    //
    /// 1) Handle an event from the PivotViewer indicating that the ViewerState has changed by creating unique
    ///    navigation states and mapping them to the ViewerStates (ViewerState changes as a result of applying
    ///    filters, changing the view, changing the sort order, etc)
    /// 2) Handle a navigation state change by finding the mapped ViewerState and applying it
    ///    back to the PivotViewer control
    //
    /// Because changing the ViewerState can cause a navigation state change, and changing the navigation state
    /// can cause a ViewerState change, flags indicating that a navigate is in progress are necessary to prevent
    /// an infinite loop.
    //
    /// Note that this sample travel log implementation does NOT survive the Silverlight app getting unloaded. When the
    /// app is unloaded, the navigation state-to-ViewerState mappings are not persisted to any sort of storage, and so
    /// they are lost. Thus, after that navigating forward or back will not take you to the correct states.
    /// </summary>
    public class TravelLog
    {
        /// <summary>
        /// Create a singleton instance of the TravelLog and connect it to a PivotViewer. Because TravelLog
        /// reads/writes App.Current.Host.NavigationState, there should only be one instance of TravelLog in
        /// an application.
        /// </summary>
        public static void Create(PivotViewer pivotViewer, Uri initialCollectionUri)
        {
            if (s_instance != null)
            {
                throw new InvalidOperationException("TravelLog interacts with the application state, there should only be one instance in an application");
            }

            if (pivotViewer.CollectionUri != null)
            {
                throw new InvalidOperationException("TravelLog should be connected to a PivotViewer before it has loaded a collection");
            }

            s_instance = new TravelLog(pivotViewer, initialCollectionUri);
        }

        /// <summary>
        /// Construct a TravelLog connected to a PivotViewer
        /// </summary>
        private TravelLog(PivotViewer pivotViewer, Uri initialCollectionUri)
        {
            m_pivotViewer = pivotViewer;
            m_travelLog = new Dictionary<string, TravelLogEntry>();
            m_currentTravelLogEntry = new TravelLogEntry(initialCollectionUri, string.Empty);

            // Hook the events that are needed to implement travel log
            m_pivotViewer.PropertyChanged += new PropertyChangedEventHandler(PivotViewer_PropertyChanged);
            App.Current.Host.NavigationStateChanged += OnHostNavigationStateChanged;
        }

        /// <summary>
        /// Handle a change to the navigation state string
        /// </summary>
        private void OnHostNavigationStateChanged(object sender, NavigationStateChangedEventArgs args)
        {
            // Update the travel log using the new and old states
            TravelLogNavigation(args.PreviousNavigationState, args.NewNavigationState);
        }

        /// <summary>
        /// Save state changes for travel log entries
        /// </summary>
        private void PivotViewer_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "ViewerState")
            {
                // A ViewerState change happens in sequence with a lot of other property changes
                // and may even happen twice in response to one user action. To account for this,
                // delay handling so everything has a chance to settle first.
                QueueDelayedViewerStateChangedHandler();
            }
            if (args.PropertyName == "CurrentItemId")
            {
                // Record current and previous values for CurrentItemId, and note
                // that it has been updated during the handling of LoadCollection
                m_lastFocusedItem = m_currentFocusedItem;
                m_currentFocusedItem = m_pivotViewer.CurrentItemId;
                m_hasItemIdBeenUpdatedFromCollectionLoad = true;
            }
        }

        /// <summary>
        /// Handle a change to the ViewerState
        /// </summary>
        private void PivotViewer_ViewerStateChanged()
        {
            // If the new ViewerState is empty, it will be resolved to a non-empty default  ViewerState when the
            // collection is finished loading, and a new travel log entry will get created for that state, so
            // don't create one yet.
            if (!string.IsNullOrEmpty(m_pivotViewer.ViewerState))
            {
                // Don't navigate when m_currentTravelLogEntry has an empty ViewerState, because that happens
                // when loading the first collection, and the ViewerState will soon transition from empty to default
                // when the collection finishes loading - navigating for both ViewerState changes would result in
                // two travel log entries for the same state.
                if (string.IsNullOrEmpty(m_currentTravelLogEntry.ViewerState))
                {
                    m_currentTravelLogEntry.ViewerState = m_pivotViewer.ViewerState;
                }
                else if (!m_isNavigating)
                {
                    // If a navigation isn't already in progress, this ViewerState change is from user input, so
                    // navigate to a new travel log entry for the new state.
                    App.Current.Host.NavigationState = GetUniqueNavigationStateKey();
                }
                else
                {
                    // Otherwise skip setting the navigation state (which would cause another navigation) and an
                    // infinite feedback loop, and record that the navigation is finished
                    m_isNavigating = false;
                }

                if ((m_lastFocusedItem != null)
                    && (m_currentFocusedItem == null)
                    && (m_pivotViewer.CurrentItemId == null))
                {
                    // Items in focus are reset on ViewerState resets
                    m_lastFocusedItem = null;
                    m_currentFocusedItem = null;
                }
            }
        }

        /// <summary>
        /// Handle navigation by using an existing travel log mapping, or creating a new one
        /// </summary>
        private void TravelLogNavigation(string oldNavigationState, string newNavigationState)
        {
            if (newNavigationState.StartsWith("/viewerStateKey/") || string.IsNullOrEmpty(newNavigationState))
            {
                TravelLogEntry newTravelLogEntry = null;

                if (m_travelLog.TryGetValue(newNavigationState, out newTravelLogEntry)
                    && (m_pivotViewer.CollectionUri != newTravelLogEntry.CollectionUri
                        || m_pivotViewer.ViewerState != newTravelLogEntry.ViewerState))
                {
                    // There is a mapping from the navigation state to a travel log entry, and the state in the travel log entry
                    // is different from the current PivotViewer state, so apply the travel log entry state to get the appropriate
                    // forward/back effect

                    m_isNavigating = true;
                    m_hasItemIdBeenUpdatedFromCollectionLoad = false;

                    // Before updating the ViewerState, update the mapping from navigation state to refer to the current ViewerState
                    AddCurrentStateToTravelLog(oldNavigationState);

                    // Load the collection and ViewerState in the travel log entry, and set the targeted focus item which will be focused
                    // once the new ViewerState has settled
                    m_pivotViewer.LoadCollection(newTravelLogEntry.CollectionUri.ToString(), newTravelLogEntry.ViewerState);
                    m_pivotViewer.CurrentItemId = newTravelLogEntry.CurrentItem;

                    // Build up a new current entry for the state that will be loaded
                    m_currentTravelLogEntry = new TravelLogEntry(newTravelLogEntry.CollectionUri, newTravelLogEntry.ViewerState);
                }
                else
                {
                    // There is no mapping, or the state is the same as the existing state (for example, user moved from grid view
                    // to graph view, and back to grid view, then uses the travel log dropdown to back up two entries in one step,
                    // this should result in no state change to PivotViewer, except for possibly a current item change)
                    if (newTravelLogEntry != null)
                    {
                        AddCurrentStateToTravelLog(oldNavigationState, m_pivotViewer.CurrentItemId);
                        m_pivotViewer.CurrentItemId = newTravelLogEntry.CurrentItem;
                    }
                    else
                    {
                        AddCurrentStateToTravelLog(oldNavigationState);
                    }

                    // Build up a new current entry for the existing current state
                    m_currentTravelLogEntry = new TravelLogEntry(m_pivotViewer.CollectionUri, m_pivotViewer.ViewerState);
                }
            }
        }

        /// <summary>
        /// Queue a delegate to run the ViewerState changed handler
        /// </summary>
        private void QueueDelayedViewerStateChangedHandler()
        {
            // Only queue one delegate at a time, it will handle
            // the final ViewerState after things settle, but skip
            // intermediate states
            if (!m_viewerStateChangedHandlerQueued)
            {
                m_viewerStateChangedHandlerQueued = true;
                m_pivotViewer.Dispatcher.BeginInvoke((Action)delegate()
                {
                    PivotViewer_ViewerStateChanged();
                    m_viewerStateChangedHandlerQueued = false;
                });
            }
        }

        /// <summary>
        /// Add current state to the travel log entry associated with a navigation state
        /// </summary>
        private void AddCurrentStateToTravelLog(string navigationStateKey)
        {
            string currentItemId;

            // The current item to be record is either the most recent
            // value of CurrentItemId, or the next most recent, depending
            // on whether the value was updated from the control itself
            // during a navigation
            if (m_hasItemIdBeenUpdatedFromCollectionLoad)
            {
                // CurrentItem has changed since starting the transition to the new state,
                // save the previous value in the travel log entry
                currentItemId = m_lastFocusedItem;
            }
            else
            {
                // CurrentItem has not changed since starting the transition to the new state,
                // save the current value in the travel log entry
                currentItemId = m_currentFocusedItem;
            }

            AddCurrentStateToTravelLog(navigationStateKey, currentItemId);
        }

        /// <summary>
        /// Add current state to the travel log entry associated with a navigation state,
        /// and stores a specific current item
        /// </summary>
        private void AddCurrentStateToTravelLog(string navigationStateKey, string currentItemId)
        {
            m_currentTravelLogEntry.CurrentItem = currentItemId;
            m_travelLog[navigationStateKey] = m_currentTravelLogEntry;
        }

        /// <summary>
        /// Build a unique navigation state string to represent a new travel log entry
        /// </summary>
        private string GetUniqueNavigationStateKey()
        {
            return string.Format("/viewerStateKey/{0}", Guid.NewGuid());
        }

        /// <summary>
        /// The single instance of TravelLog
        /// </summary>
        private static TravelLog s_instance;

        /// <summary>
        /// The PivotViewer connected to this TravelLog
        /// </summary>
        private PivotViewer m_pivotViewer;

        /// <summary>
        /// A mapping from unique navigation state strings to travel log entries, used
        /// for remembering old states when the user navigates forward or backward
        /// </summary>
        private Dictionary<string, TravelLogEntry> m_travelLog;

        /// <summary>
        /// The travel log entry for the current state, which is stored and updated in this
        /// field until a navigation happens, at which time it is persisted into the travel
        /// log map
        /// </summary>
        private TravelLogEntry m_currentTravelLogEntry;

        /// <summary>
        /// The most recently focused item, which may be used in updating
        /// travel log entries
        /// </summary>
        private string m_currentFocusedItem;

        /// <summary>
        /// The next most recently focused item, which is used instead
        /// of the current focused item when the current focused item has
        /// changed during a navigation
        /// </summary>
        private string m_lastFocusedItem;

        /// <summary>
        /// A flag indicating whether a navigation is in progress. Used to
        /// avoid starting a new navigation because of ViewerState changes
        /// that are the result of a current navigation.
        /// </summary>
        private bool m_isNavigating;

        /// <summary>
        /// 
        /// </summary>
        private bool m_hasItemIdBeenUpdatedFromCollectionLoad;

        /// <summary>
        /// Flag indicating whether a handler for ViewerState changes has already been queued
        /// </summary>
        private bool m_viewerStateChangedHandlerQueued;
    }
}
