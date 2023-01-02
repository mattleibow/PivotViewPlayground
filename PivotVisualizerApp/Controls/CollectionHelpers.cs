namespace PivotVisualizerApp.Controls;

internal static class CollectionHelpers
{
	public static void Sync<TSource, TDestination>(
		IEnumerable<TSource> source,
		IList<TDestination> destination,
		Func<TSource, TDestination, bool> comparer,
		Func<TSource, TDestination> creator,
		Action<TSource, TDestination>? syncItem = null)
	{
		var sourceList = source.AsReadOnlyList();

		foreach (var src in sourceList)
		{
			var dest = destination.FirstOrDefault(d => comparer(src, d));

			if (dest is null)
				destination.Add(creator(src));
			else
				syncItem?.Invoke(src, dest);
		}

		for (var i = destination.Count - 1; i >= 0; i--)
		{
			var dest = destination[i];

			if (sourceList.All(s => !comparer(s, dest)))
			{
				destination.RemoveAt(i);
			}
		}
	}

	public static void Sync<T>(
		IEnumerable<T> source,
		IList<T> destination,
		Action<T, T>? syncItem = null)
	{
		var sourceList = source.AsReadOnlyList();

		foreach (var src in sourceList)
		{
			var dest = destination.FirstOrDefault(d => src.Equals(d));

			if (dest is null)
				destination.Add(src);
			else
				syncItem?.Invoke(src, dest);
		}

		for (var i = destination.Count - 1; i >= 0; i--)
		{
			var dest = destination[i];

			if (sourceList.All(s => !s.Equals(dest)))
			{
				destination.RemoveAt(i);
			}
		}
	}

	private static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> enumerable)
	{
		if (enumerable is IReadOnlyList<T> rolist)
			return rolist;

		if (enumerable is IList<T> list)
			return list.AsReadOnly();

		return enumerable.ToList();
	}
}
