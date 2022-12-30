namespace PivotViewer.Core.Tests;

public class TileCacheUnitTests
{
	[Fact]
	public void CanAddItem()
	{
		var cache = new TileCache<Tile>(1);

		var removed = cache.Add(new Tile { Uri = "A" });

		Assert.Null(removed);
		Assert.Equal(1, cache.Count);
	}

	[Fact]
	public void AddingItemRemovesOlderItemWhenAtCapacity1()
	{
		var cache = new TileCache<Tile>(1);
		cache.Add(new Tile { Uri = "A" });

		var removed = cache.Add(new Tile { Uri = "B" });

		Assert.NotNull(removed);
		Assert.Equal(1, cache.Count);
		Assert.Equal("A", removed.Uri);
	}

	[Fact]
	public void AddingItemKeepsOlderItemWhenNotAtCapacity()
	{
		var cache = new TileCache<Tile>(2);
		cache.Add(new Tile { Uri = "A" });

		var removed = cache.Add(new Tile { Uri = "B" });

		Assert.Null(removed);
		Assert.Equal(2, cache.Count);
	}

	[Fact]
	public void AddingItemRemovesOlderItemWhenAtCapacity2()
	{
		var cache = new TileCache<Tile>(2);
		cache.Add(new Tile { Uri = "A" });
		cache.Add(new Tile { Uri = "B" });

		var removed = cache.Add(new Tile { Uri = "C" });

		Assert.NotNull(removed);
		Assert.Equal(2, cache.Count);
		Assert.Equal("A", removed.Uri);
	}

	[Fact]
	public void RefreshingItemPreventsRemoval()
	{
		var cache = new TileCache<Tile>(2);
		var tile = new Tile { Uri = "A" };
		cache.Add(tile);
		cache.Add(new Tile { Uri = "B" });

		cache.Refresh(tile);

		var removed = cache.Add(new Tile { Uri = "C" });

		Assert.NotNull(removed);
		Assert.Equal(2, cache.Count);
		Assert.Equal("B", removed.Uri);
	}

	[Fact]
	public void RemovingItemRemovesFromCache()
	{
		var cache = new TileCache<Tile>(1);
		var tile = new Tile { Uri = "A" };
		cache.Add(tile);

		cache.Remove(tile);

		Assert.Equal(0, cache.Count);
	}
}
