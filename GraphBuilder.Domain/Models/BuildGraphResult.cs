namespace GraphBuilder.Domain.Models;

public class BuildGraphResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public Graph? Graph { get; init; }
}
