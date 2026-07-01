using InventorySystem.Common.Utilities.Repositories.Seeds;
using InventorySystem.Common.Utilities.Responses;
using InventorySystem.Identity.Application.Dto;

namespace InventorySystem.Identity.Application.Services;

public interface IUserService : ISeed
{
    Task<Result<LoginResponse>> LoginAsync(LoginInput input, CancellationToken cancellationToken = default);
}
