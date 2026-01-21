using System.Collections.Generic;
using Avalonia;
using System;

namespace GraphBuilder.UI.Rendering;

public class GraphRenderData
{
    public IReadOnlyList<Point> Points { get; init; } = Array.Empty<Point>();
}
