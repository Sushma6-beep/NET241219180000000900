using System.ComponentModel.DataAnnotations;

namespace CRUD_BlazorApp.DTOs
{
	public class UpdateUserRequest
	{
		[Required(ErrorMessage = "Username is required")]
		[StringLength(100, MinimumLength = 3,
			ErrorMessage = "Username must be between 3 and 100 characters")]
		public string UserName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Valid email is required")]
		public string Email { get; set; } = string.Empty;
	}
}
