namespace CRUD_BlazorApp.Models
{
	public class AuditModel
	{
		public long Id { get; set; }

		public int? UserId { get; set; }

		public string? UserName { get; set; }

		public string? AuthenticationType { get; set; }

		public string Action { get; set; } = string.Empty;

		public string Status { get; set; } = string.Empty;

		public string? Details { get; set; }

		public DateTime CreatedOn { get; set; }
	}
}
