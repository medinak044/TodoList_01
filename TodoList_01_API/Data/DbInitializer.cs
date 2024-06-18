using Microsoft.AspNetCore.Identity;
using TodoList_01_API.Models;

namespace TodoList_01_API.Data;

public class DbInitializer: IDbInitializer
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task Initialize()
    {
        #region Account roles (Identity framework)
        //if (await _roleManager.FindByNameAsync(UserRoles.User.ToLower()) == null)
        //{
        //    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        //}
        //if (await _roleManager.FindByNameAsync(UserRoles.Admin.ToLower()) == null)
        //{
        //    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        //}
        #endregion

        var demoIdentityPassword = "Password!23";
        if (await _userManager.FindByEmailAsync("demo@example.com") == null)
        {
            var adminUser = new AppUser()
            {
                UserName = "demo@example.com",
                Email = "demo@example.com",
                DateCreated = DateTime.UtcNow,
            };
            await _userManager.CreateAsync(adminUser, demoIdentityPassword);
            // After user is created, add role
            //var foundUser = await _userManager.FindByEmailAsync(adminUser.Email);
            //await _userManager.AddToRoleAsync(foundUser, UserRoles.User);
            //await _userManager.AddToRoleAsync(foundUser, UserRoles.Admin);
        }
    }
}
