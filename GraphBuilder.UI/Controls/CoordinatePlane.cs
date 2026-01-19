using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;

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

    public static readonly StyledProperty<double> ScaleProperty =
    AvaloniaProperty.Register<CoordinatePlane, double>(nameof(Scale), 50);

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<CoordinatePlane, Point>(nameof(Offset));
    
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
    AvaloniaProperty.Register<CoordinatePlane, IBrush?>(nameof(Background));

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public double Scale
    {
        get => GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    public Point Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    static CoordinatePlane()
    {
        AffectsRender<CoordinatePlane>(
            PointsProperty,
            ScaleProperty,
            OffsetProperty);
    }
    private Point? _lastPanPoint;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsMiddleButtonPressed ||
            e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            _lastPanPoint = e.GetPosition(this);
            e.Pointer.Capture(this);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _lastPanPoint = null;
        e.Pointer.Capture(null);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_lastPanPoint is null)
            return;

        var current = e.GetPosition(this);
        var delta = current - _lastPanPoint.Value;

        Offset = new Point(
            Offset.X + delta.X,
            Offset.Y + delta.Y);

        _lastPanPoint = current;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        const double zoomFactor = 1.1;

        var mousePos = e.GetPosition(this);
        var oldScale = Scale;

        if (e.Delta.Y > 0)
            Scale *= zoomFactor;
        else
            Scale /= zoomFactor;

        Scale = Math.Clamp(Scale, 10, 500);

        // Adjust offset so zoom happens around mouse position
        var scaleRatio = Scale / oldScale;

        Offset = new Point(
            mousePos.X - scaleRatio * (mousePos.X - Offset.X),
            mousePos.Y - scaleRatio * (mousePos.Y - Offset.Y)
        );
    }


    public override void Render(DrawingContext context)
    {
        if (Background != null)
        context.FillRectangle(Background, Bounds);
        
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
