namespace CRUD_BlazorApp.Models
{
	public class PermissionModel
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public bool IsActive { get; set; }
	}
}
