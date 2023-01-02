namespace PivotVisualizerApp.Controls.Themes;

internal class Utils
{
	public static void EnsureResourcesRegistered<T>(ref bool registered)
		where T : ResourceDictionary, new()
	{
		if (registered)
			return;

		var merged = Application.Current?.Resources?.MergedDictionaries;
		if (merged == null)
			return;

		foreach (var dic in merged)
		{
			if (dic.GetType() == typeof(T))
			{
				registered = true;
				break;
			}
		}

		if (!registered)
		{
			merged.Add(new T());
			registered = true;
		}
	}
}
