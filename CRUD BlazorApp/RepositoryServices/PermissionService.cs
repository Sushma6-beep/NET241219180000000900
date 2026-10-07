using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CRUD_BlazorApp.IDataService;

namespace CRUD_BlazorApp.RepositoryServices
{
	public class PermissionService : IPermissionService
	{
		private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;

		public PermissionService(
			IDbContextFactory<ApplicationDbContext> dbContextFactory)
		{
			this.dbContextFactory = dbContextFactory;
		}

		public async Task<List<string>>
			GetActivePermissionsAsync(int userId)
		{
			await using var dbContext =
				await dbContextFactory.CreateDbContextAsync();
			var currentDate = DateTime.UtcNow;

			return await dbContext.UserRoles
				.Where(x =>
					x.UserId == userId &&
					x.IsActive &&
					x.ValidFrom <= currentDate &&
					(x.ValidTo == null ||
					 x.ValidTo >= currentDate))
				.Join(
					dbContext.Roles,
					userRole => userRole.RoleId,
					role => role.Id,
					(userRole, role) => role)
				.Where(role => role.IsActive)
				.SelectMany(
					role => dbContext.RolePermissions
						.Where(rp =>
							rp.RoleId == role.Id &&
							rp.IsActive)
						.Join(
							dbContext.Permissions,
							rp => rp.PermissionId,
							permission => permission.Id,
							(rp, permission) => permission))
				.Where(permission => permission.IsActive)
						.Select(permission => permission.Name)
								.Distinct()
								.ToListAsync();
						}
					}
				}
