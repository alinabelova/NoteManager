using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NoteManager.DataAccess;
using NoteManager.Models;

namespace NoteManager.Web
{
    public static class SampleData
    {
        public static async Task SeedData(this IApplicationBuilder builder)
        {
            var provider = builder.ApplicationServices;
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>())
            {

                var user = new ApplicationUser
                {
                    UserName = "test@email.com",
                    NormalizedUserName = "test@email.com",
                    Email = "test@email.com",
                    NormalizedEmail = "test@email.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@email.com",
                    NormalizedUserName = "admin@email.com",
                    Email = "admin@email.com",
                    NormalizedEmail = "admin@email.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var roleStore = new RoleStore<IdentityRole>(dbContext);

                if (!dbContext.Roles.Any(r => r.Name == "admin"))
                {
                    await roleStore.CreateAsync(new IdentityRole {Name = "admin", NormalizedName = "admin"});
                }

                if (!dbContext.Roles.Any(r => r.Name == "user"))
                {
                    await roleStore.CreateAsync(new IdentityRole { Name = "user", NormalizedName = "user" });
                }

                if (!dbContext.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(user, "password");
                    user.PasswordHash = hashed;
                    var userStore = new UserStore<ApplicationUser>(dbContext);
                    await userStore.CreateAsync(user);
                    await userStore.AddToRoleAsync(user, "user");
                }

                if (!dbContext.Users.Any(u => u.UserName == adminUser.UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(adminUser, "password");
                    adminUser.PasswordHash = hashed;
                    var userStore = new UserStore<ApplicationUser>(dbContext);
                    await userStore.CreateAsync(adminUser);
                    await userStore.AddToRoleAsync(adminUser, "admin");
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}

