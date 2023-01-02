using System.Globalization;

namespace PivotVisualizerApp;

public class PivotDataItemConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not PivotDataItem pdi || parameter is not string property)
			return value;

		return pdi.Properties[property].FirstOrDefault();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
		throw new NotSupportedException();
}
