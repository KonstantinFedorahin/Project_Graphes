using System;
using Avalonia.Media;

namespace GraphBuilder.Domain.Models;
    
public class Function
{
    public string Expression { get; set; } = string.Empty;
    public Color Color { get; set; } = Colors.Blue;
    public double LineWidth { get; set; } = 2;
}