namespace DeepZoom.Core.Tests;

public class TileLoaderUnitTest
{
	[Fact]
	public void TileThatWasNotRequestedIsNotFound()
	{
		var cache = new TileCache<ITileLoadingInfo>(10);
		var imageLoader = new TestImageLoader();

		var loader = new TileLoader(imageLoader, cache, 1);

		var info = loader.GetTileInfo(new Tile { Uri = "A" });
		Assert.Null(info);
		Assert.Equal(0, imageLoader.LoadCount);
	}

	[Fact]
	public async Task TileLoadedEventIsInvoked()
	{
		var cache = new TileCache<ITileLoadingInfo>(10);
		var imageLoader = new TestImageLoader();
		var loader = new TestWaitingTileLoader(imageLoader, cache, 1);

		Assert.True(loader.Request(new Tile { Uri = "A" }));

		await loader.Wait();

		Assert.Equal(1, loader.LoadCount);
		Assert.Equal(1, imageLoader.LoadCount);
		Assert.Equal(1, cache.Count);
	}

	[Fact]
	public async Task RequestingTheSameTileSkipsRequest()
	{
		var cache = new TileCache<ITileLoadingInfo>(10);
		var imageLoader = new TestImageLoader();
		var loader = new TestWaitingTileLoader(imageLoader, cache, 1);

		var tile = new Tile { Uri = "A" };
		Assert.True(loader.Request(tile));
		await loader.Wait();

		Assert.False(loader.Request(tile));

		Assert.Equal(1, loader.LoadCount);
		Assert.Equal(1, imageLoader.LoadCount);
		Assert.Equal(1, cache.Count);
	}

	[Fact]
	public async Task RequestingTheSameTileAppendsRequestWhenLoading()
	{
		var cache = new TileCache<ITileLoadingInfo>(10);
		var imageLoader = new TestWaitingImageLoader();
		var loader = new TestWaitingTileLoader(imageLoader, cache, 1);

		var tile = new Tile { Uri = "A" };
		Assert.True(loader.Request(tile));

		// wait until the request is queued
		imageLoader.WaitEvent.WaitOne();

		Assert.True(loader.Request(tile));

		// let the loader continue
		imageLoader.ContinueEvent.Set();

		await loader.Wait();

		Assert.Equal(2, loader.LoadCount);
		Assert.Equal(1, imageLoader.LoadCount);
		Assert.Equal(1, cache.Count);
	}

	[Fact]
	public async Task RequestingTheSameUriSkipsRequest()
	{
		var cache = new TileCache<ITileLoadingInfo>(10);
		var imageLoader = new TestImageLoader();
		var loader = new TestWaitingTileLoader(imageLoader, cache, 1);

		Assert.True(loader.Request(new Tile { Uri = "A" }));
		await loader.Wait();

		Assert.False(loader.Request(new Tile { Uri = "A" }));

		Assert.Equal(1, loader.LoadCount);
		Assert.Equal(1, imageLoader.LoadCount);
		Assert.Equal(1, cache.Count);
	}

	[Fact]
	public async Task TileThatIsRequestedHasCorrectState()
	{
		var cache = new TileCache<ITileLoadingInfo>(10);
		var imageLoader = new TestWaitingImageLoader();
		var loader = new TestWaitingTileLoader(imageLoader, cache, 1);

		var tile = new Tile { Uri = "A" };
		loader.Request(tile);

		// wait until the request is queued
		imageLoader.WaitEvent.WaitOne();

		// make sure on a different thead
		Assert.NotEqual(Environment.CurrentManagedThreadId, imageLoader.CurrentThreadId);

		var info = loader.GetTileInfo(tile);
		Assert.NotNull(info);
		Assert.Null(info.Image);
		Assert.Equal(TileLoadState.Loading, info.State);

		// let the loader continue
		imageLoader.ContinueEvent.Set();

		await loader.Wait();

		info = loader.GetTileInfo(tile);
		Assert.NotNull(info);
		Assert.Equal("A", info.Image);
		Assert.Equal(TileLoadState.Loaded, info.State);
	}

	[Fact]
	public async Task ItemsPushedFromCacheAreUnloaded()
	{
		var cache = new TileCache<ITileLoadingInfo>(1);
		var imageLoader = new TestImageLoader();
		var loader = new TestWaitingTileLoader(imageLoader, cache, 1);

		var tileA = new Tile { Uri = "A" };
		Assert.True(loader.Request(tileA));
		await loader.Wait();

		loader.Reset();
		var tileB = new Tile { Uri = "B" };
		Assert.True(loader.Request(tileB));
		await loader.Wait();

		Assert.Equal(2, loader.LoadCount);
		Assert.Equal(2, imageLoader.LoadCount);
		Assert.Equal(1, cache.Count);

		var infoA = loader.GetTileInfo(tileA);
		Assert.Null(infoA);

		var infoB = loader.GetTileInfo(tileB);
		Assert.NotNull(infoB);
		Assert.Equal(TileLoadState.Loaded, infoB.State);
	}

	[Fact]
	public async Task MultipleItemsCanBeStoredInCache()
	{
		var cache = new TileCache<ITileLoadingInfo>(2);
		var imageLoader = new TestImageLoader();
		var loader = new TestWaitingTileLoader(imageLoader, cache, 1);

		var tileA = new Tile { Uri = "A" };
		Assert.True(loader.Request(tileA));
		await loader.Wait();

		loader.Reset();
		var tileB = new Tile { Uri = "B" };
		Assert.True(loader.Request(tileB));
		await loader.Wait();

		Assert.Equal(2, loader.LoadCount);
		Assert.Equal(2, imageLoader.LoadCount);
		Assert.Equal(2, cache.Count);

		var infoA = loader.GetTileInfo(tileA);
		Assert.NotNull(infoA);
		Assert.Equal(TileLoadState.Loaded, infoA.State);

		var infoB = loader.GetTileInfo(tileB);
		Assert.NotNull(infoB);
		Assert.Equal(TileLoadState.Loaded, infoB.State);
	}

	[Fact]
	public async Task LoadingMoreTilesRemovesFromCache()
	{
		var cache = new TileCache<ITileLoadingInfo>(2);
		var imageLoader = new TestImageLoader();
		var loader = new TestWaitingTileLoader(imageLoader, cache, 1);

		var tileA = new Tile { Uri = "A" };
		Assert.True(loader.Request(tileA));
		await loader.Wait();

		loader.Reset();
		var tileB = new Tile { Uri = "B" };
		Assert.True(loader.Request(tileB));
		await loader.Wait();

		loader.Reset();
		var tileC = new Tile { Uri = "C" };
		Assert.True(loader.Request(tileC));
		await loader.Wait();

		Assert.Equal(3, loader.LoadCount);
		Assert.Equal(3, imageLoader.LoadCount);
		Assert.Equal(2, cache.Count);

		var infoA = loader.GetTileInfo(tileA);
		Assert.Null(infoA);

		var infoB = loader.GetTileInfo(tileB);
		Assert.NotNull(infoB);
		Assert.Equal(TileLoadState.Loaded, infoB.State);

		var infoC = loader.GetTileInfo(tileC);
		Assert.NotNull(infoC);
		Assert.Equal(TileLoadState.Loaded, infoC.State);
	}

	[Fact]
	public async Task RequestingInfoAndThenLoadingMoreTilesPreventsLossFromCache()
	{
		var cache = new TileCache<ITileLoadingInfo>(2);
		var imageLoader = new TestImageLoader();
		var loader = new TestWaitingTileLoader(imageLoader, cache, 1);

		var tileA = new Tile { Uri = "A" };
		Assert.True(loader.Request(tileA));
		await loader.Wait();

		loader.Reset();
		var tileB = new Tile { Uri = "B" };
		Assert.True(loader.Request(tileB));
		await loader.Wait();

		var infoA = loader.GetTileInfo(tileA);
		Assert.NotNull(infoA);
		Assert.Equal(TileLoadState.Loaded, infoA.State);

		loader.Reset();
		var tileC = new Tile { Uri = "C" };
		Assert.True(loader.Request(tileC));
		await loader.Wait();

		Assert.Equal(3, loader.LoadCount);
		Assert.Equal(3, imageLoader.LoadCount);
		Assert.Equal(2, cache.Count);

		infoA = loader.GetTileInfo(tileA);
		Assert.NotNull(infoA);
		Assert.Equal(TileLoadState.Loaded, infoA.State);

		var infoB = loader.GetTileInfo(tileB);
		Assert.Null(infoB);

		var infoC = loader.GetTileInfo(tileC);
		Assert.NotNull(infoC);
		Assert.Equal(TileLoadState.Loaded, infoC.State);
	}

	class TestImageLoader : ITileImageFileFetcher
	{
		public int LoadCount { get; set; }

		public Task<object?> FetchAsync(string uri, CancellationToken cancellationToken = default)
		{
			LoadCount++;

			return Task.FromResult<object?>(uri);
		}
	}

	class TestWaitingImageLoader : ITileImageFileFetcher
	{
		public int LoadCount { get; set; }

		public AutoResetEvent WaitEvent { get; } = new(false);

		public AutoResetEvent ContinueEvent { get; } = new(false);

		public int CurrentThreadId { get; private set; }

		public Task<object?> FetchAsync(string uri, CancellationToken cancellationToken = default)
		{
			LoadCount++;
			CurrentThreadId = Environment.CurrentManagedThreadId;

			// let everyone know the request is queued
			WaitEvent.Set();

			// wait until we get the go-ahead to continue
			ContinueEvent.WaitOne();

			return Task.FromResult<object?>(uri);
		}
	}

	class TestWaitingTileLoader : TileLoader
	{
		private TaskCompletionSource tcs = new();

		public TestWaitingTileLoader(ITileImageFileFetcher imageLoader, ITileCache<ITileLoadingInfo> tileCache, int concurrentRequests)
			: base(imageLoader, tileCache, concurrentRequests)
		{
			TileLoaded += OnTileLoaded;
		}

		public int LoadCount { get; set; }

		public Task Wait() =>
			tcs.Task;

		public void Reset() =>
			tcs = new();

		private void OnTileLoaded(object? sender, EventArgs e)
		{
			LoadCount++;
			tcs.SetResult();
		}
	}
}
