using Bcp.Domain.Models;

namespace Bcp.Application.Contracts;

public interface IStoreResolver
{
    Task<Store> GetOrAddAsync(string name, string owner, CancellationToken cancellationToken);
}
