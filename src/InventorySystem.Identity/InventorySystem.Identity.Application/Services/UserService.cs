using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventorySystem.Common.Utilities.Responses;
using InventorySystem.Identity.Application.Dto;
using InventorySystem.Identity.Domain.Models;
using InventorySystem.Identity.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InventorySystem.Identity.Application.Services;

public class UserService(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<SeedOptions> seedOptions,
    IOptions<JwtOptions> jwtOptions,
    ILogger<UserService> logger) : IUserService
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Role {Role} created", role);
            }
        }

        var options = seedOptions.Value;
        await EnsureUserAsync(options.AdminUser, "Admin");
        await EnsureUserAsync(options.NormalUser, "User");
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginInput input, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(input.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, input.Password))
        {
            logger.LogWarning("Login failed for {Email}", input.Email);
            return Result<LoginResponse>.Failure([
                new ErrorMessage { Code = "auth.invalid_credentials", Message = "Credenciales inválidas" }
            ]);
        }

        var roles = await userManager.GetRolesAsync(user);
        var response = GenerateToken(user, roles);

        logger.LogInformation("User {Email} logged in", input.Email);

        return Result<LoginResponse>.Success(response);
    }

    private LoginResponse GenerateToken(User user, IList<string> roles)
    {
        var options = jwtOptions.Value;
        var expiresAt = DateTime.UtcNow.AddMinutes(options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }

    private async Task EnsureUserAsync(SeedUserOptions options, string role)
    {
        if (await userManager.FindByEmailAsync(options.Email) != null)
            return;

        var user = new User { UserName = options.Email, Email = options.Email };
        await userManager.CreateAsync(user, options.Password);
        await userManager.AddToRoleAsync(user, role);

        logger.LogInformation("Seed user {Email} created with role {Role}", options.Email, role);
    }
}