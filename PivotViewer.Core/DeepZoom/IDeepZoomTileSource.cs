namespace PivotViewer.Core.DeepZoom
{
	public interface IDeepZoomTileSource
	{
		string ItemId { get; }

		Size ItemSize { get; }

		int MaxLevel { get; }

		int TileSize { get; }

		IList<DeepZoomImageTile> GetImageTiles(int level);

		double GetScale(int level);

		RectangleF GetTileBounds(int level, int x, int y);

		/// <summary>
		/// Get the number of image tiles for a particular scale level.
		/// </summary>
		/// <param name="level">The scale level.</param>
		/// <returns>Returns the number of tiles for a particular scale level.</returns>
		Size GetTileCount(int level);

		/// <summary>
		/// Get the number of items on a particular image tile.
		/// </summary>
		/// <param name="level">The scale level.</param>
		/// <returns>Returns the number of items on a particular image tile.</returns>
		Size GetItemCount(int level);
	}
}
