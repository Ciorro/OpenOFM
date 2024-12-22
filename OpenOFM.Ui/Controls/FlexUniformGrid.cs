using System.Windows;
using System.Windows.Controls;

namespace OpenOFM.Ui.Controls
{
    internal class FlexUniformGrid : Panel
    {
        public double MinItemWidth
        {
            get => (double)GetValue(MinItemWidthProperty);
            set => SetValue(MinItemWidthProperty, value);
        }

        public static readonly DependencyProperty MinItemWidthProperty = DependencyProperty.Register(
            "MinItemWidth",
            typeof(double),
            typeof(FlexUniformGrid),
            new PropertyMetadata(0.0));


        protected override Size MeasureOverride(Size constraint)
        {
            var columnCount = (int)Math.Max(constraint.Width / MinItemWidth, 1);
            var columnWidth = constraint.Width / columnCount;
            var totalHeight = 0.0;
            var rowHeight = 0.0;

            var childConstraint = new Size(columnWidth, constraint.Height);

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                child.Measure(childConstraint);

                rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);

                if (i % columnCount == columnCount - 1 || i == InternalChildren.Count - 1)
                {
                    totalHeight += rowHeight;
                    rowHeight = 0;
                }
            }

            return new Size(constraint.Width, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var columnCount = (int)Math.Max(finalSize.Width / MinItemWidth, 1);
            var columnWidth = finalSize.Width / columnCount;

            var rowHeight = 0.0;
            var x = 0.0;
            var y = 0.0;

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                if (i % columnCount == 0)
                {
                    y += rowHeight;
                    rowHeight = 0;
                    x = 0;
                }

                UIElement child = InternalChildren[i];
                child.Arrange(new Rect(
                    x,
                    y,
                    columnWidth,
                    child.DesiredSize.Height));

                rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
                x += columnWidth;
            }

            return finalSize;
        }
    }
}
