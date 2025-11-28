using Batch.Data;

namespace Batch.Models;

public interface IUserRepository
{
    public Task<User> GetUserByIdAsync(int id);
}