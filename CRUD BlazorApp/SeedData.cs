using CRUD_BlazorApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRUD_BlazorApp
{
	public static class SeedData
	{
		public static void Initialize(IServiceProvider serviceProvider)
		{
			using (var context = new ApplicationDbContext(
				serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
			{
				// Check if data already exists
				if (context.Users.Any() || context.Roles.Any() || context.Permissions.Any())
				{
					return;
				}

				// Seed Permissions
				var permissions = new List<PermissionModel>
				{
					new PermissionModel { Name = "User.View", IsActive = true },
					new PermissionModel { Name = "User.Create", IsActive = true },
					new PermissionModel { Name = "User.Edit", IsActive = true },
					new PermissionModel { Name = "User.Delete", IsActive = true }
				};

				context.Permissions.AddRange(permissions);
				context.SaveChanges();

				// Seed Roles
				var adminRole = new RoleModel
				{
					Name = "Admin",
					IsActive = true
				};

				var userRole = new RoleModel
				{
					Name = "User",
					IsActive = true
				};

				context.Roles.AddRange(adminRole, userRole);
				context.SaveChanges();

				// Seed RolePermissions (Admin gets all permissions)
				var rolePermissions = new List<RolePermissionModel>
				{
					new RolePermissionModel { RoleId = adminRole.Id, PermissionId = permissions[0].Id, IsActive = true },
					new RolePermissionModel { RoleId = adminRole.Id, PermissionId = permissions[1].Id, IsActive = true },
					new RolePermissionModel { RoleId = adminRole.Id, PermissionId = permissions[2].Id, IsActive = true },
					new RolePermissionModel { RoleId = adminRole.Id, PermissionId = permissions[3].Id, IsActive = true },

					// User role only gets View permission
					new RolePermissionModel { RoleId = userRole.Id, PermissionId = permissions[0].Id, IsActive = true }
				};

				context.RolePermissions.AddRange(rolePermissions);
				context.SaveChanges();

				// Seed Users
				var passwordHasher = new PasswordHasher<UserModel>();
				var adminUser = new UserModel
				{
					UserName = "admin",
					Email = "admin@example.com",
					IsActive = true,
					CreatedAt = DateTime.UtcNow
				};

				adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");

				var testUser = new UserModel
				{
					UserName = "user",
					Email = "user@example.com",
					IsActive = true,
					CreatedAt = DateTime.UtcNow
				};

				testUser.PasswordHash = passwordHasher.HashPassword(testUser, "User@123");

				context.Users.AddRange(adminUser, testUser);
				context.SaveChanges();

				// Seed UserRoles
				var userRoles = new List<UserRoleModel>
				{
					new UserRoleModel
					{
						UserId = adminUser.Id,
						RoleId = adminRole.Id,
						IsActive = true,
						ValidFrom = DateTime.UtcNow
					},
					new UserRoleModel
					{
						UserId = testUser.Id,
						RoleId = userRole.Id,
						IsActive = true,
						ValidFrom = DateTime.UtcNow
					}
				};

				context.UserRoles.AddRange(userRoles);
				context.SaveChanges();
			}
		}
	}
}
