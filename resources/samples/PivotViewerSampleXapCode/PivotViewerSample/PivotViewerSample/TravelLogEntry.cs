using System;

namespace PivotViewerSample
{
    /// <summary>
    /// Container for a set of properties describing state of the viewer. This is the type of
    /// dictionary values in the unique Uri to collection/viewerState mapping used for the
    /// travel log.
    /// </summary>
    public class TravelLogEntry
    {
        public TravelLogEntry(Uri collection, string viewerState)
        {
            CollectionUri = collection;
            ViewerState = viewerState;
        }

        public string ViewerState { get; set; }
        public Uri CollectionUri { get; set; }
        public string CurrentItem { get; set; }
    }
}
