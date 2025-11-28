using Batch.Data;

namespace Batch.Services;

public interface IUserService
{
    public Task<User> GetUserByIdAsync(int id);
}