namespace PivotView.Core.Tests.Animation;

public class TickerUnitTests
{

    public class TestTicker : Ticker
    {
        public void PerformTick(TimeSpan delta) =>
            OnTick(delta);
    }
}
