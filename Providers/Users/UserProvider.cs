using Dapper;
using Models.Users;
using Providers.Database;
using System.Data;

namespace Providers.Users;

public class UserProvider : IUserProvider
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserProvider(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<User>(
            "sp_GetAllUsers",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "sp_GetUserById",
            new { p_Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleAsync<int>(
            "sp_CreateUser",
            new
            {
                p_Name = user.Name,
                p_Email = user.Email,
                p_Role = user.Role,
                p_HashedPassword = user.HashedPassword,
                p_Status = user.Status
            },
            commandType: CommandType.StoredProcedure);

        user.Id = result;
        return user;
    }

    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<int>(
            "sp_UpdateUser",
            new
            {
                p_Id = id,
                p_Name = user.Name,
                p_Email = user.Email,
                p_Role = user.Role,
                p_HashedPassword = user.HashedPassword
            },
            commandType: CommandType.StoredProcedure);

        if (result == 0)
            return null;

        user.Id = id;
        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<int>(
            "sp_DeleteUser",
            new { p_Id = id },
            commandType: CommandType.StoredProcedure);

        return result > 0;
    }
}
