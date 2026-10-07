using CRUD_BlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD_BlazorApp
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(
			DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<UserModel> Users => Set<UserModel>();

		public DbSet<RoleModel> Roles => Set<RoleModel>();

		public DbSet<UserRoleModel> UserRoles => Set<UserRoleModel>();

		public DbSet<PermissionModel> Permissions =>
			Set<PermissionModel>();

		public DbSet<RolePermissionModel> RolePermissions =>
			Set<RolePermissionModel>();

		public DbSet<AuditModel> AuditLogs =>
           Set<AuditModel>();

		protected override void OnModelCreating(
			ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserModel>()
				.ToTable("Users");

			modelBuilder.Entity<RoleModel>()
				.ToTable("Roles");

			modelBuilder.Entity<UserRoleModel>()
				.ToTable("UserRoles");

			modelBuilder.Entity<PermissionModel>()
				.ToTable("Permissions");

			modelBuilder.Entity<RolePermissionModel>()
				.ToTable("RolePermissions");

			modelBuilder.Entity<UserRoleModel>()
				.HasKey(x => new
				{
					x.UserId,
					x.RoleId
				});

			modelBuilder.Entity<UserRoleModel>()
				.HasOne(ur => ur.Role)
				.WithMany()
				.HasForeignKey(ur => ur.RoleId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<RolePermissionModel>()
				.HasKey(x => new
				{
					x.RoleId,
					x.PermissionId
				});

			modelBuilder.Entity<AuditModel>()
           .ToTable("AuditLogs");
		}
	}
}

