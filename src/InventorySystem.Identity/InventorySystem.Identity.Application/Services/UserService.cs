using InventorySystem.Identity.Domain.Models;
using InventorySystem.Identity.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace InventorySystem.Identity.Application.Services;

public class UserService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<SeedOptions> seedOptions) : IUserService
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var options = seedOptions.Value;
        await EnsureUserAsync(options.AdminUser, "Admin");
        await EnsureUserAsync(options.NormalUser, "User");
    }

    private async Task EnsureUserAsync(SeedUserOptions options, string role)
    {
        if (await userManager.FindByEmailAsync(options.Email) != null)
            return;

        var user = new User { UserName = options.Email, Email = options.Email };
        await userManager.CreateAsync(user, options.Password);
        await userManager.AddToRoleAsync(user, role);
    }
}
