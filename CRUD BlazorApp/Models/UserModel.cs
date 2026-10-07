namespace CRUD_BlazorApp.Models
{
	public class UserModel
	{
		public int Id { get; set; }

		public string UserName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string PasswordHash { get; set; } = string.Empty;

		public bool IsActive { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
