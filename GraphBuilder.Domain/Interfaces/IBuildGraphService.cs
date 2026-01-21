using GraphBuilder.Domain.Models;


namespace GraphBuilder.Domain.Interfaces;

public interface IBuildGraphService
{
    BuildGraphResult Execute(BuildGraphRequest request);
}
