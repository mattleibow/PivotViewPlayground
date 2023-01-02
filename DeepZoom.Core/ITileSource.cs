namespace DeepZoom.Core;

public interface ITileSource
{
	string ItemId { get; }

	Size ItemSize { get; }

	int MaxLevel { get; }

	Size TileSize { get; }

	IList<DeepZoomTileSourceTile> GetImageTiles(int level);

	double GetScale(int level);

	Size GetScaledItemSize(int level);

	/// <summary>
	/// Get the bounds (in image coordinates) of a paricular image tile.
	/// </summary>
	/// <param name="level">The scale level.</param>
	/// <param name="x">The position along the X axis.</param>
	/// <param name="x">The position along the Y axis.</param>
	/// <returns>Returns the tile bounds in image coordinates.</returns>
	/// <remarks>
	/// The coordinates are in image coordinats relative to the final, full image.
	/// For example, if there is just 1 tile, then the bounds will be [0,0|1,1]. 
	/// But if there are 2 horizontal tiles, each covering a half of the image,
	/// then the bounds will be 0,0=[0,0|.5,1] and 1,0=[.5,0|.5,1]. If the tiles are
	/// partial, then they will result in different bounds 0,0=[0,0|.7,1] and
	/// 1,0=[.7,0|.3,1].
	/// </remarks>
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

	Rectangle GetTiles(int level, RectangleF frame);
}
