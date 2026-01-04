using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace GraphBuilder.UI.Controls;

public class CoordinatePlane : Control
{
    public static readonly StyledProperty<IReadOnlyList<Point>?> PointsProperty =
        AvaloniaProperty.Register<CoordinatePlane, IReadOnlyList<Point>?>(nameof(Points));

    public IReadOnlyList<Point>? Points
    {
        get => GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    public double Scale { get; set; } = 50; // пикселей на единицу
    public Point Offset { get; set; } = new(0, 0);

    static CoordinatePlane()
    {
        // перерисовываться при изменении Points
        AffectsRender<CoordinatePlane>(PointsProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds;
        var center = new Point(
            bounds.Width / 2 + Offset.X,
            bounds.Height / 2 + Offset.Y
        );

        DrawGrid(context, bounds, center);
        DrawAxes(context, bounds, center);

        if (Points is null || Points.Count < 2)
            return;

        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            bool first = true;

            foreach (var p in Points)
            {
                var px = center.X + p.X * Scale;
                var py = center.Y - p.Y * Scale;

                if (first)
                {
                    ctx.BeginFigure(new Point(px, py), false);
                    first = false;
                }
                else
                {
                    ctx.LineTo(new Point(px, py));
                }
            }
        }

        context.DrawGeometry(null, new Pen(Brushes.Red, 2), geometry);
    }

    private void DrawAxes(DrawingContext ctx, Rect bounds, Point center)
    {
        var axisPen = new Pen(Brushes.Black, 2);

        // X
        ctx.DrawLine(axisPen,
            new Point(0, center.Y),
            new Point(bounds.Width, center.Y));

        // Y
        ctx.DrawLine(axisPen,
            new Point(center.X, 0),
            new Point(center.X, bounds.Height));
    }

    private void DrawGrid(DrawingContext ctx, Rect bounds, Point center)
    {
        var gridPen = new Pen(Brushes.LightGray, 1);

        for (double x = center.X % Scale; x < bounds.Width; x += Scale)
            ctx.DrawLine(gridPen, new Point(x, 0), new Point(x, bounds.Height));

        for (double y = center.Y % Scale; y < bounds.Height; y += Scale)
            ctx.DrawLine(gridPen, new Point(0, y), new Point(bounds.Width, y));
    }
}
