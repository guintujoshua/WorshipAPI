using Models.Users;
using Providers.Users;
using Services.Security;

namespace Services.Users;

public class UserService : IUserService
{
    private readonly IUserProvider _userProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UserService(IUserProvider userProvider, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userProvider = userProvider;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = await _userProvider.GetAllUsersAsync();

        // Note: We don't return hashed passwords to clients
        // You might want to create a UserDto without HashedPassword field
        return users;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var user = await _userProvider.GetUserByIdAsync(id);

        // Note: We don't return hashed passwords to clients
        // You might want to create a UserDto without HashedPassword field
        return user;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        // Hash the password before storing
        if (!string.IsNullOrWhiteSpace(user.HashedPassword))
        {
            user.HashedPassword = _passwordHasher.HashPassword(user.HashedPassword);
        }

        return await _userProvider.CreateUserAsync(user);
    }

    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        // Hash the password if it's being updated
        if (!string.IsNullOrWhiteSpace(user.HashedPassword))
        {
            user.HashedPassword = _passwordHasher.HashPassword(user.HashedPassword);
        }

        return await _userProvider.UpdateUserAsync(id, user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _userProvider.DeleteUserAsync(id);
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        // Get all users and find by email
        var users = await _userProvider.GetAllUsersAsync();
        var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (user == null)
            return null;

        // Verify password
        if (!_passwordHasher.VerifyPassword(password, user.HashedPassword))
            return null;

        // Check if user is active
        if (user.Status.Equals("Deleted", StringComparison.OrdinalIgnoreCase) || 
            user.Status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
            return null;

        return user;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Authenticate user
        var user = await AuthenticateUserAsync(loginDto.Email, loginDto.Password);

        if (user == null)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        // Generate JWT token
        var token = _jwtTokenGenerator.GenerateToken(
            user.Id.ToString(),
            user.Email,
            user.Role
        );

        // Map to UserDto to avoid exposing hashed password
        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Status = user.Status
        };

        return new LoginResponseDto
        {
            Success = true,
            Message = "Login successful",
            User = userDto,
            Token = token
        };
    }
}
