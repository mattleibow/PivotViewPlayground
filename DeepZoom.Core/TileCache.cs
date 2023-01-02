namespace DeepZoom.Core;

public class TileCache<T> : ITileCache<T>
{
	private readonly LinkedList<T> tiles = new();

	public TileCache(int capacity)
	{
		if (capacity <= 0)
			throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0.");

		Capacity = capacity;
	}

	public int Capacity { get; }

	public int Count => tiles.Count;

	public T? Add(T tile)
	{
		T? last = default;

		tiles.AddFirst(tile);

		if (tiles.Count > Capacity)
		{
			if (tiles.Last is not null)
				last = tiles.Last.Value;

			tiles.RemoveLast();
		}

		return last;
	}

	public void Remove(T tile)
	{
		tiles.Remove(tile);
	}

	public void Refresh(T tile)
	{
		tiles.Remove(tile);
		tiles.AddFirst(tile);
	}
}
