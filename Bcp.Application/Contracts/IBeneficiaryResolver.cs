using Bcp.Domain.Models;

namespace Bcp.Application.Contracts;

public interface IBeneficiaryResolver
{
    Task<Beneficiary> GetOrAddAsync(string cpf, string card, CancellationToken cancellationToken);
}
