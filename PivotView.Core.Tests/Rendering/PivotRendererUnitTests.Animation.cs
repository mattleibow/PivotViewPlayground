namespace PivotView.Core.Tests;

public partial class PivotRendererUnitTests
{
    public class Animation
    {
        [Fact]
        public void PreAnimationHasCorrectValues()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "B", "C", "D" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });
            renderer.Filter = (item) => filterInitial.Contains(item);
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var items = renderer.Items;

            Assert.Equal(new(0, 0, 40, 40), items[0].Frame.Current);
            Assert.Equal(new(0, 40, 40, 40), items[1].Frame.Current);
            Assert.Equal(new(0, 80, 40, 40), items[2].Frame.Current);
            Assert.Equal(new(170, 80, 40, 40), items[3].Frame.Current);

            Assert.Equal(new(170, 0, 40, 40), items[0].Frame.Desired);
            Assert.Equal(new(0, 0, 40, 40), items[1].Frame.Desired);
            Assert.Equal(new(0, 40, 40, 40), items[2].Frame.Desired);
            Assert.Equal(new(0, 80, 40, 40), items[3].Frame.Desired);
        }

        [Fact]
        public void AnimationHasCorrectValues()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "B", "C", "D" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });
            renderer.Filter = (item) => filterInitial.Contains(item);
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var items = renderer.Items;

            var animation = renderer.Animation;

            Assert.NotNull(animation);
            Assert.Equal(5, animation.Count);

            var steps = animation.Steps.ToArray();
            Assert.Equal(5, steps.Length);

            // step 1.1
            {
                var step = Assert.IsType<IncrementalAnimationStep>(steps[0]);
                Assert.Equal(1, step.Count);
                Assert.Equal("Exit", step.Name);

                var stepItems = step.ToArray();
                var item = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[0]);
                Assert.Equal(items[0].Frame, item.Property);
            }

            // step 1.2
            {
                var step = Assert.IsType<InstantaneousAnimationStep>(steps[1]);
                Assert.Equal("Hide", step.Name);
            }

            // step 2.1
            {
                var step = Assert.IsType<IncrementalAnimationStep>(steps[2]);
                Assert.Equal(2, step.Count);
                Assert.Equal("Layout", step.Name);

                var stepItems = step.ToArray();
                var item1 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[0]);
                var item2 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[1]);
                Assert.Equal(items[1].Frame, item1.Property);
                Assert.Equal(items[2].Frame, item2.Property);
            }

            // step 2.2
            {
                var step = Assert.IsType<InstantaneousAnimationStep>(steps[3]);
                Assert.Equal("Add", step.Name);
            }

            // step 3
            {
                var step = Assert.IsType<IncrementalAnimationStep>(steps[4]);
                Assert.Equal(1, step.Count);
                Assert.Equal("Enter", step.Name);

                var stepItems = step.ToArray();
                var item = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[0]);
                Assert.Equal(items[3].Frame, item.Property);
            }
        }

        [Fact]
        public void AnimationStepsCorrectly()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "B", "C", "D" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });
            renderer.Filter = (item) => filterInitial.Contains(item);
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var allItems = renderer.Items;
            var visibleItems = renderer.VisibleItems;

            var animation = renderer.Animation;
            Assert.NotNull(animation);

            // before step
            Assert.False(animation.IsComplete);
            Assert.Equal(3, visibleItems.Count);
            Assert.Equal(allItems[0], visibleItems[0]);
            Assert.Equal(allItems[1], visibleItems[1]);
            Assert.Equal(allItems[2], visibleItems[2]);

            // step 1
            animation.Update(Time.OneSec);

            // after step 1
            Assert.False(animation.IsComplete);
            Assert.Equal(2, visibleItems.Count);
            Assert.Equal(allItems[1], visibleItems[0]);
            Assert.Equal(allItems[2], visibleItems[1]);

            // step 2
            animation.Update(Time.OneSec);

            // after step 2
            Assert.False(animation.IsComplete);
            Assert.Equal(3, visibleItems.Count);
            Assert.Equal(allItems[1], visibleItems[0]);
            Assert.Equal(allItems[2], visibleItems[1]);
            Assert.Equal(allItems[3], visibleItems[2]);

            // step 3
            animation.Update(Time.OneSec);

            // after step 3
            Assert.True(animation.IsComplete);
            Assert.Equal(3, visibleItems.Count);
            Assert.Equal(allItems[1], visibleItems[0]);
            Assert.Equal(allItems[2], visibleItems[1]);
            Assert.Equal(allItems[3], visibleItems[2]);
        }

        [Fact]
        public void AnimationThatDoesNotRemoveOrAddSkipsSteps()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "A", "B", "C" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });
            renderer.Filter = (item) => filterInitial.Contains(item);
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var items = renderer.Items;

            var animation = renderer.Animation;

            Assert.NotNull(animation);
            Assert.Equal(1, animation.Count);

            var steps = animation.Steps.ToArray();
            Assert.Single(steps);

            // step 1 is missing

            // step 2.1
            {
                var step = Assert.IsType<IncrementalAnimationStep>(steps[0]);
                Assert.Equal(3, step.Count);
                Assert.Equal("Layout", step.Name);

                var stepItems = step.ToArray();
                var item1 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[0]);
                var item2 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[1]);
                var item3 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[2]);
                Assert.Equal(items[0].Frame, item1.Property);
                Assert.Equal(items[1].Frame, item2.Property);
                Assert.Equal(items[2].Frame, item3.Property);
            }

            // step 2.2 is missing

            // step 3 is missing
        }

        [Fact]
        public void AnimationThatDoesNotRemoveOrAddStepsCorrectly()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "A", "B", "C" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });
            renderer.Filter = (item) => filterInitial.Contains(item);
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var allItems = renderer.Items;
            var visibleItems = renderer.VisibleItems;

            var animation = renderer.Animation;
            Assert.NotNull(animation);

            // before step
            Assert.False(animation.IsComplete);
            Assert.Equal(3, visibleItems.Count);
            Assert.Equal(allItems[0], visibleItems[0]);
            Assert.Equal(allItems[1], visibleItems[1]);
            Assert.Equal(allItems[2], visibleItems[2]);

            // step
            animation.Update(Time.OneSec);

            // after step
            Assert.True(animation.IsComplete);
            Assert.Equal(3, visibleItems.Count);
            Assert.Equal(allItems[0], visibleItems[0]);
            Assert.Equal(allItems[1], visibleItems[1]);
            Assert.Equal(allItems[2], visibleItems[2]);
        }

        [Fact]
        public void AnimationThatDoesNotRemoveSkipsSteps()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "A", "B", "C", "D" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });
            renderer.Filter = (item) => filterInitial.Contains(item);
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var items = renderer.Items;

            var animation = renderer.Animation;

            Assert.NotNull(animation);
            Assert.Equal(3, animation.Count);

            var steps = animation.Steps.ToArray();
            Assert.Equal(3, steps.Length);

            // step 1 is missing

            // step 2.1
            {
                var step = Assert.IsType<IncrementalAnimationStep>(steps[0]);
                Assert.Equal(3, step.Count);
                Assert.Equal("Layout", step.Name);

                var stepItems = step.ToArray();
                var item1 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[0]);
                var item2 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[1]);
                var item3 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[2]);
                Assert.Equal(items[0].Frame, item1.Property);
                Assert.Equal(items[1].Frame, item2.Property);
                Assert.Equal(items[2].Frame, item3.Property);
            }

            // step 2.2
            {
                var step = Assert.IsType<InstantaneousAnimationStep>(steps[1]);
                Assert.Equal("Add", step.Name);
            }

            // step 3
            {
                var step = Assert.IsType<IncrementalAnimationStep>(steps[2]);
                Assert.Equal(1, step.Count);
                Assert.Equal("Enter", step.Name);

                var stepItems = step.ToArray();
                var item = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[0]);
                Assert.Equal(items[3].Frame, item.Property);
            }
        }

        [Fact]
        public void AnimationThatDoesNotRemoveStepsCorrectly()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "A", "B", "C", "D" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });
            renderer.Filter = (item) => filterInitial.Contains(item);
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var allItems = renderer.Items;
            var visibleItems = renderer.VisibleItems;

            var animation = renderer.Animation;
            Assert.NotNull(animation);

            // before step
            Assert.False(animation.IsComplete);
            Assert.Equal(3, visibleItems.Count);
            Assert.Equal(allItems[0], visibleItems[0]);
            Assert.Equal(allItems[1], visibleItems[1]);
            Assert.Equal(allItems[2], visibleItems[2]);

            // step 1 is missing

            // step 2
            animation.Update(Time.OneSec);

            // after step 2
            Assert.False(animation.IsComplete);
            Assert.Equal(4, visibleItems.Count);
            Assert.Equal(allItems[0], visibleItems[0]);
            Assert.Equal(allItems[1], visibleItems[1]);
            Assert.Equal(allItems[2], visibleItems[2]);
            Assert.Equal(allItems[3], visibleItems[3]);

            // step 3
            animation.Update(Time.OneSec);

            // after step 3
            Assert.True(animation.IsComplete);
            Assert.Equal(4, visibleItems.Count);
            Assert.Equal(allItems[0], visibleItems[0]);
            Assert.Equal(allItems[1], visibleItems[1]);
            Assert.Equal(allItems[2], visibleItems[2]);
            Assert.Equal(allItems[3], visibleItems[3]);
        }

        [Fact]
        public void AnimationThatDoesNotAddSkipsSteps()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "A", "B" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });
            renderer.Filter = (item) => filterInitial.Contains(item);
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var items = renderer.Items;

            var animation = renderer.Animation;

            Assert.NotNull(animation);
            Assert.Equal(3, animation.Count);

            var steps = animation.Steps.ToArray();
            Assert.Equal(3, steps.Length);

            // step 1.1
            {
                var step = Assert.IsType<IncrementalAnimationStep>(steps[0]);
                Assert.Equal(1, step.Count);
                Assert.Equal("Exit", step.Name);

                var stepItems = step.ToArray();
                var item = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[0]);
                Assert.Equal(items[2].Frame, item.Property);
            }

            // step 1.2
            {
                var step = Assert.IsType<InstantaneousAnimationStep>(steps[1]);
                Assert.Equal("Hide", step.Name);
            }

            // step 2
            {
                var step = Assert.IsType<IncrementalAnimationStep>(steps[2]);
                Assert.Equal(2, step.Count);
                Assert.Equal("Layout", step.Name);

                var stepItems = step.ToArray();
                var item1 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[0]);
                var item2 = Assert.IsType<AnimationStepItem<RectangleF>>(stepItems[1]);
                Assert.Equal(items[0].Frame, item1.Property);
                Assert.Equal(items[1].Frame, item2.Property);
            }

            // step 2.2 is missing

            // step 3 is missing
        }

        [Fact]
        public void AnimationThatDoesNotAddStepsCorrectly()
        {
            var filterInitial = new[] { "A", "B", "C" };
            var filterUpdate = new[] { "A", "B" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });
            renderer.Filter = (item) => filterInitial.Contains(item);
            renderer.ResetLayout();

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var allItems = renderer.Items;
            var visibleItems = renderer.VisibleItems;

            var animation = renderer.Animation;
            Assert.NotNull(animation);

            // before step 1
            Assert.False(animation.IsComplete);
            Assert.Equal(3, visibleItems.Count);
            Assert.Equal(allItems[0], visibleItems[0]);
            Assert.Equal(allItems[1], visibleItems[1]);
            Assert.Equal(allItems[2], visibleItems[2]);

            // step 1
            animation.Update(Time.OneSec);

            // after step 1
            Assert.False(animation.IsComplete);
            Assert.Equal(2, visibleItems.Count);
            Assert.Equal(allItems[0], visibleItems[0]);
            Assert.Equal(allItems[1], visibleItems[1]);

            // step 2
            animation.Update(Time.OneSec);

            // after step 2
            Assert.True(animation.IsComplete);
            Assert.Equal(2, visibleItems.Count);
            Assert.Equal(allItems[0], visibleItems[0]);
            Assert.Equal(allItems[1], visibleItems[1]);

            // step 3 is missing
        }

        [Fact]
        public void AnimationWithNoStartingItemsStepsCorrectly()
        {
            var filterUpdate = new[] { "B", "C", "D" };

            var renderer = CreateTestRenderer(new[] { "A", "B", "C", "D" });

            renderer.Filter = (item) => filterUpdate.Contains(item);

            var allItems = renderer.Items;
            var visibleItems = renderer.VisibleItems;

            var animation = renderer.Animation;
            Assert.NotNull(animation);

            // before step
            Assert.False(animation.IsComplete);
            Assert.Equal(0, visibleItems.Count);

            // step 1 is missing

            // step 2
            animation.Update(Time.OneSec);

            // after step 2
            Assert.True(animation.IsComplete);
            Assert.Equal(3, visibleItems.Count);
            Assert.Equal(allItems[1], visibleItems[0]);
            Assert.Equal(allItems[2], visibleItems[1]);
            Assert.Equal(allItems[3], visibleItems[2]);

            // step 3 is missing
        }

        private static PivotRenderer CreateTestRenderer(params string[] items) =>
            new()
            {
                DataSource = CreateDataSource(items),
                Layout = new VerticalStackLayout(),
                LayoutTransition = new HorizontalOffsetLayoutTransition(),
                Frame = new RectangleF(0, 0, 120, 120),
                AnimationDelay = TimeSpan.Zero,
                AddItemsAnimationDuration = Time.OneSec,
                MoveItemsAnimationDuration = Time.OneSec,
                RemoveItemsAnimationDuration = Time.OneSec,
                AddItemsAnimationEasing = Easing.Linear,
                MoveItemsAnimationEasing = Easing.Linear,
                RemoveItemsAnimationEasing = Easing.Linear,
            };
    }
}
