using CRUD_BlazorApp.IDataService;
using CRUD_BlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD_BlazorApp.RepositoryServices
{
	public class RoleService : IRoleService
	{
		private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;

		public RoleService(
			IDbContextFactory<ApplicationDbContext> dbContextFactory)
		{
			this.dbContextFactory = dbContextFactory;
		}

		public async Task<List<string>> GetActiveRolesAsync(
			int userId)
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
				.Select(role => role.Name)
				.Distinct()
				.ToListAsync();
		}
	}
}
