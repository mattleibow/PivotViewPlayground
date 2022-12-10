namespace PivotView.Core.Tests;

class NewPivotDataSource : List<string>
{
    public NewPivotDataSource(IEnumerable<string> collection)
        : base(collection)
    {
    }
}
