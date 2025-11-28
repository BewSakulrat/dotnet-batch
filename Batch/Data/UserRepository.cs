using Batch.Data;
using Microsoft.EntityFrameworkCore;

namespace Batch.Models;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User> GetUserByIdAsync(int id)
    {
        return _dbContext.Users.FirstOrDefaultAsync();
    }
}