using System.ComponentModel.DataAnnotations;

namespace HouseRentingSystemApi.Models.Auth
{
	public class AuthModel
	{
		[Required]
		[StringLength(20, MinimumLength = 3)]
		public string Username { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 6)]
		public string Password { get; set; }
		[Required]
		[EmailAddress]
		public string  Email { get; set; }

		// Optional at registration. Allowed values: "Client" or "Agent".
		// Defaults to "Client" when not provided.
		public string? Role { get; set; }
	}
}
