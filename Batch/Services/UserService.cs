using Batch.Data;
using Batch.Models;

namespace Batch.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<User> GetUserByIdAsync(int id)
    {
        return _userRepository.GetUserByIdAsync(id);
    }
}