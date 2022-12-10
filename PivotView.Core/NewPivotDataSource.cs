namespace PivotView.Core;

public class NewPivotDataSource : List<string>
{
    public NewPivotDataSource(IEnumerable<string> collection)
        : base(collection)
    {
    }
}
