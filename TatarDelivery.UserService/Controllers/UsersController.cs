using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TatarDelivery.UserService.Contracts.Requests;
using TatarDelivery.UserService.Contracts.Responses;
using TatarDelivery.UserService.Contracts.Responses.Mappings;
using TatarDelivery.UserService.Data;
using TatarDelivery.UserService.Domain;
using TatarDelivery.UserService.Security;

namespace TatarDelivery.UserService.Controllers;

[ApiController]
[Route("users")]
[Produces("application/json")]
public sealed class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly PasswordHasher _passwordHasher;

    public UsersController(AppDbContext dbContext, PasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterUserRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var phone = request.Phone.Trim();

        var emailExists = await _dbContext.Users.AnyAsync(user => user.Email == email);
        if (emailExists)
        {
            return Conflict(new ErrorResponse("A user with this email already exists."));
        }

        _passwordHasher.HashPassword(request.Password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            FullName = request.FullName.Trim(),
            Phone = phone,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, user.ToResponse());
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.FirstOrDefaultAsync(entity => entity.Email == email);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Unauthorized(new ErrorResponse("Invalid email or password."));
        }

        user.AuthToken = Guid.NewGuid().ToString("N");
        user.UpdatedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new AuthResponse("Bearer", user.AuthToken, user.ToResponse()));
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized(new ErrorResponse("User is not authenticated."));
        }

        return Ok(user.ToResponse());
    }

    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UpdateProfileRequest request)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized(new ErrorResponse("User is not authenticated."));
        }

        user.FullName = request.FullName.Trim();
        user.Phone = request.Phone.Trim();
        user.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(user.ToResponse());
    }

    [HttpGet("me/addresses")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyCollection<AddressResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<AddressResponse>>> GetAddresses()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(new ErrorResponse("User is not authenticated."));
        }

        var addresses = await _dbContext.Addresses
            .AsNoTracking()
            .Where(address => address.UserId == userId.Value)
            .OrderByDescending(address => address.IsDefault)
            .ThenBy(address => address.Id)
            .Select(address => address.ToResponse())
            .ToListAsync();

        return Ok(addresses);
    }

    [HttpPost("me/addresses")]
    [Authorize]
    [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AddressResponse>> AddAddress([FromBody] CreateAddressRequest request)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized(new ErrorResponse("User is not authenticated."));
        }

        var shouldBeDefault = request.IsDefault || !await _dbContext.Addresses.AnyAsync(address => address.UserId == user.Id);
        if (shouldBeDefault)
        {
            await _dbContext.Addresses
                .Where(address => address.UserId == user.Id && address.IsDefault)
                .ExecuteUpdateAsync(setters => setters.SetProperty(address => address.IsDefault, false));
        }

        var address = new Address
        {
            UserId = user.Id,
            City = request.City.Trim(),
            Street = request.Street.Trim(),
            House = request.House.Trim(),
            Apartment = string.IsNullOrWhiteSpace(request.Apartment) ? null : request.Apartment.Trim(),
            Entrance = string.IsNullOrWhiteSpace(request.Entrance) ? null : request.Entrance.Trim(),
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            IsDefault = shouldBeDefault,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, address.ToResponse());
    }

    private int? GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out var userId) ? userId : null;
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return null;
        }

        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId.Value);
    }
}
