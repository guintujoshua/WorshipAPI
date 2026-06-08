using Models.Users;
using Providers.Users;

namespace Services.Users;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(int id, User user);
    Task<bool> DeleteUserAsync(int id);
    Task<User?> AuthenticateUserAsync(string email, string password);
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
}
