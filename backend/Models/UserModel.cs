using Microsoft.AspNetCore.Identity;

namespace FormatChanger.WebAPI.Models
{
	public class UserModel : IdentityUser
	{
		/// <summary>
		/// Имя пользователя в Telegram
		/// </summary>
		public string TelegramUserName { get; set; }
	}
}
