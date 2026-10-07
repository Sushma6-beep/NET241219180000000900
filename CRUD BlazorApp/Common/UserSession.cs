namespace CRUD_BlazorApp.Common
{
	public class UserSession
	{
		public int UserId { get; set; }

		public string UserName { get; set; } = string.Empty;

		public string AuthenticationType { get; set; } = string.Empty;

		public List<string> Roles { get; set; } = new();

		public List<string> Permissions { get; set; } = new();

		public string Language { get; set; } = "en";

		public string Theme { get; set; } = "light";

		public DateTime LoginTime { get; set; }

		public bool IsAuthenticated =>
			UserId > 0 && Roles.Count > 0;

		public bool HasPermission(string permission)
		{
			return Permissions.Contains(permission);
		}

		public bool HasRole(string role)
		{
			return Roles.Contains(role);
		}

		public void Clear()
		{
			UserId = 0;
			UserName = string.Empty;
			AuthenticationType = string.Empty;

			Roles.Clear();
			Permissions.Clear();

			Language = "en";
			Theme = "light";
			LoginTime = default;
		}
	}
}
