using System.Diagnostics;

namespace PivotViewer.Core.DeepZoom;

public class TileLoader
{
	private readonly Dictionary<string, LoadingTile> requests = new();
	private readonly Dictionary<string, LoadingTile> infos = new();
	private readonly IImageLoader imageLoader;
	private readonly ITileCache<ITileLoadingInfo> tileCache;
	private readonly int concurrentRequests;

	private SynchronizationContext? syncContext;
	private int currentRequests = 0;

	public TileLoader(IImageLoader imageLoader, ITileCache<ITileLoadingInfo> tileCache, int concurrentRequests)
	{
		this.imageLoader = imageLoader;
		this.tileCache = tileCache;
		this.concurrentRequests = concurrentRequests;
	}

	public event EventHandler? TileLoaded;

	public bool Request(Tile tile)
	{
		// check the currently loaded/loading tiles
		if (infos.TryGetValue(tile.Uri, out var info))
		{
			if (info.State != TileLoadState.Loading)
			{
				Debug.WriteLine("Unexpected request for a tile that is already loaded.");
				return false;
			}

			// add the tile to the list if we are still loading
			info.Add(tile);
		}
		else
		{
			// check the requested tiles
			if (requests.TryGetValue(tile.Uri, out var loadingTile))
				loadingTile.Add(tile);
			else
				requests[tile.Uri] = new LoadingTile(tile);

			// something happened, so make sure the requests are running
			ProcessRequests();
		}

		return true;
	}

	public ITileLoadingInfo? GetTileInfo(Tile tile)
	{
		if (!infos.TryGetValue(tile.Uri, out var info))
			return null;

		// if this tile is loaded, bump it up to the top of the cache
		if (info.State == TileLoadState.Loaded)
			tileCache.Refresh(info);

		return info;
	}

	private void ProcessRequests()
	{
		var remainingRequests = concurrentRequests - currentRequests;
		if (remainingRequests == 0)
			return;

		syncContext = SynchronizationContext.Current;

		// TODO: sort by best tile: larger, centered, low res
		var sorted = requests.Keys.ToList();

		for (var i = 0; i < sorted.Count && remainingRequests > 0; i++)
		{
			var request = sorted[i];

			// move from requests to infos as we are now loading
			if (requests.Remove(request, out var loadingTile))
			{
				currentRequests++;
				loadingTile.State = TileLoadState.Loading;
				loadingTile.Requests = 0;

				infos.Add(request, loadingTile);

				// start the actual request
				ThreadPool.QueueUserWorkItem(ProcessRequest, loadingTile, false);
			}
		}
	}

	private async void ProcessRequest(LoadingTile loadingTile)
	{
		object? image;
		try
		{
			image = await imageLoader.LoadAsync(loadingTile.Uri);

			if (image is null)
				throw new InvalidDataException("Tile loaded no image.");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading tile: {loadingTile.Uri}: {ex}");

			loadingTile.State = TileLoadState.Error;
			image = null;
		}

		if (syncContext is not null)
			syncContext.Post(_ => ProcessComplete(loadingTile, image), null);
		else
			ProcessComplete(loadingTile, image);
	}

	private void ProcessComplete(LoadingTile loadingTile, object? image)
	{
		currentRequests--;

		var tiles = loadingTile.Tiles.ToArray();

		loadingTile.Image = image;
		loadingTile.Requests = 0;
		loadingTile.State = TileLoadState.Loaded;
		loadingTile.Tiles.Clear();

		var removed = tileCache.Add(loadingTile);
		if (removed is not null)
			infos.Remove(removed.Uri);

		foreach (var tile in tiles)
		{
			TileLoaded?.Invoke(this, EventArgs.Empty);
		}

		// there is now a space free, so make sure we are still running requests
		ProcessRequests();
	}

	class LoadingTile : ITileLoadingInfo
	{
		public LoadingTile(Tile tile)
		{
			Uri = tile.Uri;
			Tiles.Add(tile);
			Requests = 1;
		}

		public string Uri { get; set; }

		public List<Tile> Tiles { get; } = new();

		public int Requests { get; set; }

		public TileLoadState State { get; set; }

		public object? Image { get; set; }

		public void Add(Tile tile)
		{
			Requests++;
			Tiles.Add(tile);
		}
	}
}
