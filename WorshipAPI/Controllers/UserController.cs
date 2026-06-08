using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models.Users;
using Services.Users;

namespace WorshipAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/User
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        // Map to DTO to exclude hashed password
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            Status = u.Status
        });

        return Ok(userDtos);
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound();

        // Map to DTO to exclude hashed password
        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Status = user.Status
        };

        return Ok(userDto);
    }

    // POST: api/User
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createDto)
    {
        // Map DTO to User entity
        var user = new User
        {
            Name = createDto.Name,
            Email = createDto.Email,
            Role = createDto.Role,
            HashedPassword = createDto.Password, // Will be hashed in service
            Status = createDto.Status
        };

        var createdUser = await _userService.CreateUserAsync(user);

        // Map to DTO for response
        var userDto = new UserDto
        {
            Id = createdUser.Id,
            Name = createdUser.Name,
            Email = createdUser.Email,
            Role = createdUser.Role,
            Status = createdUser.Status
        };

        return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
    }

    // PUT: api/User/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
    {
        // Map DTO to User entity
        var user = new User
        {
            Name = updateDto.Name,
            Email = updateDto.Email,
            Role = updateDto.Role,
            HashedPassword = updateDto.Password ?? string.Empty // Will be hashed in service if provided
        };

        var updatedUser = await _userService.UpdateUserAsync(id, user);

        if (updatedUser == null)
            return NotFound();

        // Map to DTO for response
        var userDto = new UserDto
        {
            Id = updatedUser.Id,
            Name = updatedUser.Name,
            Email = updatedUser.Email,
            Role = updatedUser.Role,
            Status = updatedUser.Status
        };

        return Ok(userDto);
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }

    // POST: api/User/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var response = await _userService.LoginAsync(loginDto);

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }
}
